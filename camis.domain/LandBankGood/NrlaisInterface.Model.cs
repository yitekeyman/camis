using System;
using System.Collections.Generic;
using System.Text;

namespace intapscamis.camis.domain.LandBank
{
    
    public interface NrlaisInterface
    {
        NrlaisInterfaceModel.NrlaisParcelResult getLandData(String upid);
        Guid RequestLandSplit(NrlaisInterfaceModel.Parcel parcel, List<String> geometries);
    }
}
