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
    public class FarmRegistrationWorkflow : CamisService
    {
        public enum States
        {
            Filing = 0,
            Cancelled = -1,
            Reviewing = 1,
            Approved = -3,
        }

        public enum Triggers
        {
            Save = 0,
            Cancel = 1,
            Request = 2,
            Reject = 3,
            Approve = 4,
        }

        private readonly IFarmsService _service;
        private readonly IWorkflowService _workflowService;
        private readonly LandAssignmentWorkflow _landAssignmentWorkflow;

        private StateMachine<States, Triggers> _machine;

        public FarmRegistrationWorkflow(
            IFarmsService service,
            IWorkflowService workflowService,
            LandAssignmentWorkflow landAssignmentWorkflow)
        {
            _service = service;
            _workflowService = workflowService;
            _landAssignmentWorkflow = landAssignmentWorkflow;
        }


        public Workflow Workflow { get; private set; }


        public void SetSession(UserSession session)
        {
            _service.SetSession(session);
            _workflowService.SetSession(session);
            _landAssignmentWorkflow.SetSession(session);
        }

        public override void SetContext(CamisContext value)
        {
            base.SetContext(value);
            _service.SetContext(Context);
            _workflowService.SetContext(Context);
            _landAssignmentWorkflow.SetContext(Context);
        }


        // create new workflow
        public void ConfigureMachine()
        {
            Workflow = _workflowService.CreateWorkflow(new WorkflowRequest
            {
                CurrentState = (int)States.Filing,
                Description = "New farm registration.",
                TypeId = (int)WorkflowTypes.FarmRegistration
            });
            _machine = new StateMachine<States, Triggers>(States.Filing);

            DefineStateMachine();
        }

        // access existing workflow
        public void ConfigureMachine(Guid workflowId)
        {
            Workflow = Context.Workflow.First(wf =>
                wf.Id == workflowId && wf.TypeId == (int)WorkflowTypes.FarmRegistration);
            _machine = new StateMachine<States, Triggers>((States)Workflow.CurrentState);

            DefineStateMachine();
        }

        private void DefineStateMachine()
        {
            ParameterizedTriggers.ConfigureParameters(_machine);

            _machine.Configure(States.Filing)
                .OnEntryFrom(ParameterizedTriggers.Reject, OnReject)
                .OnEntryFrom(ParameterizedTriggers.Save, OnSave)
                .PermitReentry(Triggers.Save)
                .Permit(Triggers.Cancel, States.Cancelled)
                .Permit(Triggers.Request, States.Reviewing);

            _machine.Configure(States.Cancelled)
                .OnEntryFrom(ParameterizedTriggers.Cancel, OnCancel);

            _machine.Configure(States.Reviewing)
                .OnEntryFrom(ParameterizedTriggers.Request, OnRequest)
                .Permit(Triggers.Reject, States.Filing)
                .Permit(Triggers.Approve, States.Approved);

            _machine.Configure(States.Approved)
                .OnEntryFrom(ParameterizedTriggers.Approve, OnApprove);
        }


        public void Fire(Guid workflowId, StateMachine<States, Triggers>.TriggerWithParameters<string, long?> trigger,
            string description, long? assignedUser)
        {
            _machine.Fire(trigger, description, assignedUser);
            _workflowService.UpdateWorkflow(workflowId, (int)_machine.State, description);
        }

        public void Fire(Guid workflowId,
            StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> trigger, FarmRequest data,
            string description, long? assignedUser)
        {
            _machine.Fire(trigger, data, description, assignedUser);
            _workflowService.UpdateWorkflow(workflowId, (int)_machine.State, description);
        }


        private void OnSave(FarmRequest data, string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.FarmClerk, data, description, assignedUser, transition);
        }

        private void OnReject(string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.FarmClerk, GetData(), description, assignedUser, transition);
        }

        private void OnCancel(string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(null, GetData(), description, assignedUser, transition);
        }

        private void OnRequest(FarmRequest data, string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.FarmSupervisor, data, description, assignedUser, transition);
        }

        private void OnApprove(string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            var data = GetData();
    
            // IMPORTANT: Load files from the last work item before creating entities
            var lastWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
            if (lastWorkItem != null && data != null)
            {
                LoadFilesFromWorkItem(lastWorkItem.Id, data);
            }
    
            // Create the actual entities (this will save files to the Documents table)
            if (data.OperatorId == null) 
                data.OperatorId = _service.CreateFarmOperator(data.Operator).Id.ToString();
            data.ActivityId = _service.CreateActivity(data.ActivityPlan).Id.ToString();
            var farm = _service.CreateFarm(data);
            data.Id = farm.Id.ToString();

            ConfigureAndAddWorkItem(UserRoles.LandAdmin, data, description, assignedUser, transition);
    
            _landAssignmentWorkflow.ConfigureMachine();
            _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id,
                LandAssignmentWorkflow.ParameterizedTriggers.Start, data, description, assignedUser);
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
                                "/usr/bin/CAMIS/data/docs";
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

            // Create work item
            _workflowService.CreateWorkItem(new WorkItemRequest
            {
                Id = workItemId,
                WorkflowId = Workflow.Id.ToString(),
                FromState = (int)transition.Source,
                ToState = (int)transition.Destination,
                Trigger = (int)transition.Trigger,
                DataType = typeof(FarmRequest).ToString(),
                Data = data != null ? JsonConvert.SerializeObject(data) : null,
                Description = description,
                AssignedRole = role,
                AssignedUser = assignedUser
            });
        }


        public static class ParameterizedTriggers
        {
            public static StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> Save;
            public static StateMachine<States, Triggers>.TriggerWithParameters<string, long?> Cancel;
            public static StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> Request;
            public static StateMachine<States, Triggers>.TriggerWithParameters<string, long?> Reject;
            public static StateMachine<States, Triggers>.TriggerWithParameters<string, long?> Approve;

            public static void ConfigureParameters(StateMachine<States, Triggers> machine)
            {
                Save = machine.SetTriggerParameters<FarmRequest, string, long?>(Triggers.Save);
                Cancel = machine.SetTriggerParameters<string, long?>(Triggers.Cancel);
                Request = machine.SetTriggerParameters<FarmRequest, string, long?>(Triggers.Request);
                Reject = machine.SetTriggerParameters<string, long?>(Triggers.Reject);
                Approve = machine.SetTriggerParameters<string, long?>(Triggers.Approve);
            }
        }

        private void LoadFilesFromWorkItem(Guid workItemId, FarmRequest data)
        {
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var workItemPath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory, workItemId.ToString());

            if (!Directory.Exists(workItemPath)) return;

            // Load photo if exists
            if (data?.Operator?.Photo?.Id != null)
            {
                var photoPath = Path.Combine(workItemPath, $"{data.Operator.Photo.Id}");
                if (File.Exists(photoPath))
                {
                    data.Operator.Photo.File = Convert.ToBase64String(File.ReadAllBytes(photoPath));
                }
            }

            // Load registration documents
            if (data?.Registrations != null)
            {
                foreach (var reg in data.Registrations)
                {
                    if (reg.Document?.Id != null)
                    {
                        var docPath = Path.Combine(workItemPath, $"{reg.Document.Id}");
                        if (File.Exists(docPath))
                        {
                            reg.Document.File = Convert.ToBase64String(File.ReadAllBytes(docPath));
                        }
                    }
                }
            }

            // Load operator registration documents
            if (data?.Operator?.Registrations != null)
            {
                foreach (var reg in data.Operator.Registrations)
                {
                    if (reg.Document?.Id != null)
                    {
                        var docPath = Path.Combine(workItemPath, $"{reg.Document.Id}");
                        if (File.Exists(docPath))
                        {
                            reg.Document.File = Convert.ToBase64String(File.ReadAllBytes(docPath));
                        }
                    }
                }
            }

            // Load activity plan documents
            if (data?.ActivityPlan?.Documents != null)
            {
                foreach (var doc in data.ActivityPlan.Documents)
                {
                    if (doc?.Id != null)
                    {
                        var docPath = Path.Combine(workItemPath, $"{doc.Id}");
                        if (File.Exists(docPath))
                        {
                            doc.File = Convert.ToBase64String(File.ReadAllBytes(docPath));
                        }
                    }
                }
            }
        }
    }
}