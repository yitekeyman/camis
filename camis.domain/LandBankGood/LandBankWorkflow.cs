using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace intapscamis.camis.domain.LandBank
{
    public class LandBankWorkflow: StatlessWorkFlowServiceBase<LandBankWorkflow.Triggers,LandBankWorkflow.States>
    {
        public enum States
        {
            Initial = 0,
            Started=1,
            Executed= -2,
            Canceled = -3,
            ApprovalRequested=2,
        }

        public enum Triggers
        {
            Start= 1,
            RequestAproval = 2,
            Execute,
            Approve = 3,
            Reject = 4,
            Cancel = 5,
        }

        
        private readonly ILandBankService _landBankService;

        
        private UserSession _session;
        Workflow Workflow { get; set; }
        public LandBankWorkflow(
            ILandBankService landBankService)
        {
            _workflowService = new WorkflowService();
            _landBankService = landBankService;

        }
        void ConfigureMachine(Guid workflowId)
        {
            Workflow = Context.Workflow.First(wf =>
                wf.Id == workflowId && wf.TypeId == (int)WorkflowTypes.LandRegistration);
            _machine = new StateMachine<States, Triggers>((States)Workflow.CurrentState);

            DefineStateMachine();
        }

        internal List<LandBankFacadeModel.LandBankWorkItem> getUserWorkItems()
        {
            var ws=_workflowService.GetCurrentWorkItemsForUser(_session.Username,new long[] { _session.Role});
            var ret = new List<LandBankFacadeModel.LandBankWorkItem>();
            var user = new UserActionService(Context);
            
            foreach(var w in ws)
            {
                var a=user.GetUserAction(w.WorkItem.Aid);
                ret.Add(new LandBankFacadeModel.LandBankWorkItem()
                {
                    wfid = w.Id.ToString(),
                    wiid =w.WorkItem.Id.ToString(),
                    description=w.WorkItem.Description,
                    sentUserName=a.Username,
                    workItemDate=new DateTime(a.Timestamp.Value),
                    workFlowType=w.TypeId,
                    workItemNote=w.WorkItem.Description,
                });
            }
            return ret;
        }
        public LandBankFacadeModel.LandData GetWorkFlowLand(Guid wfid)
        {

            //var wi=_workflowService.GetLastWorkItem(wfid);
            var wi = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandData>(wfid);
            if (!typeof(LandBankFacadeModel.LandData).ToString().Equals(wi.DataType))
                return null;
            var land = (LandBankFacadeModel.LandData)wi.Data;
            _landBankService.CalculateCentroid(land);
            return land;
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
            _machine.Configure(States.ApprovalRequested)
                .Permit(Triggers.Approve,States.Executed)
                .Permit(Triggers.Reject, States.Started)
                .Permit(Triggers.Cancel, States.Canceled);

            _machine.Configure(States.Started)
               .Permit(Triggers.RequestAproval, States.ApprovalRequested);

        }
        internal Guid RequestLandRegistration(LandBankFacadeModel.LandData data,String wfid)
        {
            int prevState;
            Workflow wf;
            if (String.IsNullOrEmpty(wfid))
            {
                wf = _workflowService.CreateWorkflow(new Workflows.Models.WorkflowRequest()
                {
                    CurrentState = (int)States.ApprovalRequested,
                    Description = String.IsNullOrEmpty(data.LandID)?"Initial land registration":"Update land registration request",
                    TypeId = String.IsNullOrEmpty(data.LandID)?(int)WorkflowTypes.LandRegistration: (int)WorkflowTypes.LandUpdate,
                });
                prevState = (int)States.Initial;
            }
            else
            {
                ConfigureMachine(Guid.Parse(wfid));
                wf = Workflow;
                prevState = wf.CurrentState;
            }
            if (data.Upins==null || data.Upins.Count==0)
            {
                throw new InvalidOperationException("At lease one UPIN must be provided");
            }
            data.parcels = new Dictionary<string, NrlaisInterfaceModel.Parcel>();
            double a = 0;
            foreach(String u in data.Upins)
            {
                var l = _landBankService.GetLandByUpin(u);
                if (l != null && !l.LandID.Equals(data.LandID))
                    throw new InvalidOperationException("UPIN " + u + " already used");
                try
                {
                    var p = _landBankService.getParcel(u);
                    if (p == null)
                        throw new InvalidOperationException($"No parcel found with upin{u}");
                    a += p.areaGeom;
                    
                    data.parcels.Add(u, p);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to retreive parcel information from nrlais for upin:{u}\n{ex.Message}");
                }
            }
            data.Area = a;

            _workflowService.CreateWorkItemChangeState(new Workflows.Models.WorkItemRequest()
            {
                WorkflowId = wf.Id.ToString(),
                FromState = prevState,
                ToState=(int)States.ApprovalRequested,
                Trigger = (int)Triggers.Approve,
                DataType = typeof(LandBankFacadeModel.LandData).ToString(),
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(data),
                Description = wf.Description,
                AssignedRole = (int)intapscamis.camis.domain.Admin.UserRoles.LandSupervisor
            });
            return wf.Id;
        }
        public Guid RejectLandRegistration(Guid workFlowID,String note)
        {
            ConfigureMachine(workFlowID);
            return fireAction(workFlowID, Triggers.Reject, note, intapscamis.camis.domain.Admin.UserRoles.LandClerk).Id;
        }
        public Guid CancelLandRegistration(Guid workFlowID, String note)
        {
            ConfigureMachine(workFlowID);
            return fireAction(workFlowID, Triggers.Cancel, note, null).Id;
        }

        public Guid ApproveLandRegistration(Guid workFlowID,String note)
        {
            ConfigureMachine(workFlowID);
            var data = GetWorkFlowLand(workFlowID);
            if (data == null)
                throw new InvalidOperationException($"Workflow id {workFlowID} doesn't have associated land data");
            var prevState = _machine.State;
            _machine.Fire(Triggers.Approve);
            var wi=_workflowService.CreateWorkItemChangeState(new WorkItemRequest
            {
                AssignedRole=null,
                Data=null,
                DataType=null,
                Description=note,
                AssignedUser=null,
                FromState=(int)prevState,
                ToState= (int)_machine.State,
                Trigger=(int)Triggers.Approve,
                WorkflowId=workFlowID.ToString(),
            });
            data.WID = workFlowID.ToString();
            data.LandType = (int)LandBankFacadeModel.LandTypeEnum.Prepared; // todo: SLI-temp
            if(String.IsNullOrEmpty(data.LandID))
                return _landBankService.RegisterLand(data);
            _landBankService.Updateland(data);
            return Guid.Parse(data.LandID);
        }

        internal WorkItemResponse GetLastWorkItem<T>(Guid guid)
        {
            return _workflowService.GetLastWorkItem<T>(guid);
        }
    }
}
