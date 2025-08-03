using System;
using System.Collections.Generic;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Newtonsoft.Json;
using Stateless;

namespace intapscamis.camis.domain.Farms.StateMachines
{
    public class LandAssignmentWorkflow : CamisService
    {
        public enum States
        {
            Locating = 2,
            Waiting = 3,
            Certifying = 4,
            Leased = -2
        }

        public enum Triggers
        {
            Start = 8, // loop to Locating (with data)
            Wait = 5,
            Assign = 6,
            Certify = 7
        }

        private readonly IFarmsService _service;
        private readonly IWorkflowService _workflowService;
        private readonly LandBankTransferWorkflow _landBankTransferWorkflow;

        private StateMachine<States, Triggers> _machine;

        public LandAssignmentWorkflow(
            IFarmsService service,
            IWorkflowService workflowService,
            LandBankTransferWorkflow landBankTransferWorkflow)
        {
            _service = service;
            _workflowService = workflowService;
            _landBankTransferWorkflow = landBankTransferWorkflow;
        }


        public Workflow Workflow { get; private set; }


        public void SetSession(UserSession session)
        {
            _service.SetSession(session);
            _workflowService.SetSession(session);
            _landBankTransferWorkflow.SetSession(session);
        }

        public override void SetContext(CamisContext value)
        {
            base.SetContext(value);
            _service.SetContext(Context);
            _workflowService.SetContext(Context);
            _landBankTransferWorkflow.SetContext(Context);
        }


        // create new workflow
        public void ConfigureMachine()
        {
            Workflow = _workflowService.CreateWorkflow(new WorkflowRequest
            {
                CurrentState = (int) States.Locating,
                Description = "New land assignment.",
                TypeId = (int) WorkflowTypes.LandAssignment
            });
            _machine = new StateMachine<States, Triggers>(States.Locating);

            DefineStateMachine();
        }

        // access existing workflow
        public void ConfigureMachine(Guid workflowId)
        {
            Workflow = Context.Workflow.First(wf =>
                wf.Id == workflowId && wf.TypeId == (int) WorkflowTypes.LandAssignment);
            _machine = new StateMachine<States, Triggers>((States) Workflow.CurrentState);

            DefineStateMachine();
        }

        private void DefineStateMachine()
        {
            ParameterizedTriggers.ConfigureParameters(_machine);

            _machine.Configure(States.Locating)
                .PermitReentry(Triggers.Start)
                .OnEntryFrom(ParameterizedTriggers.Start, OnStart)
                .Permit(Triggers.Wait, States.Waiting);

            _machine.Configure(States.Waiting)
                .OnEntryFrom(ParameterizedTriggers.Wait, OnWait)
                .Permit(Triggers.Assign, States.Certifying);

            _machine.Configure(States.Certifying)
                .OnEntryFrom(ParameterizedTriggers.Assign, OnAssign)
                .Permit(Triggers.Certify, States.Leased);

            _machine.Configure(States.Leased)
                .OnEntryFrom(ParameterizedTriggers.Certify, OnCertify);
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


        private void OnStart(FarmRequest data, string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.LandAdmin, data, description, assignedUser, transition);
        }
        
        private void OnWait(FarmRequest data, string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            // the real act (part 2): assign land and wait for NRLAIS
            data.LandTransferWorkflowId = _landBankTransferWorkflow.RequestLandTransfer(data.LandTransferRequest);
            data.FarmLands = new List<FarmLandRequest>
            {
                new FarmLandRequest {FarmId = data.Id.ToGuid(), LandId = data.LandTransferRequest.landID}
            };

            ConfigureAndAddWorkItem(UserRoles.LandCertificateIssuer, data, description, assignedUser, transition);
        }

        public int GetTransferStatus()
        {
            var data = GetData();
            var status = _landBankTransferWorkflow.GetTransferStatus(data.LandTransferWorkflowId);

            if (Workflow.CurrentState == (int) States.Waiting && status == (int) LandBankTransferWorkflow.States.Executed)
            {
                Fire(Workflow.Id, ParameterizedTriggers.Assign, "Land assignment resolved in NRLAIS.", null);
                return -99;
            }

            return status;
        }

        private void OnAssign(string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.LandAdmin, GetData(), description, assignedUser, transition);
        }

        private void OnCertify(FarmRequest data, string description, long? assignedUser,
            StateMachine<States, Triggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(null, data, description, assignedUser, transition);
            
            // the real act (part 3): commit the land assignment and give certificate to the farm
            foreach (var farmLand in data.FarmLands) _service.AssignFarmLand(data.Id.ToGuid(), farmLand);
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
            public static StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> Start;
            public static StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> Wait;
            public static StateMachine<States, Triggers>.TriggerWithParameters<string, long?> Assign;
            public static StateMachine<States, Triggers>.TriggerWithParameters<FarmRequest, string, long?> Certify;

            public static void ConfigureParameters(StateMachine<States, Triggers> machine)
            {
                Start = machine.SetTriggerParameters<FarmRequest, string, long?>(Triggers.Start);
                Wait = machine.SetTriggerParameters<FarmRequest, string, long?>(Triggers.Wait);
                Assign = machine.SetTriggerParameters<string, long?>(Triggers.Assign);
                Certify = machine.SetTriggerParameters<FarmRequest, string, long?>(Triggers.Certify);
            }
        }
    }
}