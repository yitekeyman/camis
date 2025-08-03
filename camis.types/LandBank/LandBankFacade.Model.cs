using intapscamis.camis.domain.Documents.Models;
using System;
using System.Collections.Generic;
using System.Text;
namespace intapscamis.camis.domain.LandBank
{


    public class LandBankReportModel
    {
        
        public class LandBankSummary
        {
            public class AreaCount
            {
                public int count { get; set; } = 0;
                public double area { get; set; } = 0;
            }
            public AreaCount identified_state { get; set; } = new AreaCount();
            public AreaCount identified_private { get; set; } = new AreaCount();

            public AreaCount prepared_state { get; set; } = new AreaCount();
            public AreaCount prepared_private { get; set; } = new AreaCount();

            public AreaCount transfered_state { get; set; } = new AreaCount();
            public AreaCount transfered_private { get; set; } = new AreaCount();
            public AreaCount transfered_private_cf { get; set; } = new AreaCount();

        }
    }

    public class LandBankFacadeModel
    {
        public class LandSearchPar
        {
            public String Upin;
            public int LandType = -1;
            public double AreaMin = -1;
            public double AreaMax = -1;
        }
        public class LandSearchResult
        {
            public List<LandData> Result;
            public int NRec;
        }
        public enum LandTypeEnum
        {
            Identified = 1,
            Prepared = 2,
            Transfered = 3,
        }
        public enum LandRightType
        {
            LeaseFromState = 1,
            RentFromPrivate = 2,
            Private = 3,
            ContractFarming = 4,
            SubLease = 5
        }
        public class SoilTest
        {
            public int TestType { get; set; }
            public string Result { get; set; }
        }
        public class Climate
        {
            public int month { get; set; }
            public double temp_low { get; set; }
            public double temp_high { get; set; }
            public double temp_avg { get; set; }
            public double precipitation { get; set; }

        }
        public class LatLng
        {
            public double lat=0;
            public double lng=0;

            public static LatLng FromWkt(string point)
            {
                var i1 = point.IndexOf("Point(", StringComparison.CurrentCultureIgnoreCase);
                var i2 = point.IndexOf(' ', i1);
                var i3 = point.IndexOf(')', i2);
                return new LatLng()
                {
                    lng = double.Parse(point.Substring(i1 + "Point(".Length, i2 - i1 - "Point(".Length)),
                    lat = double.Parse(point.Substring(i2+1, i3-i2-1)),
                };
            }
        }
        public class LandData
        {
            public LandData()
            {

            }
            public String LandID { get; set; }
            public string WID { get; set; }
            public int LandType { get; set; }
            public List<int> Accessablity { get; set; }
            public List<SoilTest> SoilTests { get; set; }
            public List<string> Upins { get; set; }
            public Dictionary<String, NrlaisInterfaceModel.Parcel> parcels { get; set; }
            public string Description { get; set; }
            public double Area { get; set; }
            public double CentroidX { get; set; }
            public double CentroidY { get; set; }
            public List<Climate> Climate { get; set; }
            public String Holdership {get;set;}="Unknown";
            public String FarmID { get; set; } = null;
            public int landHolderType { get; set; } //1:private, 3:state land



            #region New Data
            public List<AgroEchologyZone> AgroEchologyZone { get; set; }
            public List<int> InvestmentType { get; set; }
            public int MoistureSource { get; set; }
            public IrrigationValues IrrigationValues { get; set; }
            public List<Topography> Topography { get; set; }
            public List<int> ExistLandUse { get; set; }
            public string IsAgriculturalZone { get; set; }
            public List<DocumentRequest> UploadDocument { get; set; }
            
            #endregion
        }

        #region Older DataSet
        public class LandBankWorkItemNote
        {
            public String note { get; set; }
        }
        
        public class LandBankWorkItem
        {
            public String wiid;//workflow id
            public String wfid;//work item id
            public DateTime workItemDate;//date of the last work flow action
            public String sentUserID;//user ID of the last action work flow action
            public String sentUserName;//user name of the last action work flow action
            public String description;//description of the work flow
            public String workItemNote;//note entered by the last user
            public int workFlowType;//type of the workflow, see wf.workflow_type table
        }
        public class Bound
        {
            public double X1, Y1, X2, Y2;
        }
        public class SplitData
        {
            public string upin;
            public int n;
            public double area;
        }
        public class SplitTaskItem
        {
            public String date;
            public String id;
            public String description;
            public int n;
            public string upin;
            public bool pending = true;
        }
        public class SplitTaskList
        {
            public List<SplitTaskItem> tasks;
        }
        public class LandPreparationRequest
        {
            public Guid landID;
            public int n;
        }

        public class SetPrepareGeometries
        {
            public List<String> geoms;
        }

        public class TransferRequest
        {
            public Farms.Models.FarmOperatorRequest farmer;
            public Guid landID;
            public DateTime leaseFrom;
            public DateTime leaseTo;
            public LandRightType right = LandRightType.LeaseFromState;
            public double? yearlyLease;
            public double? landSectionArea;
            public Guid txuid;
        }

        public class SplitTaskGeom
        {
            public int id;
            public String geom;
            public String label;
            public double area;
        }
        #endregion
        
        #region New Data
        public class AgroEchologyZone
        {
            public int AgroType { get; set; }
            public string Result { get; set; }
        }
        public class IrrigationValues
        {
            public List<WaterSourceParameter> WaterSourceParameter { get; set; }
            public List<int> GroundWater { get; set; } //check this
            public List<SurfaceWater> SurfaceWater { get; set; }
        }
        public class WaterSourceParameter
        {
            public int WaterSourceType { get; set; }
            public int Result { get; set; }
        }
        public class SurfaceWater
        {
            public int SurfaceWaterType { get; set; }
            public string Result { get; set; }
        }
        public class Topography
        {
            public int TopographyType { get; set; }
            public string Result { get; set; }
        }
        #endregion
    }

}
