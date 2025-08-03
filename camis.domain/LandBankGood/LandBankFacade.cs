using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using intapscamis.camis.domain.LandBankGood.ViewModel;

namespace intapscamis.camis.domain.LandBank
{
    public class LandBankFacade:CamisFacade, ILandBankFacade
    {
        LandBankWorkflow _landBankWorkflow;
        LandBankPrepareWorkflow _landPrepareWorkflow;

        UserSession _session;
        LandBankService _landBankService;
        CamisContext _context;
        public LandBankFacade(CamisContext context)
        {
            _landBankService = new LandBankService();
            _landBankWorkflow = new LandBankWorkflow(_landBankService);
            _landPrepareWorkflow = new LandBankPrepareWorkflow(_landBankService);
            this._context = context;
        }
        public void SetSession(UserSession session)
        {
            _session = session;
            _landBankWorkflow.SetSession(_session);
            _landBankService.SetSession(_session);
            _landPrepareWorkflow.SetSession(_session);

        }

        public Guid RequestLandRegistration(LandBankFacadeModel.LandData data,String wfid)
        {
            return  base.Transact<Guid>(_context,(t) =>
                {
                    PassContext(_landBankWorkflow,_context);
                    return _landBankWorkflow.RequestLandRegistration(data,wfid);
                });    
           
        }

        public List<LandBankFacadeModel.LandBankWorkItem> GetUserWorkItems()
        {
            PassContext(_landBankWorkflow, _context);
            return _landBankWorkflow.getUserWorkItems();
        }

        public LandBankFacadeModel.LandData GetWorkFlowLand(Guid wfid)
        {
            PassContext(_landPrepareWorkflow, _context);
            var wfs = new WorkflowService();
            wfs.SetSession(_session);
            wfs.SetContext(_context);
            var wf = wfs.GetWorkflow(wfid);
            if (wf == null)
                    
                return null;
            if (wf.TypeId == (int)WorkflowTypes.LandRegistration)
            {
                PassContext(_landBankWorkflow, _context);
                return _landBankWorkflow.GetWorkFlowLand(wfid);
            }
            if (wf.TypeId == (int)WorkflowTypes.PrepareLand)
            {
                var data = wfs.GetLastWorkItem<LandBankFacadeModel.LandPreparationRequest>(wfid);
                if (data == null)
                    return null;
                if (!(data.Data is LandBankFacadeModel.LandPreparationRequest))
                    return null;
                return _landBankService.GetLand(((LandBankFacadeModel.LandPreparationRequest)data.Data).landID,false,false);
            }
            return null;
        }

        public Guid ApproveRegistration(Guid wfid,String note)
        {
            return base.Transact<Guid>(_context,(t) =>
            {
                PassContext(_landBankWorkflow, _context);
                return _landBankWorkflow.ApproveLandRegistration(wfid,note);              
            });
        }
        public Guid ApprovePreparation(Guid wfid, String note)
        {
            return base.Transact<Guid>(_context, (t) =>
            {
                PassContext(_landPrepareWorkflow, _context);
                return _landPrepareWorkflow.ApprovePreparation(wfid, note);
            });
        }
        public int GetPreparationStatus(Guid wfid)
        {
            return base.Transact<int>(_context, (t) =>
            {
                PassContext(_landPrepareWorkflow, _context);
                return _landPrepareWorkflow.GetPreparationStatus(wfid);
            });
        }
        public LandBankFacadeModel.LandSearchResult SearchLand(LandBankFacadeModel.LandSearchPar par)
        {
            PassContext(_landBankService,_context);
            return _landBankService.SearchLand(par);
        }

        public LandBankFacadeModel.LandData GetLand(Guid landId, bool geom, bool dd)
        {
            PassContext(_landBankService, _context);
            return _landBankService.GetLand(landId,geom,dd);
        }
        public LandBankFacadeModel.LatLng GetLandCoordinate(Guid landId)
        {
            PassContext(_landBankService, _context);
            return _landBankService.GetLandCoordinate(landId);
        }
        public LandBankFacadeModel.Bound GetLandMapBound()
        {
            PassContext(_landBankService, _context);
            return _landBankService.GetLandMapBound();
        }

