using camis.types.Utils;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Farms.StateMachines;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Microsoft.EntityFrameworkCore;
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
        Executed = -2,
        Approved = -3,
        Rejected = 11,
        Cancelled = -4,
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
        if (workflowId == Guid.Empty)
        {
            _machine = new StateMachine<States, Triggers>(States.ParcelSplitRequested);
        }
        else
        {
            Workflow = Context.Workflow.First(wf =>
                wf.Id == workflowId && wf.TypeId == (int)WorkflowTypes.PrepareLand);
            _machine = new StateMachine<LandBankSplitWorkflow.States, LandBankSplitWorkflow.Triggers>(
                (LandBankSplitWorkflow.States)Workflow.CurrentState);
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

    private void SetRegionId()
    {
        regionId ??= Context.SysConfigs.First(e => e.Name.Equals("region_code")).Value;
    }

    internal LandBankFacadeModel.SplitData GetParcelSplitData(Guid wfid)
    {
        var w = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
        var r = (LandBankFacadeModel.LandPreparationRequest)w.Data;
        var p = _landBankService.GetLand(Guid.Parse(r.LandId), false, false);
        double area = p.parcels[p.Upins[0]].areaGeom;
        var upid = p.Upins[0];
        if (r.SubLand > 0)
        {
            foreach (var sp in p.LandSplit)
            {
                if (sp.Id == r.SubLand)
                {
                    area = sp.Area;
                    upid=upid+"-"+sp.Indexes;
                }
            }
        }
        return new LandBankFacadeModel.SplitData()
        {
            n = r.NoOfSplit,
            area = area,
            upin = upid,
        };
    }

    internal string GetRegionCode()
    {
        string ret = null;
        ret = Context.SysConfigs.First(e => e.Name.Equals("region_code")).Value;
        return ret;
    }

    internal LandBankFacadeModel.SplitTaskList GetParcelSplitTaskList()
    {
        var ret = new LandBankFacadeModel.SplitTaskList();
        ret.tasks = new List<LandBankFacadeModel.SplitTaskItem>();
        var ws = _workflowService.GetWorkflows((int)WorkflowTypes.PrepareLand, (int)States.WaitingForCMSS);
        foreach (var w in ws)
        {
            var wi = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(w.Id);
            var data = wi.Data as LandBankFacadeModel.LandPreparationRequest;
            if (data?.NoOfSplit > 0)
            {
                var land = _landBankService.GetLand(Guid.Parse(data.LandId), false, false);
                var upid = land.Upins[0];
                if (data.SubLand > 0)
                {
                    foreach (var sp in land.LandSplit)
                    {
                        if (sp.Id == data.SubLand)
                            upid = upid + "-" + sp.Indexes;
                    }
                }
                
                ret.tasks.Add(new LandBankFacadeModel.SplitTaskItem()
                {
                    id = w.Id.ToString(),
                    description = data.Description,
                    n = data.NoOfSplit,
                    upin = upid,
                });
            }
        }

        return ret;
    }

    internal List<LandBankFacadeModel.SplitTaskGeom> GetParcelSplitTaskGeom(Guid wfid)
    {
        var w = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
        var r = (LandBankFacadeModel.LandPreparationRequest)w.Data;
        Context.Database.OpenConnection();
        var rets = new List<LandBankFacadeModel.SplitTaskGeom>();
        if (r.GeomData!=null && r.GeomData.Count > 0)
        {
            foreach (var tGeom in r.GeomData)
            {
                rets.Add(new LandBankFacadeModel.SplitTaskGeom()
                {
                    id = tGeom.id,
                    area = tGeom.area,
                    geom = tGeom.geom,
                    label = "Parcel-" + tGeom.id
                });
            }

            return rets;
        }
        else
        {
            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                var sql = $"Select ST_AsText(geometry) from lb.land_upin where land_id='{r.LandId}'";
                if (r.SubLand > 0)
                {
                    sql = $"Select ST_AsText(geom) from lb.land_split where land_id='{r.LandId}' and id={r.SubLand}";
                }

                command.CommandText = sql;
                var res = command.ExecuteScalar();

                rets.Add(new LandBankFacadeModel.SplitTaskGeom()
                {
                    area = 0,
                    id = 1,
                    geom = res.ToString(),
                    label = "split"
                });
                return rets;
            }
        }


        return null;
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
            ConfigureMachine(Guid.Empty);
            wf = _workflowService.CreateWorkflow(new Workflows.Models.WorkflowRequest()
            {
                CurrentState = (int)States.ParcelSplitRequested,
                Description = "Initial parcel split request",
                TypeId = (int)WorkflowTypes.PrepareLand,
            });
            prevState = (int)States.ParcelSplitRequested;
        }
        else
        {
            ConfigureMachine(Guid.Parse(wfid));
            wf = Workflow;
            prevState = wf.CurrentState;
        }

        var l = _landBankService.GetLand(Guid.Parse(request.LandId), false, false);
        var landSplit = l.LandSplit;
        if (request.SubLand > 0)
        {
            foreach (var sp in landSplit)
            {
                if (sp.Id == request.SubLand)
                    CamisUtils.Assert((sp.Status == 2 || sp.Locked),
                        $"Parcel part {sp.Indexes} status doesn't allow split");
            }
        }

        CamisUtils.Assert(request.NoOfSplit > 1, $"Split no should be at least 2");
        CamisUtils.Assert(l != null, $"{request.LandId} is land id");
        CamisUtils.Assert((l.LandType == 2 || l.LandType == 5 || l.LandType == 6), "Land Status doesn't allow split");
        CamisUtils.Assert(l?.Upins.Count == 1, $"Land {request.LandId} doesn't have unique UPIN");
        var p = l?.parcels[l.Upins[0]];
        CamisUtils.Assert(p.IsStateLand(), $"Only state land can be prepared.");
        CamisUtils.Assert(p != null, $"Land {request.LandId} doesn't have associated land Profile");


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

        if (regionId.Equals("AM") || regionId.Equals("am") || regionId.Equals("03"))
        {
            if (request.SubLand > 0)
            {
                _landBankService.SetSubLandState(Guid.Parse(request.LandId), request.SubLand,
                    LandBankFacadeModel.LandTypeEnum.OnSplit);
                _landBankService.SetSubLandLock(Guid.Parse(request.LandId), request.SubLand, true);
            }
            else
            {
                _landBankService.SetLandState(Guid.Parse(request.LandId), LandBankFacadeModel.LandTypeEnum.OnSplit);
                _landBankService.SetLandLock(Guid.Parse(request.LandId), true);
            }
        }
        else
        {
            throw new InvalidOperationException("Parcel Spliting operation allowed for only Amhara region");
        }

        fireAction(wf.Id, trigger, request.Description, role, request);
        return wf.Id;
    }

    public Guid CancelParcelSplitRequest(Guid wfid, string note)
    {
        ConfigureMachine(wfid);
        var data = GetPreparationRequest(wfid);
        _landBankService.SetLandLock(Guid.Parse(data.LandId), true);
        if (data.SubLand > 0)
            _landBankService.SetSubLandLock(Guid.Parse(data.LandId), data.SubLand,true);
        return fireAction(wfid, Triggers.Cancel, note, null).Id;
    }

    public Guid CmssDoneSplitting(LandBankFacadeModel.SplitParcelData request, Guid wfid, string note)
    {
        ConfigureMachine(wfid);
        if (string.IsNullOrEmpty(note))
            note = "CAMIS-v2 QGIS done parcel splitting";
        var wi = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
        var data = (LandBankFacadeModel.LandPreparationRequest)wi.Data;
        data.GeomData = request.geomData;
        return fireAction(wfid, Triggers.CMSSParcelSplitted, note, UserRoles.LandAdmin, data).Id;
    }

    public Guid CmssRejectSplitting(Guid wfid, string note)
    {
        ConfigureMachine(wfid);
        if (string.IsNullOrEmpty(note))
            note = "CAMIS-v2 QGIS reject splitting with unknown reason";
        var data = GetPreparationRequest(wfid);
        return fireAction(wfid, Triggers.CMSSReject, note, UserRoles.LandAdmin, data).Id;
    }

    public void SaveCmssWork(Guid wfid, LandBankFacadeModel.SplitParcelData request)
    {
        var wi = _workflowService.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);

        if (wi == null)
            throw new Exception("sorry i can't find last work_item for this workflow_id ");
        var data = (LandBankFacadeModel.LandPreparationRequest)wi.Data;
        data.GeomData = request.geomData;

        wi.Data = data;
        _workflowService.UpdateWorkItem(wi);
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
        var land = _landBankService.GetLand(Guid.Parse((ReadOnlySpan<char>)request.LandId), false, false);
        if (land == null)
            throw new Exception("sorry i can't find land");
        if (request.GeomData.Count == 0)
            throw new Exception("Geometry can't be empty");

        if (request.GeomData.Count != request.NoOfSplit)
            throw new Exception("No of splited land should be equal with requested no of split");
        List<LandSplit> splited = new List<LandSplit>();
        if (request.SubLand > 0)
            DeleteSubParcel(request.SubLand);
        var query = Context.LandSplit.Where(s => s.LandId == Guid.Parse(request.LandId));
        var oldSplitIndex = query.Any() ? query.Max(s => s.Indexes) : 0;
        foreach (var sp in request.GeomData)
        {
            oldSplitIndex++;
            splited.Add(new LandSplit
            {
                Geom = ParseGeometry(sp.geom),
                Area = sp.area,
                LandId = Guid.Parse(request.LandId),
                Indexes = oldSplitIndex,
                Status = (int)LandBankFacadeModel.LandTypeEnum.Prepared,
                Wid = wfid,
                Locked = false
            });
        }

        Context.LandSplit.AddRange(splited);
        Context.SaveChanges();
        _landBankService.SetLandState(Guid.Parse(request.LandId), LandBankFacadeModel.LandTypeEnum.PreparedWithSplit);
        _landBankService.SetLandLock(Guid.Parse(request.LandId), false);
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
        var oldParcel = Context.LandSplit.FirstOrDefault(e => e.Id == id);
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
            .Permit(Triggers.Reject, States.WaitingForCMSS)
            .Permit(Triggers.Cancel, States.Cancelled);

        _machine.Configure(States.CMSSRejected)
            .Permit(Triggers.WaitForCMSS, States.WaitingForCMSS)
            .Permit(Triggers.Cancel, States.Cancelled);
    }
    
    internal int GetSplitStatus(Guid wfid)
    {
        ConfigureMachine(wfid);
        if (_machine.State == States.WaitingForNRLAIS)
        {
            var w = _workflowService.GetLastWorkItem<String>(wfid);
            var txuid = w.Data.ToString();
            WorkItem wi;
            switch (new RestNrlaisInterface().GetApplicationStatus(Guid.Parse(txuid)))
            {
                case NrlaisInterfaceModel.NrlaisApplicationStatus.Canceled:
                    wi = this.fireAction(wfid, Triggers.NRLAISReject, "Rejected by nrlais", null);
                    return (int)wi.ToState;
                case NrlaisInterfaceModel.NrlaisApplicationStatus.Completd:
                    wi = this.fireAction(wfid, Triggers.NRLAISApprov, "Approve by nrlais", null);
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
}