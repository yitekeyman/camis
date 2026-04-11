using camis.types.Utils;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Farms.StateMachines;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Stateless;

namespace intapscamis.camis.domain.LandBank;

public class
    LandBankSplitWorkflow : StatlessWorkFlowServiceBase<LandBankSplitWorkflow.Triggers, LandBankSplitWorkflow.States>
{
    public enum States
    {
        Initial = 0,
        Started = 1,
        ParcelSplitRequested = 2,
        WaitingForNRLAIS = 3,
        WaitingForCMSS = 4,
        NRLAISApproved = 5,
        NRLAISRejected = 6,
        CMSSDoneSplit = 7,
        CMSSRejected = 8,
        Executed = 9,
        Approved = 10,
        Rejected = 11,
        Cancelled = 12,
    }

    public enum Triggers
    {
        Start = 1,
        RequestSplitting = 2,
        WaitForNRLAIS = 3,
        WaitForCMSS = 4,
        NRLAISApprov = 5,
        NRLAISReject = 6,
        CMSSParcelSplitted = 7,
        CMSSReject = 8,
        Execute = 9,
        Approve = 10,
        Reject = 11,
        Cancel = 12,
        SetSplitGeom,
    }

    private readonly ILandBankService _landBankService;
    private UserSession _session;
    private string regionId;
    Workflow Workflow { get; set; }

    public LandBankSplitWorkflow(ILandBankService landBankService)
    {
        _workflowService = new WorkflowService();
        _landBankService = landBankService;
    }
    
    public void ConfigureMachine(Guid workflowId)
    {
        Workflow = Context.Workflow.First(wf =>
            wf.Id == workflowId && wf.TypeId == (int)WorkflowTypes.PrepareLand);
        _machine = new StateMachine<LandBankSplitWorkflow.States, LandBankSplitWorkflow.Triggers>((LandBankSplitWorkflow.States)Workflow.CurrentState);

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

    private void SetRegionId()
    {
        regionId ??= Context.SysConfigs.First(e => e.Name.Equals("region_code")).Value;
    }
    internal LandBankFacadeModel.SplitData GetSplitData(Guid wfid)
    {
        var w = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
        var r = (LandBankFacadeModel.LandPreparationRequest)w.Data;
        var p = _landBankService.GetLand(Guid.Parse( r.landID),false,false);

        return new LandBankFacadeModel.SplitData()
        {
            n=r.n,
            area=p.parcels[p.Upins[0]].areaGeom,
            upin=p.Upins[0],
        };
    }
    LandBankFacadeModel.LandPreparationRequest GetPreparationRequest(Guid wfid)
    {
        var request = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
        if (request == null)
            return null;
        return (LandBankFacadeModel.LandPreparationRequest)request.Data;

    }
    public Guid RequestParcelSplit(LandBankFacadeModel.LandPreparationRequest request, string wfid)
    {
        int prevState;
        Workflow wf;
        if (String.IsNullOrEmpty(wfid))
        {
            wf = _workflowService.CreateWorkflow(new Workflows.Models.WorkflowRequest()
            {
                CurrentState = (int)States.ParcelSplitRequested,
                Description = "Initial parcel split request",
                TypeId = (int)WorkflowTypes.PrepareLand,
            });
            prevState = (int)States.Started;
        }
        else
        {
            ConfigureMachine(Guid.Parse(wfid));
            wf = Workflow;
            prevState = wf.CurrentState;
        }
        
        var l = _landBankService.GetLand(Guid.Parse( request.landID),false,false);
        CamisUtils.Assert(request.n>1, $"Split no should be at least 2");
        CamisUtils.Assert(l != null, $"{request.landID} is land id");
        CamisUtils.Assert(l?.Upins.Count == 1, $"Land {request.landID} doesn't have unique UPIN");
        var p = l?.parcels[l.Upins[0]];
        CamisUtils.Assert(p.IsStateLand(), $"Only state land can be prepared.");
        CamisUtils.Assert(p!= null, $"Land {request.landID} doesn't have associated land Profile");
        
        States nexState;
        Triggers trigger;
        int role;
        SetRegionId();
        if (regionId.Equals("AM"))
        {
            nexState = States.WaitingForCMSS;
            trigger = Triggers.WaitForCMSS;
            role = UserRoles.CMSSUser;
        }
        else
        {
            nexState = States.WaitingForNRLAIS;
            trigger = Triggers.WaitForNRLAIS;
            role = UserRoles.LandAdmin;
        }
        _landBankService.SetLandState(Guid.Parse(request.landID), LandBankFacadeModel.LandTypeEnum.OnSplit);
        _workflowService.CreateWorkItemChangeState(new Workflows.Models.WorkItemRequest()
        {
            WorkflowId = wf.Id.ToString(),
            FromState = prevState,
            ToState=(int)nexState,
            Trigger = (int)trigger,
            DataType = typeof(LandBankFacadeModel.LandPreparationRequest).ToString(),
            Data = Newtonsoft.Json.JsonConvert.SerializeObject(request),
            Description = "Parcel Split Requested",
            AssignedRole = role
        });
        return wf.Id;
    }
    
    public Guid CancelParcelSplitRequest(Guid wfid, string note)
    {
     ConfigureMachine(wfid);
     return fireAction(wfid, Triggers.Cancel, note, null).Id;
    }

    public Guid CmssDoneSplitting(LandBankFacadeModel.LandPreparationRequest request, Guid wfid, string note)
    {
        ConfigureMachine(wfid);
        return fireAction(wfid, Triggers.CMSSParcelSplitted, note, UserRoles.LandAdmin, request).Id;
    }

    public Guid CmssRejectSplitting(Guid wfid, string note)
    {
        ConfigureMachine(wfid);
        var data = GetPreparationRequest(wfid);
        return fireAction(wfid, Triggers.CMSSReject, note, UserRoles.LandAdmin, data).Id;
    }

    public void SaveCmssWork(Guid wfid, LandBankFacadeModel.LandPreparationRequest request)
    {
        var data = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
        if(data==null)
            throw new Exception("sorry i can't find last work_item for this workflow_id ");

        data.Data = request;
        _workflowService.UpdateWorkItem(data);
    }
    public Guid RejectParcelSplitting(Guid wfid, string note)
    {
        ConfigureMachine(wfid);
        var data = GetPreparationRequest(wfid);
        return fireAction(wfid, Triggers.Reject, note, UserRoles.CMSSUser, data).Id;
    }
    public Guid ApproveParcelSplitting(Guid wfid, string note)
    {
        ConfigureMachine(wfid);
        var data = GetPreparationRequest(wfid);
        OnApproveSaveParcelSplitting(wfid, data);
        return fireAction(wfid, Triggers.Approve, note, null, data).Id;
    }

    private void OnApproveSaveParcelSplitting(Guid wfid, LandBankFacadeModel.LandPreparationRequest request)
    {
        var land = _landBankService.GetLand(Guid.Parse((ReadOnlySpan<char>)request.landID), false, false);
        if(land==null)
            throw new Exception("sorry i can't find land");
        if(request.geoms.Count==0)
            throw new Exception("Geometry can't be empty");
        
        if(request.geoms.Count!=request.n)
            throw new Exception("No of splited land should be equal with requested no split");
        List<LandSplit> splited = new List<LandSplit>();
        if (request.subLand > 0)
            DeleteSubParcel(request.subLand);
        var  oldSplitIndex=Context.LandSplit.Where(s=>s.LandId==Guid.Parse(request.landID)).Max(e=>e.Indexes);
        foreach (var sp in request.geoms)
        {
            oldSplitIndex++;
            splited.Add(new LandSplit
            {
                Geom = ParseGeometry(sp),
                LandId = Guid.Parse(request.landID),
                Indexes = oldSplitIndex,
                Status = (int)LandBankFacadeModel.LandTypeEnum.Prepared,
                Wid = wfid
            });
        }
        Context.LandSplit.AddRange(splited);
        Context.SaveChanges();
        _landBankService.SetLandState(Guid.Parse(request.landID), LandBankFacadeModel.LandTypeEnum.OnSplit);
    }

    private Geometry ParseGeometry(string geom)
    {
        int targetSrid = 20137;
        var wktReader = new WKTReader();
        MultiPolygon multiPolygonGeometry = (MultiPolygon)wktReader.Read(geom);
        multiPolygonGeometry.SRID = targetSrid;
        return multiPolygonGeometry;
    }

    private void DeleteSubParcel(int id)
    {
        var oldParcel=Context.LandSplit.FirstOrDefault(e=>e.Id==id);
        if (oldParcel != null)
        {
            Context.LandSplit.Remove(oldParcel);
            Context.SaveChanges();
        }
            
    }
    void DefineStateMachine()
    {
        _machine.Configure(States.Started)
            .Permit(Triggers.RequestSplitting, States.ParcelSplitRequested)
            .Permit(Triggers.Cancel, States.Cancelled);

        _machine.Configure(States.ParcelSplitRequested)
            .Permit(Triggers.WaitForNRLAIS, States.WaitingForNRLAIS)
            .Permit(Triggers.WaitForCMSS, States.WaitingForCMSS)
            .Permit(Triggers.Cancel, States.Cancelled)
            ;


        _machine.Configure(States.WaitingForNRLAIS)
            .Permit(Triggers.NRLAISApprov, States.Executed)
            .Permit(Triggers.NRLAISReject, States.NRLAISRejected);

        _machine.Configure(States.NRLAISRejected)
            .Permit(Triggers.RequestSplitting, States.ParcelSplitRequested)
            .Permit(Triggers.Cancel, States.Cancelled);

        _machine.Configure(States.WaitingForCMSS)
            .Permit(Triggers.CMSSParcelSplitted, States.CMSSDoneSplit)
            .Permit(Triggers.CMSSReject, States.CMSSRejected);

        _machine.Configure(States.CMSSDoneSplit)
            .Permit(Triggers.Approve, States.Executed)
            .Permit(Triggers.Reject, States.ParcelSplitRequested)
            .Permit(Triggers.Cancel, States.Cancelled);

        _machine.Configure(States.CMSSRejected)
            .Permit(Triggers.WaitForCMSS, States.WaitingForCMSS)
            .Permit(Triggers.Cancel, States.Cancelled);
    }
}