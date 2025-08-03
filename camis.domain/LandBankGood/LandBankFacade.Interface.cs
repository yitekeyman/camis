using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.LandBankGood.ViewModel;
using intapscamis.camis.domain.Workflows.Models;
using System;
using System.Collections.Generic;
using System.Text;
namespace intapscamis.camis.domain.LandBank
{
    
    


    public interface ILandBankFacade
    {
        Guid RequestLandRegistration(LandBankFacadeModel.LandData data,String wfid);
        List<LandBankFacadeModel.LandBankWorkItem> GetUserWorkItems();
        LandBankFacadeModel.LandData GetWorkFlowLand(Guid wfid);
        Guid ApproveRegistration(Guid wfid, String note);
        void SetSession(UserSession userSession);
        LandBankFacadeModel.LandSearchResult SearchLand(LandBankFacadeModel.LandSearchPar par);
        LandBankFacadeModel.LatLng GetLandCoordinate(Guid landId);
        LandBankFacadeModel.LandData GetLand(Guid landId, bool geom,bool dd);
        Guid CancelRegistrationRequest(Guid wfid, string note);
        Guid RejectLandPreparationRequest(Guid wfid, string note);
        Guid CancelLandPreparationRequest(Guid wfid, string note);
        LandBankFacadeModel.Bound GetLandMapBound();
        Guid RejectRegistrationRequest(Guid wfid,string note);
        LandBankFacadeModel.SplitTaskList GetSplitTaskList();
        Guid RequestLandPreparation(LandBankFacadeModel.LandPreparationRequest request);
        LandBankFacadeModel.SplitData GetSplitData(Guid wfid);
        LandBankFacadeModel.SplitTaskGeom GetTaskGeom(Guid guid);
        void SplitParcel(Guid wfid, List<string> geoms);
        Guid ApprovePreparation(Guid wfid, String note);
        int GetPreparationStatus(Guid wfid);
        Guid RequestLandTransfer(LandBankFacadeModel.TransferRequest request);
        int GetTransferStatus(Guid wfid);
        WorkItemResponse GetLastWorkItem<T>(Guid guid);


        LandAttributeName GetLandAttributeNames();
        List<LandBankFacadeModel.LandData> GetLandData(Guid[] excludedIds);

    }
    
}
