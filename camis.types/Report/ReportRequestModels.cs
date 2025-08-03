using intapscamis.camis.data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static intapscamis.camis.domain.LandBank.LandBankFacadeModel;

namespace intapscamis.camis.domain.Farms.Models
{

    public enum ReportTypes
    {

        SummaryOfLandByLandStatus = 1,
        ListOfLands = 2,
        ListOfCommercialFarmOwners = 3,
        AnnualLeaseAmountByAdministrativeLocationInvType =4,
        LandTransferredAtDifferentTimes = 5,
        LandTransferreToInvestorsByFarmSize = 6,
        SummaryOfAgricultureInvestmentByOriginOfInvestors = 7,
        SummaryOfLandByAdministrativeLocationandWaterSource = 8,
        SummaryOfSubLeasedLandByLocation = 9,
        CropProductionList = 10,
        TimeSeriesInvestorCropProduction = 11,
        FarmMachineryList=  12,
        CampingFacilitesList = 13,
        CampingFacilitySummary = 14,
        JobCreationListByProfession = 15,
        JobCreationListByType = 16,
        LandDevelopmentProgressList = 17,
        LandDevelopmentProgressSummary = 18,
        InvstmentStatusSummery= 19,
        ContractFarmingList = 20,
        ContractFarmingSummary = 21,

        ListOfAllFarmOperators = 22,
        OperatorSummaryByOrigin = 23

    }

    public enum SummerizedBy
    {
        Region = 1,
        Zone = 2,
        Woreda = 3
    }

    public enum FilteredBy
    {
        None = 0,
        Region = 1,
        Zone = 2,
        Woreda = 3
    }



    public class ReportRequestModel
    {
        public ReportTypes SelectedReportType { get; set; }

        public string Region { get; set; }

        public string[] Regions { get; set; }

        public string Zone { get; set; }
        public string Woreda { get; set; }


        public List<DateParam> Dates { get; set; }
        public List<SizeParam> FarmSizes { get; set; }

        public SummerizedBy SummerizedBy { get; set; }
        public FilteredBy FilteredBy { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid? FarmId { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }

        public bool DateEquals(List<DateParam> dates)
        {
            bool equals = true; 

            var d = this.Dates.Select(m => new DateTime(m.date.Year, m.date.Month, m.date.Day) ).OrderBy(m => m).ToList();
            var d2 = dates.Select(m => new DateTime(m.date.Year, m.date.Month, m.date.Day)).OrderBy(m => m).ToList();

            if(d.Count() != d2.Count())
            {
                return false;
            }
            else
            {
                for (int i = 0; i < d.Count(); i++)
                {
                    if (d[i] != d2[i])
                        equals = false;
                        
                }
            }
            return equals;
            
        }

        public bool SizeEquals(List<SizeParam> sizes)
        {
            bool equals = true;

            var d = this.FarmSizes.Select(m => m.size).OrderBy(m => m).ToList();
            var d2 = sizes.Select(m => m.size).OrderBy(m => m).ToList();

            if (d.Count() != d2.Count())
            {
                return false;
            }
            else
            {
                for (int i = 0; i < d.Count(); i++)
                {
                    if (d[i] != d2[i])
                        equals = false;

                }
            }
            return equals;
        }


    }

    public class DateParam
    {
        public DateTime date { get; set; }
    }

    public class SizeParam
    {
        public double size { get; set; }
    }

    public class ReportResponseModel
    {
        public int nRecords { get; set; }
        public ReportRequestModel Request { get; set; }
        public List<FarmResponse> Farms { get; set; }
        public List<LandData> Lands { get; set; }
        public List<FarmOperatorResponse> FarmOperators { get; set; }


        public List<LandAreaReportModel> LandArea { get; set; }
        public List<LandListReport> LandList { get; set; }
        public List<FarmListReport> FarmList { get; set; }
        public List<AreaInDateInverval> RegionAreaSummery { get; set; }

        public List<FarmInfoInAreaRange> FarmCount { get; set; }
        public List<FarmAreaInReigion> FarmArea { get; set; }

        public List<FarmListByDateAndArea> FarmListReport { get; set; }

        public List<OperatorResponse> OperatorList { get; set; }
        public List<OperatorResponse> OperatorSummary { get; set; }
        public List<OperatorSummaryByOrigin> OperatorSummaryByOrigins { get; set; }
        public List<LeaseSummaryResonse> LeaseSummary { get; set; }
        public List<WaterSourceSummary> WaterSourceSummary { get; set; }

