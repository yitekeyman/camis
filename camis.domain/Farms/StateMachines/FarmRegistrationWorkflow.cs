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
                CurrentState = (int) States.Filing,
                Description = "New farm registration.",
                TypeId = (int) WorkflowTypes.FarmRegistration
            });
            _machine = new StateMachine<States, Triggers>(States.Filing);

            DefineStateMachine();
        }

        // access existing workflow
        public void ConfigureMachine(Guid workflowId)
        {
            Workflow = Context.Workflow.First(wf =>
                wf.Id == workflowId && wf.TypeId == (int) WorkflowTypes.FarmRegistration);
            _machine = new StateMachine<States, Triggers>((States) Workflow.CurrentState);

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
            _workflowService.UpdateWorkflow(workflowId, (int) _machine.State, description);
        }

        public void Fire(Guid workflowId,
            StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> trigger, FarmRequest data,
            string description, long? assignedUser)
        {
            _machine.Fire(trigger, data, description, assignedUser);
            _workflowService.UpdateWorkflow(workflowId, (int) _machine.State, description);
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

            // the real act (part 1): register farm
            if (data.OperatorId == null) data.OperatorId = _service.CreateFarmOperator(data.Operator).Id.ToString();
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
            
            // tag each (FarmRequest data).Registrations[i].Document 
            if (data?.Registrations != null)
            {
                var i = -1;
                
                foreach (var reg in data.Registrations)
                {
                    if (reg.Document == null) continue;

                    // temp Id used only in this workflow to access the document
                    reg.Id = i;
                    i--;
                    
                    const string pathPrefix = "/api/Farms/InWorkItemRegistrationFile/";

                    if (reg.Document.OverrideFilePath != null &&
                        reg.Document.OverrideFilePath.Substring(0, pathPrefix.Length) == pathPrefix)
                    {
                        var lastWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                        if (lastWorkItem != null)
                        {
                            var file = _service.InWorkItemRegistrationFile(lastWorkItem.Id, reg.Id).File;
                            if (file != null) reg.Document.File = Convert.ToBase64String(file);
                        }
                    }

                    reg.Document.Id = reg.Document.Id ?? Guid.NewGuid();
                    reg.Document.OverrideFilePath = $"{pathPrefix}{workItemId}?regId={reg.Id}";
                }
            }
            
            // tag each (FarmRequest data).Operator.Registrations[i].Document 
            if (data?.Operator?.Registrations != null)
            {
                var i = -1;

                foreach (var reg in data.Operator.Registrations)
                {
                    if (reg.Document == null) continue;

                    // temp Id used only in this workflow to access the document
                    reg.Id = i;
                    i--;

                    const string pathPrefix = "/api/Farms/InWorkItemOperatorRegistrationFile/";
                    
                    if (reg.Document.OverrideFilePath != null &&
                        reg.Document.OverrideFilePath.Substring(0, pathPrefix.Length) == pathPrefix)
                    {
                        var lastWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                        if (lastWorkItem != null)
                        {
                            var file = _service.InWorkItemOperatorRegistrationFile(lastWorkItem.Id, reg.Id).File;
                            if (file != null) reg.Document.File = Convert.ToBase64String(file);
                        }
                    }

                    reg.Document.Id = reg.Document.Id ?? Guid.NewGuid();
                    reg.Document.OverrideFilePath = $"{pathPrefix}{workItemId}?regId={reg.Id}";
                }
            }
            
            // tag each (FarmRequest data).ActivityPlan.Documents 
            if (data?.ActivityPlan?.Documents != null)
            {
                foreach (var doc in data.ActivityPlan.Documents)
                {
                    if (doc == null) continue;
                    
                    const string pathPrefix = "/api/Farms/InWorkItemActivityPlanFile/";

                    if (doc.OverrideFilePath != null &&
                        doc.OverrideFilePath.Substring(0, pathPrefix.Length) == pathPrefix)
                    {
                        var lastWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                        if (lastWorkItem != null)
                        {
                            var file = _service
                                .InWorkItemActivityPlanFile(lastWorkItem.Id, doc.Id ?? Guid.Empty)
                                .File;
                            if (file != null) doc.File = Convert.ToBase64String(file);
                        }
                    }
                    
                    doc.Id = doc.Id ?? Guid.NewGuid();
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
    }
}