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
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Farms.StateMachines;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.Internal;

namespace intapscamis.camis.domain.LandBank
{
    public class LandBankTransferWorkflow : StatlessWorkFlowServiceBase<LandBankTransferWorkflow.Triggers,
        LandBankTransferWorkflow.States>
    {
        public enum States
        {
            Initial = 0,
            WaitingForNrlais = 1,
            RequestSent = 3,
            Rejected = 6,
            Executed = -2,
            Canceled = -3,
        }

        public enum Triggers
        {
            Start = 1,
            WaitForNrlais = 2,
            SentRequest = 3,
            Reject = 4,
            Execute,
            Cancel = 5,
        }

        private readonly ILandBankService _landBankService;

        private UserSession _session;

        private string regionId;

        //private readonly LandAssignmentWorkflow _landAssignmentWorkflow;
        Workflow Workflow { get; set; }

        public LandBankTransferWorkflow(
            ILandBankService landBankService)
        {
            _workflowService = new WorkflowService();
            _landBankService = landBankService;
        }

        public void ConfigureMachine(Guid workflowId)
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
            // _landAssignmentWorkflow.SetContext(Context);
        }

        public override void SetContext(CamisContext context)
        {
            base.SetContext(context);
            _landBankService.SetContext(context);
            _workflowService.SetContext(context);
        }

        private void SetRegionId()
        {
            regionId ??= Context.SysConfigs.First(e => e.Name.Equals("region_code")).Value;
        }

        void DefineStateMachine()
        {
            _machine.Configure(States.Initial)
                .Permit(Triggers.Execute, States.Executed)
                .Permit(Triggers.WaitForNrlais, States.WaitingForNrlais)
                .Permit(Triggers.Cancel, States.Canceled)
                .Permit(Triggers.SentRequest, States.RequestSent);

            _machine.Configure(States.RequestSent)
                .Permit(Triggers.Reject, States.Rejected)
                .Permit(Triggers.Execute, States.Executed);

            _machine.Configure(States.Rejected)
                .Permit(Triggers.SentRequest, States.RequestSent)
                .Permit(Triggers.Cancel, States.Canceled);

            _machine.Configure(States.WaitingForNrlais)
                .Permit(Triggers.Execute, States.Executed)
                .Permit(Triggers.Cancel, States.Canceled);
        }

        void ExecuteLandTransfer(LandBankFacadeModel.TransferRequest request)
        {
            _landBankService.TransferLand(request);
        }

