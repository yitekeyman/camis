using System;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Newtonsoft.Json;
using Stateless;

namespace intapscamis.camis.domain.Farms.StateMachines
{
    public class FarmDeletionWorkflow : CamisService
    {
        public enum States
        {
            Filing = 0,
            Reviewing = 1,
            Rejected = -1,
            Approved = -2
        }

        public enum Triggers
        {
            Request = 1,
            Reject = 2,
            Approve = 3
        }

        private readonly IFarmsService _service;

        private readonly IWorkflowService _workflowService;

        private StateMachine<States, Triggers> _machine;

        public FarmDeletionWorkflow(IFarmsService service, IWorkflowService workflowService)
        {
            _service = service;
            _workflowService = workflowService;
        }


        public Workflow Workflow { get; set; }


        public void SetSession(UserSession session)
        {
            _service.SetSession(session);
            _workflowService.SetSession(session);
        }
        
        public override void SetContext(CamisContext value)
        {
            Context = value;
            _service.SetContext(Context);
            _workflowService.SetContext(Context);
        }


        // create new workflow
        public void ConfigureMachine()
        {
            Workflow = _workflowService.CreateWorkflow(new WorkflowRequest
            {
                CurrentState = (int) States.Filing,
                Description = "New farm deletion.",
                TypeId = (int) WorkflowTypes.FarmDeletion
            });
            _machine = new StateMachine<States, Triggers>(States.Filing);

            DefineStateMachine();
        }

        // access existing workflow
        public void ConfigureMachine(Guid workflowId)
        {
            Workflow = Context.Workflow.First(wf =>
                wf.Id == workflowId && wf.TypeId == (int) WorkflowTypes.FarmDeletion);
            _machine = new StateMachine<States, Triggers>((States) Workflow.CurrentState);

            DefineStateMachine();
        }

        private void DefineStateMachine()
        {
            ParameterizedTriggers.ConfigureParameters(_machine);

            _machine.Configure(States.Filing)
                .Permit(Triggers.Request, States.Reviewing);

            _machine.Configure(States.Reviewing)
                .OnEntryFrom(ParameterizedTriggers.Request, OnRequest)
                .Permit(Triggers.Reject, States.Rejected)
                .Permit(Triggers.Approve, States.Approved);

            _machine.Configure(States.Rejected)
                .OnEntryFrom(ParameterizedTriggers.Reject, OnReject);

            _machine.Configure(States.Approved)
                .OnEntryFrom(ParameterizedTriggers.Approve, OnApprove);
        }


        public void Fire(Guid workflowId, StateMachine<States, Triggers>.TriggerWithParameters<string, long?> trigger,
            string description, long? assignedUser)
        {
            _machine.Fire(trigger, description, assignedUser);
            _workflowService.UpdateWorkflow(workflowId, (int) _machine.State, description);
        }

        public void Fire(Guid workflowId,
            StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> trigger, FarmRequest data,
            string description, long? assignedUser)
        {
            _machine.Fire(trigger, data, description, assignedUser);
            _workflowService.UpdateWorkflow(workflowId, (int) _machine.State, description);
        }


        private void OnRequest(FarmRequest data, string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.FarmSupervisor, data, description, assignedUser, transition);
        }

        private void OnReject(string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(null, GetData(), description, assignedUser, transition);
        }

        private void OnApprove(string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            var data = GetData();

            ConfigureAndAddWorkItem(null, data, description, assignedUser, transition);

            // the real act
            if (data.Id != null) _service.DeleteFarm(data);
            if (data.OperatorId != null) _service.DeleteFarmOperator(new FarmOperatorRequest {Id = data.OperatorId});
        }


        private FarmRequest GetData()
        {
            var workItem = Context.WorkItem.Where(wi => wi.WorkflowId == Workflow.Id).OrderBy(wi => wi.SeqNo)
                .LastOrDefault();

            return workItem != null ? JsonConvert.DeserializeObject<FarmRequest>(workItem.Data) : null;
        }

        private void ConfigureAndAddWorkItem(long? role, FarmRequest data, string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            var workItemId = Guid.NewGuid();
             var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "C:\\usr\\bin\\CAMIS\\data\\docs";
            var fileSavePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory, workItemId.ToString());
            if (!Directory.Exists(fileSavePath))
            {
                Directory.CreateDirectory(fileSavePath);
            }

            // Process photo - Copy from previous work item if exists
            if (data?.Operator?.Photo != null)
            {
                const string pathPrefix = "/api/Farms/InWorkItemOperatorPhoto/";

                // Check if this is a reference to a file from a previous work item
                if (data.Operator.Photo.OverrideFilePath != null &&
                    data.Operator.Photo.OverrideFilePath.Contains(pathPrefix))
                {
                    // Extract previous work item ID and copy the file
                    var previousWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                    if (previousWorkItem != null)
                    {
                        var previousPath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            previousWorkItem.Id.ToString());
                        var sourceFilePath = Path.Combine(previousPath, $"{data.Operator.Photo.Id}");
                        var destFilePath = Path.Combine(fileSavePath, $"{data.Operator.Photo.Id}");

                        if (File.Exists(sourceFilePath))
                        {
                            File.Copy(sourceFilePath, destFilePath, true);
                        }
                    }
                }
                else if (data.Operator.Photo.File != null)
                {
                    // New file - save to disk
                    data.Operator.Photo.Id = data.Operator.Photo.Id ?? Guid.NewGuid();
                    var filePath = Path.Combine(fileSavePath, $"{data.Operator.Photo.Id}");

                    var fileBytes = Convert.FromBase64String(data.Operator.Photo.File);
                    File.WriteAllBytes(filePath, fileBytes);
                    data.Operator.Photo.File = null; // Set to null after saving to disk
                }