        public Dictionary<Guid, List<PlanProgressItem>> CropProductionList { get; set; }
        public Dictionary<int,List<PlanProgressItem>> InvestorTimeSeriesCropProduction { get; set; }
        public Dictionary<Guid,List<PlanProgressItem>> MachineryList { get; set; }
        public Dictionary<Guid,List<PlanProgressItem>> CampingFacilityList { get; set; }

        public List<CampingFacilitySummary> CampSummary { get; set; }
        public List<JobCreationItem> JobCreationList { get; set; }
        public List<PlanProgressItem> LandDevelopmentList { get; set; }
        public List<LandDevelopmentSummaryItem> LandDevelopmentSummary { get; set; }
        public List<InvestmentStatusSummaryItem> InvestmentStatusSummary { get; set; }

        public List<FarmLandSummary> FarmLandSummary { get; set; }

        public List<ContractFarmModel> ContractFarmingList { get; set; }
        public List<ContractFarmSummaryModel> ContractFarmSummary { get; set; }

        public ReferenceData ReferenceData { get; set; }

        public Dictionary<string,string> LocationLookup { get; set; }
        public string AddressType { get; set; }
    }

    public class WaterSourceSummary
    {
        public string location { get; set; }
        public List<RainfedSummaryItem> RainfedSummary { get; set; }
        public List<SurfaceWaterSummrayItem> SurfaceWaterSummary { get; set; }
        public List<GroundWaterSummaryItem> GroundWaterSummary { get; set; }
    }


    public class RainfedSummaryItem
    {
        public string location { get; set; }
        public double area { get; set; }
        public int count { get; set; }
    }

    public class SurfaceWaterSummrayItem
    {
        public string location { get; set; }
        public double area { get; set; }
        public int count { get; set; }
        public int type { get; set; }

    }

    public class GroundWaterSummaryItem
    {
        public string location { get; set; }
        public double area { get; set; }
        public int count { get; set; }
        public int type { get; set; }

    }

    public class ReferenceData
    {
        public List<InverstmentType> InvestmentTypes { get; set; }
        public List<MoistureSource> MoistureSources { get; set; }
        public IList<FarmOperatorTypeResponse> FarmOperatorTypes { get; set; }
        public IList<FarmTypeResponse> FarmTypes { get; set; }
        public IList<FarmOperatorOriginResponse> FarmOperatorOrigins { get; set; }
        public IList<SurfaceWaterType> SurfaceWaterType { get; set; }
        public IList<GroundWater> GroundWaterType { get; set; }

    }

    public class FarmLandSummary
    {
        public string Location { get; set; }
        public int Count { get; set; }
        public double Area { get; set; }
        public double SubleasedArea { get; set; }

    }

    public class ContractFarmModel
    {
        public Guid farmId { get; set; }
        public string farmName { get; set; }
        public string upin { get; set; }
        public double  area { get; set; }
        public string woreda { get; set; }
        public List<ContractedFarmModel> ContractedFarms { get; set; }

        public int RowCount { get
            {
                return this.ContractedFarms.Sum(m => m.MaxPPICount);
            }
        }
    }

    public class ContractedFarmModel
    {
        public string upin { get; set; }
        public List<PlanProgressItem> Input { get; set; }
        public List<PlanProgressItem> Output { get; set; }

        public int MaxPPICount { get
            {
                return Math.Max(Input.Count(), Output.Count());
            } }
    }

    public class ContractFarmSummaryModel
    {
        public string location { get; set; }
        public int numberOfFarms { get; set; }
        public int numberOfContractors { get; set; }
        public List<ProgressItem> input { get; set; }
        public List<ProgressItem> output { get; set; }

        public int RowCount
        {
            get
            {
                return Math.Max(input.Count(), output.Count());
            }
        }

    }

   public class ProgressItem
    {
        public string tag { get; set; }
        public double progress { get; set; }

    }

    public class LandAreaReportModel
    {
        public string region { get; set; }
        public string zone { get; set; }
        public string woreda { get; set; }
        public string location { get; set; }
        public int type { get; set; }
        public double area { get; set; }


    }

    public class CampingFacilitySummary
    {
        public string Location { get; set; }
        public int farmsWithCamps { get; set; }
        public int farmsWithoutCamps { get; set; }

    }