        public Guid RejectLandTransfer(Guid wfid, string note)
        {
            ConfigureMachine(wfid);
            var data = GetPreparationRequest(wfid);
            return fireAction(wfid, Triggers.Reject, note, Admin.UserRoles.LandAdmin, data).Id;
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
                        var request = (LandBankFacadeModel.TransferRequest)_workflowService
                            .GetLastWorkItem<LandBankFacadeModel.TransferRequest>(wfid).Data;
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

        internal Guid RequestLandTransfer(FarmRequest request, Guid wfid, string note)
        {
            var id = "";
            if (wfid != Guid.Empty)
                id = wfid.ToString();

            return RequestLandTransfer(request, id, note);
        }

        // todo: TTT review this algorithm, with how it works for request.right == 5
        internal Guid RequestLandTransfer(FarmRequest request, String wfid, string note)
        {
            var l = _landBankService.GetLand(request.LandTransferRequest.landID, false, false);
            CamisUtils.Assert(l != null, "Land not found in land bank with id " + request.LandTransferRequest.landID);
            CamisUtils.Assert(
                (l.LandType == (int)LandBankFacadeModel.LandTypeEnum.Prepared ||
                 l.LandType == (int)LandBankFacadeModel.LandTypeEnum.PreparedWithSplit ||
                 l.LandType == (int)LandBankFacadeModel.LandTypeEnum.HalfTransferred),
                "The land status can't be allowed for transfered");

            var stateholderType = l.parcels[l.Upins[0]].GetHolder().partyType ==
                                  NrlaisInterfaceModel.Party.PARTY_TYPE_STATE;
            switch (request.LandTransferRequest.right)
            {
                case LandBankFacadeModel.LandRightType.LeaseFromState:
                    CamisUtils.Assert(stateholderType, "Only state land can be leased");
                    break;
                case LandBankFacadeModel.LandRightType.RentFromPrivate:
                case LandBankFacadeModel.LandRightType.Private:
                    CamisUtils.Assert(!stateholderType, "Only private land can be used for this kind of transfer");
                    break;
            }

            request.LandTransferRequest.landID = Guid.Parse(l.LandID);

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

            SetRegionId();

            if (request.LandTransferRequest.landPart > 0)
            {
                _landBankService.SetSubLandLock(request.LandTransferRequest.landID,
                    request.LandTransferRequest.landPart ?? 0, true);
            }
            else
            {
                _landBankService.SetLandLock(request.LandTransferRequest.landID, true);
            }

            fireAction(wf.Id, Triggers.SentRequest, note ?? "Land Transfer Request Sent to Farm Supervisor",
                Admin.UserRoles.FarmSupervisor, request);

            // if (request.LandTransferRequest.right == LandBankFacadeModel.LandRightType.Private
            //     || request.LandTransferRequest.right == LandBankFacadeModel.LandRightType.ContractFarming)
            // {
            //     ExecuteLandTransfer(request.LandTransferRequest);
            //     fireAction(wf.Id, Triggers.Execute, "Complete", null);
            // }
            // else
            // {
            //     request.LandTransferRequest.txuid = new RestNrlaisInterface().RequestLandTransfer(l.parcels[l.Upins[0]], request.LandTransferRequest);
            //     fireAction(wf.Id, Triggers.WaitForNrlais, "Waiting for nrlais", Admin.UserRoles.LandSupervisor,
            //         request);
            // }


            return wf.Id;
        }

        FarmRequest GetPreparationRequest(Guid wfid)
        {
            var request = _workflowService.GetLastWorkItem<FarmRequest>(wfid);
            if (request == null)
                return null;
            return (FarmRequest)request.Data;
        }

        internal Guid ApproveLandTransfer(Guid wfid, string note)
        {
            ConfigureMachine(wfid);
            var data = GetPreparationRequest(wfid);
            data.LandTransferRequest.farmId = data.Id;
            ExecuteLandTransfer(data.LandTransferRequest);
            var workflowId = this.fireAction(wfid, Triggers.Execute, note ?? "Accepted by Farm supervisor", null)
                .WorkflowId;
            // _landAssignmentWorkflow.ConfigureMachine();
            // _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id,
            //     LandAssignmentWorkflow.ParameterizedTriggers.Certify, data, note, Admin.UserRoles.FarmSupervisor);

            return workflowId;
        }

        internal Guid RerequestApproval(Guid wfid, FarmRequest request, string note)
        {
            ConfigureMachine(wfid);
            var data = GetPreparationRequest(wfid);
            if (request.LandTransferRequest.landID != data.LandTransferRequest.landID)
            {
                _landBankService.SetLandLock(data.LandTransferRequest.landID, false);
                var l = _landBankService.GetLand(request.LandTransferRequest.landID, false, false);
                CamisUtils.Assert(l != null,
                    "Land not found in land bank with id " + request.LandTransferRequest.landID);
                CamisUtils.Assert(
                    (l.LandType == (int)LandBankFacadeModel.LandTypeEnum.Prepared ||
                     l.LandType == (int)LandBankFacadeModel.LandTypeEnum.PreparedWithSplit ||
                     l.LandType == (int)LandBankFacadeModel.LandTypeEnum.HalfTransferred),
                    "The land status can't be allowed for transfered");

                var stateholderType = l.parcels[l.Upins[0]].GetHolder().partyType ==
                                      NrlaisInterfaceModel.Party.PARTY_TYPE_STATE;
                switch (request.LandTransferRequest.right)
                {
                    case LandBankFacadeModel.LandRightType.LeaseFromState:
                        CamisUtils.Assert(stateholderType, "Only state land can be leased");
                        break;
                    case LandBankFacadeModel.LandRightType.RentFromPrivate:
                    case LandBankFacadeModel.LandRightType.Private:
                        CamisUtils.Assert(!stateholderType, "Only private land can be used for this kind of transfer");
                        break;
                }

                request.LandTransferRequest.landID = Guid.Parse(l.LandID);
            }

            if (data.LandTransferRequest.landPart > 0)
                _landBankService.SetSubLandLock(data.LandTransferRequest.landID, data.LandTransferRequest.landPart ?? 0,
                    false);

            if (request.LandTransferRequest.landPart > 0)
            {
                _landBankService.SetSubLandLock(request.LandTransferRequest.landID,
                    request.LandTransferRequest.landPart ?? 0, true);
            }
            else
            {
                _landBankService.SetLandLock(request.LandTransferRequest.landID, true);
            }

            return fireAction(wfid, Triggers.SentRequest, note ?? "Farm Supervisor resent transfer request",
                Admin.UserRoles.FarmSupervisor, request).WorkflowId;
        }

        internal List<Workflow> GetTransferWorkFlows()
        {
            return Context.Workflow.Where(x => x.TypeId == 10 && x.CurrentState == 3).ToList();
        }
    }
}