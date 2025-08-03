using System;
using System.Collections.Generic;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;

namespace intapscamis.camis.domain.LandBank
{
    public interface ILandBankService
    {
        void SetContext(CamisContext context);
        void SetSession(UserSession session);
        void TransferToCertification(Guid wiid, Guid landID);
        NrlaisInterfaceModel.Parcel getParcel(String upid);
        Guid RegisterLand(LandBankFacadeModel.LandData data);
        void Updateland(LandBankFacadeModel.LandData data);
        LandBankFacadeModel.LandData GetLandByUpin(string u);
        LandBankFacadeModel.LandData GetLand(Guid landID,bool geom,bool dd);
        void SetLandState(Guid landID, LandBankFacadeModel.LandTypeEnum transfered);
        void RemoveLand(Guid landID);
        void TransferLand(LandBankFacadeModel.TransferRequest request);
        void CalculateCentroid(LandBankFacadeModel.LandData land);
        List<LandBankFacadeModel.LandData> GetLandData(Guid[] excludedIds);
    }
}