    public class LandListReport
    {
        public Guid id { get; set; }
        public string region { get; set; }
        public string zone { get; set; }
        public string woreda { get; set; }
        public string upin { get; set; }
        public double area { get; set; }
        public int type { get; set; }
        public string name { get; set; }

        public ICollection<LandInvestment> LandInvestment { get; set; }
        public ICollection<LandMoisture> LandMoisture { get; set; }
        public ICollection<LandUsage> LandUsage { get; set; }
        public ICollection<Irrigation> Irrigation { get; set; }
        public LandRight LandRight { get; set; }
        public List<SurfaceWaterItem> SurfaceWater { get; set; }
        public List<GroundWaterItem> GroundWater { get; set; }

    }

    public class SurfaceWaterItem
    {
        public Guid land_id { get; set; }
        public int type { get; set; }
        public string result { get; set; }

    }

    public class GroundWaterItem
    {
        public Guid land_id { get; set; }
        public int type { get; set; }
    }

    public class FarmListReport
    {
        public string Region { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public double Area { get; set; }
        public int Number { get; set; }

    }

    public class OperatorResponse
    {
        public string Woreda { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
        public int OriginId { get; set; }
        public int InvestmentType { get; set; }
        public DateTime StartDate { get; set; }
        public double InvestedCapital { get; set; }
        public string LicenseNumber { get; set; }

        public string Location { get; set; }

        public string Nationality { get; set; }
        public string Type { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Origin { get; set; }
        public string Authroity { get; set; }
        public string RegistrationType { get; set; }

        public Guid Id { get; set; }

    }

    public class OperatorSummaryByOrigin
    {
        public string Origin { get; set; }
        public int Count { get; set; }
    }

    public class LeaseSummaryResonse
    {
        public string location { get; set; }
        public int land_type { get; set; }
        public double lease { get; set; }
    }


    public class AreaInDateInverval
    {
        public long T1 { get; set; }
        public long T2 { get; set; }
        public List<AreaRegionReport> RegionAreas { get; set; }

    }

    public class AreaRegionReport
    {
        public string region { get; set; }
        public double area { get; set; }
    }



    public class FarmInfoInAreaRange
    {
        public double areaFrom { get; set; }
        public double areaTo { get; set; }
        public Dictionary<string, FarmAreaCount> RegionAreaCount { get; set; }
    }

    public class FarmAreaCount
    {
        public int count { get; set; }
        public double area { get; set; }
    }

    public class FarmAreaInReigion
    {
        public string Region { get; set; }
        public double TotalArea { get; set; }

    }

    public class FarmListByDateAndArea
    {
        public string Name { get; set; }
        public int Origin { get; set; }
        public int Type { get; set; }
        public string Region { get; set; }
        public string Zone { get; set; }
        public string Woreda { get; set; }
        public long StartDate { get; set; }
        public double Area { get; set; }
        public string Upin { get; set; }
        public Guid? ActivityId { get; set; }
        public Guid FarmId { get; set; }
    }


    public class PlanProgressItem
    {
        public Guid farmid { get; set; }
        public string upin { get; set; }
        public double area { get; set; }
        public Guid? rootActivityId { get; set; }
        public Guid? planId { get; set; }
        public Guid? activityId { get; set; }
        public double target { get; set; }
        public string tag { get; set; }
        public double progress { get; set; }
        public long report_time { get; set; }
        public string OpeartorName { get; set; }


    }

    public class JobCreationItem
    {
        public Guid farmId { get; set; }
        public string upin { get; set; }
        public string name { get; set; }
        public string profession { get; set; }
        public string type { get; set; }
        public int male { get; set; }
        public int female { get; set; }
        
    }

    public class LandDevelopmentSummaryItem
    {
        public string location { get; set; }
        public double totalArea { get; set; }
        public double developedArea { get; set; }
        public double undevelopedArea { get; set;  }
        public double efficiency { get; set; }
        //public double undevelopedArea { get {
        //        return totalArea - undevelopedArea;
        //    } }

        //public double efficiency { get {
        //        return Math.Round((developedArea / totalArea) * 100, 2);
        //    } }
    }

    public class InvestmentStatusSummaryItem
    {
        public string location { get; set; }
        public int activeFarms { get; set; } = 0;
        public int developedFarms { get; set; }
        public int inactiveFarms { get { return totalNoOfFarms - activeFarms; } }
        public int totalNoOfFarms { get; set; }

    }

    public class FarmActiveStatus
    {
        public Guid farmId { get; set; }
        public bool active { get; set; }
        public string upin { get; set; }

    }

}
