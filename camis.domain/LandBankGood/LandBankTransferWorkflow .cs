using camis.types.Utils;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Stateless;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace intapscamis.camis.domain.LandBank
{
    public class LandBankTransferWorkflow : StatlessWorkFlowServiceBase<LandBankTransferWorkflow.Triggers, LandBankTransferWorkflow.States>
    {


        public enum States
        {
            Initial = 0,
            WaitingForNrlais = 1,
            Executed = -2,
            Canceled = -3,
        }

        public enum Triggers
        {
            Start = 1,
            WaitForNrlais = 2,
            Execute,
            Cancel = 5,
        }

        private readonly ILandBankService _landBankService;

        private UserSession _session;
        Workflow Workflow { get; set; }
        public LandBankTransferWorkflow(
            ILandBankService landBankService)
        {
            _workflowService = new WorkflowService();
            _landBankService = landBankService;

        }
        void ConfigureMachine(Guid workflowId)
        {
            if (workflowId == Guid.Empty)
            {
                _machine = new StateMachine<States, Triggers>(States.Initial);
            }
            else
            {
                Workflow = Context.Workflow.First(wf =>
                    wf.Id == workflowId && wf.TypeId == (int)WorkflowTypes.TransferLand);
                _machine = new StateMachine<States, Triggers>((States)Workflow.CurrentState);
            }

            DefineStateMachine();
        }

      
        public void SetSession(UserSession session)
        {
            _session = session;
            _workflowService.SetSession(session);
            _landBankService.SetSession(session);
        }

        public override void SetContext(CamisContext context)
        {
            base.SetContext(context);
            _landBankService.SetContext(context);
            _workflowService.SetContext(context);
        }

        void DefineStateMachine()
        {
            _machine.Configure(States.Initial)
                .Permit(Triggers.Execute, States.Executed)
                .Permit(Triggers.WaitForNrlais, States.WaitingForNrlais)
                .Permit(Triggers.Cancel, States.Canceled);

            _machine.Configure(States.WaitingForNrlais)
                .Permit(Triggers.Execute, States.Executed)
                .Permit(Triggers.Cancel, States.Canceled);

        }
        void ExecuteLandTransfer(LandBankFacadeModel.TransferRequest request)
        {

            _landBankService.TransferLand(request);
        }

        internal int GetTransferStatus(Guid wfid)
        {
            ConfigureMachine(wfid);
            if (_machine.State == States.WaitingForNrlais)
            {
                var w = _workflowService.GetLastWorkItem<LandBankFacadeModel.TransferRequest>(wfid);
                var txuid = ((LandBankFacadeModel.TransferRequest)w.Data).txuid;
                WorkItem wi;
                switch (new RestNrlaisInterface().GetApplicationStatus(txuid))
                {
                    case NrlaisInterfaceModel.NrlaisApplicationStatus.Canceled:
                        wi = this.fireAction(wfid, Triggers.Cancel, "Rejected by nrlais", null);
                        return (int)wi.ToState;
                    case NrlaisInterfaceModel.NrlaisApplicationStatus.Completd:
                        var request = (LandBankFacadeModel.TransferRequest)_workflowService.GetLastWorkItem<LandBankFacadeModel.TransferRequest>(wfid).Data;
                        wi = this.fireAction(wfid, Triggers.Execute, "Accepted by nrlais", null);
                        ExecuteLandTransfer(request);
                        return (int)wi.ToState;
                    case NrlaisInterfaceModel.NrlaisApplicationStatus.Processing:
                        return (int)_machine.State;
                    default:
                        throw new InvalidOperationException("Invalid status returned by nrlais");
                }
            }
            else
                return (int)_machine.State;
        }
        internal Guid RequestLandTransfer(LandBankFacadeModel.TransferRequest request)
        {
            return RequestLandTransfer(request, null);
        }

        // todo: TTT review this algorithm, with how it works for request.right == 5
        internal Guid RequestLandTransfer(LandBankFacadeModel.TransferRequest request, String wfid)
        {
            var l = _landBankService.GetLand(request.landID,false,false);
            CamisUtils.Assert(l != null, "Land not found in land bank with id " + request.landID);
            CamisUtils.Assert(l.LandType == (int)LandBankFacadeModel.LandTypeEnum.Prepared, "The land is not in prepared status, hence it can't be transfered");
            var stateholderType = l.parcels[l.Upins[0]].GetHolder().partyType == NrlaisInterfaceModel.Party.PARTY_TYPE_STATE;
            switch (request.right)
            {
                case LandBankFacadeModel.LandRightType.LeaseFromState:
                    CamisUtils.Assert(stateholderType, "Only state land can be leased");
                    break;
                case LandBankFacadeModel.LandRightType.RentFromPrivate:
                case LandBankFacadeModel.LandRightType.Private:
                    CamisUtils.Assert(!stateholderType, "Only private land can be used for this kind of transfer");
                    break;
            }
            request.landID = Guid.Parse(l.LandID);

            int prevState;
            Workflow wf;
            if (String.IsNullOrEmpty(wfid))
            {
                ConfigureMachine(Guid.Empty);
                wf = _workflowService.CreateWorkflow(new Workflows.Models.WorkflowRequest()
                {
                    CurrentState = (int)States.Initial,
                    Description = "Initial land transfer request",
                    TypeId = (int)WorkflowTypes.TransferLand,
                });
                prevState = (int)States.Initial;
            }
            else
            {
                ConfigureMachine(Guid.Parse(wfid));
                wf = Workflow;
                prevState = wf.CurrentState;
            }

            if (request.right == LandBankFacadeModel.LandRightType.Private
                    || request.right == LandBankFacadeModel.LandRightType.ContractFarming)
            {
                ExecuteLandTransfer(request);
                fireAction(wf.Id, Triggers.Execute, "Complete",null);
            }
            else
            {
                request.txuid = new RestNrlaisInterface().RequestLandTransfer(l.parcels[l.Upins[0]], request);
                fireAction(wf.Id, Triggers.WaitForNrlais, "Waiting for nrlais", Admin.UserRoles.LandSupervisor, request);
            }
            return wf.Id;
        }
    }
}
