using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
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
using Microsoft.EntityFrameworkCore;
using camis.types.Utils;

namespace intapscamis.camis.domain.LandBank
{
    public class LandBankPrepareWorkflow : 
        StatlessWorkFlowServiceBase<LandBankPrepareWorkflow.Triggers, LandBankPrepareWorkflow.States>
    {
    

        public enum States
        {
            Initial = 0,
            Started=1,
            Executed= -2,
            Canceled = -3,
            ApprovalRequested=2,
            ApprovalWithSplitRequested = 3,
            WaitingForNrlais = 4,
            NrlaisRejected= 5,
        }

        public enum Triggers
        {
            Start= 1,
            RequestAproval = 2,
            RequestAprovalWithSplit = 3,
            Execute=4,
            Approve = 5,
            Reject = 6,
            Cancel = 7,
            WaitForNrlais=8,
            NrlaisReject=9,
            NrlaisApprove=10,
            SetSplitGeom,
        }
        private readonly ILandBankService _landBankService;

        private UserSession _session;
        Workflow Workflow { get; set; }
        public LandBankPrepareWorkflow(
            ILandBankService landBankService)
        {
            _workflowService = new WorkflowService();
            _landBankService = landBankService;

        }
        void ConfigureMachine(Guid workflowId)
        {
            Workflow = Context.Workflow.First(wf =>
                wf.Id == workflowId && wf.TypeId == (int)WorkflowTypes.PrepareLand);
            _machine = new StateMachine<States, Triggers>((States)Workflow.CurrentState);

            DefineStateMachine();
        }

       
        WorkItem NrlaisAccept(Guid txuid)
        {
            var w = _workflowService.GetLastWorkItem<LandBankFacadeModel.SetPrepareGeometries>(this.Workflow.Id);
            CamisUtils.Assert(w != null && w.Data is LandBankFacadeModel.SetPrepareGeometries, "Couldn't retreive split data");
            var d = (LandBankFacadeModel.SetPrepareGeometries)w.Data;
            var p=_workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(this.Workflow.Id).Data  as LandBankFacadeModel.LandPreparationRequest;

            var l = _landBankService.GetLand(p.landID,false,false);
            var ps=new RestNrlaisInterface().GetParcelsByTranscation(txuid);
            CamisUtils.Assert(ps.Count == d.geoms.Count, "Nrlais returned unexpected number of parcels for split transaction");

            _landBankService.RemoveLand(p.landID);
            var oldUpin = l.Upins[0];

            var newwi = fireAction(this.Workflow.Id, Triggers.NrlaisApprove, "Approved from NRLAIS", null);

            foreach(var parcel in ps)
            {
                l.Description = "Land split from " + oldUpin;
                l.Upins[0] = parcel.upid;
                l.parcels = new Dictionary<string, NrlaisInterfaceModel.Parcel>();
                l.parcels.Add(l.Upins[0], parcel);
                l.Area = parcel.areaGeom;
                l.LandID = null;
                l.LandType = (int)LandBankFacadeModel.LandTypeEnum.Prepared;
                l.WID = newwi.Id.ToString();
                _landBankService.RegisterLand(l);
            }
            return newwi;
        }
        internal int GetPreparationStatus(Guid wfid)
        {
            ConfigureMachine(wfid);
            if (_machine.State == States.WaitingForNrlais)
            {
                var w = _workflowService.GetLastWorkItem<String>(wfid);
                var txuid = w.Data.ToString();
                WorkItem wi;
                switch (new RestNrlaisInterface().GetApplicationStatus(Guid.Parse(txuid)))
                {
                    case NrlaisInterfaceModel.NrlaisApplicationStatus.Canceled:
                        wi = this.fireAction(wfid, Triggers.NrlaisReject, "Rejected by nrlais", null);
                        return (int)wi.ToState;
                    case NrlaisInterfaceModel.NrlaisApplicationStatus.Completd:
                        return (int)NrlaisAccept(Guid.Parse(txuid)).ToState;
                    case NrlaisInterfaceModel.NrlaisApplicationStatus.Processing:
                        return (int)_machine.State;
                    default:
                        throw new InvalidOperationException("Invalid status returned by nrlais");
                }
            }
            else
                return (int)_machine.State;
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

        internal LandBankFacadeModel.SplitTaskList GetSplitList()
        {
            var ret = new LandBankFacadeModel.SplitTaskList();
            ret.tasks = new List<LandBankFacadeModel.SplitTaskItem>();
            var ws = _workflowService.GetWorkflows((int)WorkflowTypes.PrepareLand, (int)States.Started); 
            foreach(var w in ws)
            {
                var wi = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(w.Id);
                var data = wi.Data as LandBankFacadeModel.LandPreparationRequest;
                if(data.n>0)
                {
                    ret.tasks.Add(new LandBankFacadeModel.SplitTaskItem()
                    {
                        id=w.Id.ToString(),
                        description="Split land",
                        n=data.n,
                        upin=_landBankService.GetLand(data.landID,false,false).Upins[0],
                    });
                }
            }
            return ret;
        }

        internal Guid CancelRequest(Guid workFlowID, string note)
        {
            ConfigureMachine(workFlowID);
            return fireAction(workFlowID, Triggers.Cancel, note, null).Id;

        }
        internal Guid RejectRequest(Guid workFlowID, string note)
        {
            ConfigureMachine(workFlowID);
            return fireAction(workFlowID, Triggers.Reject, note, null).Id;

        }
        internal LandBankFacadeModel.SplitTaskGeom GetTaskGeom(Guid wfid)
        {
            var w = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
            var r = (LandBankFacadeModel.LandPreparationRequest)w.Data;
            Context.Database.OpenConnection();
            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                var sql = $"Select ST_AsText(geometry) from lb.land_upin where land_id='{r.landID}'";
                command.CommandText = sql;
                var res=command.ExecuteScalar();
                var ret = new LandBankFacadeModel.SplitTaskGeom()
                {
                    area = 0,
                    id = 1,
                    geom = res.ToString(),
                    label="split"
                };
                return ret;

            }

            return null;
        }

        internal LandBankFacadeModel.SplitData GetSplitData(Guid wfid)
        {
            var w = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
            var r = (LandBankFacadeModel.LandPreparationRequest)w.Data;
            var p = _landBankService.GetLand(r.landID,false,false);

            return new LandBankFacadeModel.SplitData()
            {
                n=r.n,
                area=p.parcels[p.Upins[0]].areaGeom,
                upin=p.Upins[0],
            };
        }

        void DefineStateMachine()
        {
            _machine.Configure(States.Started)
               .Permit(Triggers.RequestAproval, States.ApprovalRequested)
               .Permit(Triggers.RequestAprovalWithSplit, States.ApprovalWithSplitRequested)
               .Permit(Triggers.Cancel,States.Canceled);
           

            _machine.Configure(States.ApprovalRequested)
                .Permit(Triggers.Approve,States.Executed)
                .Permit(Triggers.Reject, States.Started)
                .Permit(Triggers.Cancel, States.Canceled);

            _machine.Configure(States.ApprovalWithSplitRequested)
                .Permit(Triggers.WaitForNrlais, States.WaitingForNrlais)
                .Permit(Triggers.Reject, States.Started)
                .Permit(Triggers.Cancel, States.Canceled);

            _machine.Configure(States.WaitingForNrlais)
                .Permit(Triggers.NrlaisApprove, States.Executed)
                .Permit(Triggers.NrlaisReject, States.NrlaisRejected);

            _machine.Configure(States.NrlaisRejected)
                .Permit(Triggers.Reject, States.Started)
                .Permit(Triggers.Cancel, States.Canceled);
        }
        internal Guid RequestLandPreparation(LandBankFacadeModel.LandPreparationRequest request,String wfid)
        {
            int prevState;
            Workflow wf;
            if (String.IsNullOrEmpty(wfid))
            {
                wf = _workflowService.CreateWorkflow(new Workflows.Models.WorkflowRequest()
                {
                    CurrentState = (int)States.ApprovalRequested,
                    Description = "Iniital land prepare request",
                    TypeId = (int)WorkflowTypes.PrepareLand,
                });
                prevState = (int)States.Initial;
            }
            else
            {
                ConfigureMachine(Guid.Parse(wfid));
                wf = Workflow;
                prevState = wf.CurrentState;
            }

            var l = _landBankService.GetLand(request.landID,false,false);
            CamisUtils.Assert(request.n>0, $"Split size should be at least 1");
            CamisUtils.Assert(l != null, $"{request.landID} is land id");
            CamisUtils.Assert(l.LandType != (int)LandBankFacadeModel.LandTypeEnum.Transfered, $"Land preparation request is allowed only for land that is transfered");
            CamisUtils.Assert(l.Upins.Count == 1, $"Land {request.landID} doesn't have unique UPIN");
            var p = l.parcels[l.Upins[0]];
            CamisUtils.Assert(p.IsStateLand(), $"Only state land can be prepared.");
            CamisUtils.Assert(p!= null, $"Land {request.landID} doesn't have associated land propfile");
            States nexState;
            Triggers triger;
            int role;
            if (request.n > 1)
            {
                nexState = States.Started;
                triger = Triggers.Start;
                role = UserRoles.LandClerk;
            }
            else
            {
                nexState = States.ApprovalRequested;
                triger = Triggers.RequestAproval;
                role = UserRoles.LandSupervisor;
            }
            _workflowService.CreateWorkItemChangeState(new Workflows.Models.WorkItemRequest()
            {
                WorkflowId = wf.Id.ToString(),
                FromState = prevState,
                ToState=(int)nexState,
                Trigger = (int)triger,
                DataType = typeof(LandBankFacadeModel.LandPreparationRequest).ToString(),
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(request),
                Description = "Land prepartion request",
                AssignedRole = role
            });
            return wf.Id;
        }
        LandBankFacadeModel.LandPreparationRequest GetPreparationRequest(Guid wfid)
        {
            var request = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
            if (request == null)
                return null;
            return (LandBankFacadeModel.LandPreparationRequest)request.Data;

        }
        public Guid SetSplitGeometries(Guid wfid, LandBankFacadeModel.SetPrepareGeometries par)
        {
            ConfigureMachine(wfid);
            var data = GetPreparationRequest(wfid);
            CamisUtils.Assert(data!= null , "Can't find the associated land preparation request");
            CamisUtils.Assert(data.n == par.geoms.Count, "Number of geometries must be consistent with the prepration request");
            
            var prevState = _machine.State;
            _machine.Fire(Triggers.RequestAprovalWithSplit);
            return _workflowService.CreateWorkItemChangeState(new WorkItemRequest()
            {
                WorkflowId=wfid.ToString(),
                AssignedRole=(long)UserRoles.LandSupervisor,
                Data=Newtonsoft.Json.JsonConvert.SerializeObject(par),
                DataType=typeof(LandBankFacadeModel.SetPrepareGeometries).ToString(),
                AssignedUser=null,
                Description="Assign geometry to split land",
                ToState=(int)States.ApprovalWithSplitRequested,
                FromState=(int)prevState,
                Trigger=(int)Triggers.RequestAprovalWithSplit
            }).Id;
        }
        public Guid ApprovePreparation(Guid wfid,String note)
        {
            ConfigureMachine(wfid);
            var data = GetPreparationRequest(wfid);
            CamisUtils.Assert(data != null, "Can't find the associated land preparation request");

            if (_machine.State==States.ApprovalRequested)
            {
                _landBankService.SetLandState(data.landID, LandBankFacadeModel.LandTypeEnum.Prepared);
                return fireAction(wfid,Triggers.Approve,note,null).Id;
            }
            else
            {
                var split = _workflowService.GetLastWorkItem<LandBankFacadeModel.SetPrepareGeometries>(wfid);
                CamisUtils.Assert(split != null, "Failed to find split geometries data in workflow");
                var splitData = (LandBankFacadeModel.SetPrepareGeometries)split.Data;
                var p = _landBankService.GetLand(data.landID,false,false);
                CamisUtils.Assert(p != null && p.Upins.Count==1, "Failed to find valid parcel information");
                var parcel = p.parcels[p.Upins[0]];
                var tranID=new RestNrlaisInterface().RequestLandSplit(parcel, splitData.geoms);
                return fireAction(wfid, Triggers.WaitForNrlais, note,UserRoles.LandSupervisor, tranID.ToString()).Id;
            }
        }
        

        
        
    }
}