        public Guid CancelRegistrationRequest(Guid wfid, string note)
        {
            return base.Transact<Guid>(_context, (t) =>
            {
                PassContext(_landBankWorkflow, _context);
                return _landBankWorkflow.CancelLandRegistration(wfid, note);
            });
        }
        public Guid CancelLandPreparationRequest(Guid wfid, string note)
        {
            return base.Transact<Guid>(_context, (t) =>
            {
                PassContext(_landPrepareWorkflow, _context);
                return _landPrepareWorkflow.CancelRequest(wfid, note);
            });
        }
        public Guid RejectLandPreparationRequest(Guid wfid, string note)
        {
            return base.Transact<Guid>(_context, (t) =>
            {
                PassContext(_landPrepareWorkflow, _context);
                return _landPrepareWorkflow.RejectRequest(wfid, note);
            });
        }

        public Guid RejectRegistrationRequest(Guid wfid, string note)
        {
            return base.Transact<Guid>(_context, (t) =>
            {
                PassContext(_landBankWorkflow, _context);
                return _landBankWorkflow.RejectLandRegistration(wfid, note);
            });
        }

        public LandBankFacadeModel.SplitTaskList GetSplitTaskList()
        {
            PassContext(_landPrepareWorkflow, _context);
            return _landPrepareWorkflow.GetSplitList();
        }

        public Guid RequestLandPreparation(LandBankFacadeModel.LandPreparationRequest request)
        {
            return base.Transact<Guid>(_context, (t) =>
            {
                PassContext(_landPrepareWorkflow, _context);
                return _landPrepareWorkflow.RequestLandPreparation(request, null);
            });

            
        }

        public LandBankFacadeModel.SplitData GetSplitData(Guid wfid)
        {
            PassContext(_landPrepareWorkflow, _context);
            return _landPrepareWorkflow.GetSplitData(wfid);
        }

        public LandBankFacadeModel.SplitTaskGeom GetTaskGeom(Guid wfid)
        {
            PassContext(_landPrepareWorkflow, _context);
            return _landPrepareWorkflow.GetTaskGeom(wfid);
        }

        public void SplitParcel(Guid wfid, List<string> geoms)
        {
            base.Transact(_context, (t) =>
            {
                PassContext(_landPrepareWorkflow, _context);
                _landPrepareWorkflow.SetSplitGeometries(wfid,
                new LandBankFacadeModel.SetPrepareGeometries()
                {
                    geoms = geoms
                });
            });
            
        }

        public Guid RequestLandTransfer(LandBankFacadeModel.TransferRequest request)
        {
            return Transact<Guid>(_context, tran=>
            {
                var t = new LandBankTransferWorkflow(_landBankService);
                t.SetContext(this._context);
                t.SetSession(this._session);
                return t.RequestLandTransfer(request);
            });

        }
        public int GetTransferStatus(Guid wfid)
        {
            return Transact<int>(_context, x =>
            {
                var t = new LandBankTransferWorkflow(_landBankService);
                t.SetContext(this._context);
                t.SetSession(this._session);
                return t.GetTransferStatus(wfid);
            });
        }

        LandBankFacadeModel.LatLng ILandBankFacade.GetLandCoordinate(Guid landId)
        {
            PassContext(_landBankService, _context);
            return _landBankService.GetLandCoordinate(landId);
        }

        public WorkItemResponse GetLastWorkItem<T>(Guid guid)
        {
            PassContext(_landBankWorkflow, _context);
            return _landBankWorkflow.GetLastWorkItem<T>(guid);
        }

       
        public LandAttributeName GetLandAttributeNames()
        {
            PassContext(_landBankService,_context);
            return _landBankService.GetLandAttributeName();
        }

        public List<LandBankFacadeModel.LandData> GetLandData(Guid[] excludedIds)
        {
            PassContext(_landBankService, _context);
            return _landBankService.GetLandData(excludedIds);
        }
    }
}