                // Update the override file path
                data.Operator.Photo.OverrideFilePath = $"{pathPrefix}{workItemId}?photoId={data.Operator.Photo.Id}";
            }

            // Process Registrations
            if (data?.Registrations != null)
            {
                var i = -1;
                const string pathPrefix = "/api/Farms/InWorkItemRegistrationFile/";

                foreach (var reg in data.Registrations)
                {
                    if (reg.Document == null) continue;

                    reg.Id = i;
                    i--;

                    // Check if this is a reference to a file from a previous work item
                    if (reg.Document.OverrideFilePath != null &&
                        reg.Document.OverrideFilePath.Contains(pathPrefix))
                    {
                        var previousWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                        if (previousWorkItem != null)
                        {
                            var previousPath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                                previousWorkItem.Id.ToString());
                            var sourceFilePath = Path.Combine(previousPath, $"{reg.Document.Id}");
                            var destFilePath = Path.Combine(fileSavePath, $"{reg.Document.Id}");

                            if (File.Exists(sourceFilePath))
                            {
                                File.Copy(sourceFilePath, destFilePath, true);
                            }
                        }
                    }
                    else if (reg.Document.File != null)
                    {
                        // New file - save to disk
                        reg.Document.Id = reg.Document.Id ?? Guid.NewGuid();
                        var filePath = Path.Combine(fileSavePath, $"{reg.Document.Id}");

                        var fileBytes = Convert.FromBase64String(reg.Document.File);
                        File.WriteAllBytes(filePath, fileBytes);
                        reg.Document.File = null; // Set to null after saving
                    }

                    reg.Document.OverrideFilePath = $"{pathPrefix}{workItemId}?regId={reg.Id}";
                }
            }

            // Process Operator Registrations (similar pattern)
            if (data?.Operator?.Registrations != null)
            {
                var i = -1;
                const string pathPrefix = "/api/Farms/InWorkItemOperatorRegistrationFile/";

                foreach (var reg in data.Operator.Registrations)
                {
                    if (reg.Document == null) continue;

                    reg.Id = i;
                    i--;

                    if (reg.Document.OverrideFilePath != null &&
                        reg.Document.OverrideFilePath.Contains(pathPrefix))
                    {
                        var previousWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                        if (previousWorkItem != null)
                        {
                            var previousPath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                                previousWorkItem.Id.ToString());
                            var sourceFilePath = Path.Combine(previousPath, $"{reg.Document.Id}");
                            var destFilePath = Path.Combine(fileSavePath, $"{reg.Document.Id}");

                            if (File.Exists(sourceFilePath))
                            {
                                File.Copy(sourceFilePath, destFilePath, true);
                            }
                        }
                    }
                    else if (reg.Document.File != null)
                    {
                        reg.Document.Id = reg.Document.Id ?? Guid.NewGuid();
                        var filePath = Path.Combine(fileSavePath, $"{reg.Document.Id}");

                        var fileBytes = Convert.FromBase64String(reg.Document.File);
                        File.WriteAllBytes(filePath, fileBytes);
                        reg.Document.File = null;
                    }

                    reg.Document.OverrideFilePath = $"{pathPrefix}{workItemId}?regId={reg.Id}";
                }
            }

            // Process ActivityPlan Documents (similar pattern)
            if (data?.ActivityPlan?.Documents != null)
            {
                const string pathPrefix = "/api/Farms/InWorkItemActivityPlanFile/";

                foreach (var doc in data.ActivityPlan.Documents)
                {
                    if (doc == null) continue;

                    if (doc.OverrideFilePath != null &&
                        doc.OverrideFilePath.Contains(pathPrefix))
                    {
                        var previousWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                        if (previousWorkItem != null)
                        {
                            var previousPath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                                previousWorkItem.Id.ToString());
                            var sourceFilePath = Path.Combine(previousPath, $"{doc.Id}");
                            var destFilePath = Path.Combine(fileSavePath, $"{doc.Id}");

                            if (File.Exists(sourceFilePath))
                            {
                                File.Copy(sourceFilePath, destFilePath, true);
                            }
                        }
                    }
                    else if (doc.File != null)
                    {
                        doc.Id = doc.Id ?? Guid.NewGuid();
                        var filePath = Path.Combine(fileSavePath, $"{doc.Id}");

                        var fileBytes = Convert.FromBase64String(doc.File);
                        File.WriteAllBytes(filePath, fileBytes);
                        doc.File = null;
                    }

                    doc.OverrideFilePath = $"{pathPrefix}{workItemId}?documentId={doc.Id}";
                }
            }
            
            _workflowService.CreateWorkItem(new WorkItemRequest
            {
                Id = workItemId,
                WorkflowId = Workflow.Id.ToString(),
                FromState = (int) transition.Source,
                ToState = (int) transition.Destination,
                Trigger = (int) transition.Trigger,
                DataType = typeof(FarmRequest).ToString(),
                Data = data != null ? JsonConvert.SerializeObject(data) : null,
                Description = description,
                AssignedRole = role,
                AssignedUser = assignedUser
            });
        }


        public static class ParameterizedTriggers
        {
            public static StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> Request;
            public static StateMachine<States, Triggers>.TriggerWithParameters<string, long?> Reject;
            public static StateMachine<States, Triggers>.TriggerWithParameters<string, long?> Approve;

            public static void ConfigureParameters(StateMachine<States, Triggers> machine)
            {
                Request = machine.SetTriggerParameters<FarmRequest, string, long?>(Triggers.Request);
                Reject = machine.SetTriggerParameters<string, long?>(Triggers.Reject);
                Approve = machine.SetTriggerParameters<string, long?>(Triggers.Approve);
            }
        }
        
    }
}