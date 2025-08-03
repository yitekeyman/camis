using camis.aggregator.data.Entities;
using camis.aggregator.domain.Admin;
using camis.aggregator.domain.Infrastructure;
using camis.aggregator.domain.Infrastructure.Architecture;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
namespace camis.aggregator.domain.Report
{
    public interface IReportService : ICamisService
    {
        void SetSession(UserSession session);
        List<data.Entities.TRegions> GetAllRegions();
        List<data.Entities.TZones> GetZones(string csaregionid);
        List<data.Entities.TWoredas> GetWoredas(string csaworedaid);
        void SetRegionUrl(string regionId, string url, string username, string password);
        void UpdateRegionConfig(RegionConfigModel Model);
        data.Entities.TRegions GetRegion(string code);
        ReportResponseModel GetReport(ReportRequestModel Request);
        List<Farm> GetFarms(string region);

    }

    public class ReportService : CamisService, IReportService
    {
        private UserSession _session;

        public ReportService(aggregatorContext Context)
        {
            base.SetContext(Context);
        }



        public void SetSession(UserSession session)
        {
            _session = session;
        }

        public List<data.Entities.TRegions> GetAllRegions()
        {
            return Context.TRegions.ToList();
        }

        public data.Entities.TRegions GetRegion(string code)
        {
            return Context.TRegions.Where(m => m.Csaregionid == code).First();
        }

        public List<Farm> GetFarms(string region)
        {
            var r = GetRegion(region);
            var CamisInterface = new CamisInterface(r);
            var farms = CamisInterface.GetFarms();
            return farms;
        }

        public ReportResponseModel GetReport(ReportRequestModel Request)
        {
            Dictionary<string, ReportResponseModel> resp = new Dictionary<string, ReportResponseModel>();
            ReportResponseModel response = new ReportResponseModel();
            foreach (var reg in Request.Regions)
            {
                Request.Region = reg;
                var region = GetRegion(reg);
                bool found;
                try
                {
                    var CamisInterface = new CamisInterface(region);
                    response = CamisInterface.GetReport(Request);
                    var d = GetSavedReport(Request, out found);
                    if (found)
                    {
                        Context.ReportData.Remove(d);
                        Context.SaveChanges();
                    }
                    SaveReport(GetReportDataModel(response));
                    
                }
                catch (Exception ex)
                {
                    if (ex is WebException)
                    {
                        var d = GetSavedReport(Request, out found);
                        if (found)
                        {
                            response = GetReportResponseModel(d);
                        }
                        else
                            throw new Exception($"Connection Refused and There is not catched data in DB for region {region.Csaregionnameeng}");
                    }
                    else
                    {
                        throw (ex);
                    }

                }
                resp[reg] = response;
            }
            if(resp.Keys.Count() == 1)
            {
                var key = resp.Keys.ToList()[0];
                return resp[key];

            }
            else
            {
                var r = ConcatentateResponse(resp);
                return r;
            }          
           

        }

        public ReportResponseModel ConcatentateResponse(Dictionary<string,ReportResponseModel> resp)
        {
            ReportResponseModel response = new ReportResponseModel();

            var keys = resp.Keys.ToList();
            response = resp[keys[0]];
            resp.Remove(keys[0]);
            List<ReportResponseModel> rl = resp.Values.ToList();
            rl.ForEach(d =>
            {
                response.LocationLookup.AddRangeNewOnly(d.LocationLookup);
            });
            switch (response.Request.SelectedReportType)
            {
                case ReportTypes.SummaryOfLandByLandStatus:
                    response.LandArea.AddRange(rl.SelectMany(m => m.LandArea));
                    break;
                case ReportTypes.ListOfLands:
                    response.LandList.AddRange(rl.SelectMany(m => m.LandList));
                    break;
                case ReportTypes.ListOfCommercialFarmOwners:
                    response.OperatorList.AddRange(rl.SelectMany(m => m.OperatorList));
                    break;
                case ReportTypes.AnnualLeaseAmountByAdministrativeLocationInvType:
                    response.LeaseSummary.AddRange(rl.SelectMany(m => m.LeaseSummary));
                    break;
                case ReportTypes.LandTransferredAtDifferentTimes:
                    response.RegionAreaSummery.AddRange(rl.SelectMany(m => m.RegionAreaSummery));
                    break;
                case ReportTypes.LandTransferreToInvestorsByFarmSize:
                    response.FarmCount.AddRange(rl.SelectMany(m => m.FarmCount));
                    response.FarmArea.AddRange(rl.SelectMany(m => m.FarmArea));
                    break;
                case ReportTypes.SummaryOfAgricultureInvestmentByOriginOfInvestors:
                    response.OperatorSummary.AddRange(rl.SelectMany(m => m.OperatorSummary));
                    break;
                case ReportTypes.SummaryOfLandByAdministrativeLocationandWaterSource:
                    response.WaterSourceSummary.AddRange(rl.SelectMany(m => m.WaterSourceSummary));
                    break;
                case ReportTypes.SummaryOfSubLeasedLandByLocation:
                    response.FarmLandSummary.AddRange(rl.SelectMany(m => m.FarmLandSummary));
                    break;
                case ReportTypes.CropProductionList:
                    response.CropProductionList.AddRange(rl.SelectMany(m => m.CropProductionList).ToDictionary(m => m.Key, m => m.Value));
                    break;
                case ReportTypes.TimeSeriesInvestorCropProduction:
                    response.InvestorTimeSeriesCropProduction.AddRange(rl.SelectMany(m => m.InvestorTimeSeriesCropProduction).ToDictionary(m => m.Key, m => m.Value));
                    break;
                case ReportTypes.FarmMachineryList:
                    response.MachineryList.AddRange(rl.SelectMany(m => m.MachineryList).ToDictionary(m => m.Key, m => m.Value));
                    break;
                case ReportTypes.CampingFacilitesList:
                    response.CampingFacilityList.AddRange(rl.SelectMany(m => m.CampingFacilityList).ToDictionary(m => m.Key, m => m.Value));
                    break;
                case ReportTypes.CampingFacilitySummary:
                    response.CampSummary.AddRange(rl.SelectMany(m => m.CampSummary));
                    break;
                case ReportTypes.JobCreationListByProfession:
                    response.JobCreationList.AddRange(rl.SelectMany(m => m.JobCreationList));
                    break;
                case ReportTypes.JobCreationListByType:
                    response.JobCreationList.AddRange(rl.SelectMany(m => m.JobCreationList));
                    break;
                case ReportTypes.LandDevelopmentProgressList:
                    response.LandDevelopmentList.AddRange(rl.SelectMany(m => m.LandDevelopmentList));
                    break;
                case ReportTypes.LandDevelopmentProgressSummary:
                    response.LandDevelopmentSummary.AddRange(rl.SelectMany(m => m.LandDevelopmentSummary));
                    break;
                case ReportTypes.InvstmentStatusSummery:
                    response.InvestmentStatusSummary.AddRange(rl.SelectMany(m => m.InvestmentStatusSummary));
                    break;
                case ReportTypes.ContractFarmingList:
                    response.ContractFarmingList.AddRange(rl.SelectMany(m => m.ContractFarmingList));
                    break;
                case ReportTypes.ContractFarmingSummary:
                    response.ContractFarmSummary.AddRange(rl.SelectMany(m => m.ContractFarmSummary));
                    break;
                default:
                    break;
            }
            response.LocationLookup.AddRange(rl.SelectMany(m => m.LocationLookup).ToDictionary(m => m.Key, m => m.Value));



            return response;
        }

        public List<data.Entities.TZones> GetZones(string csaregionid)
        {
            var res = Context.TZones.Where(m => m.Csaregionid == csaregionid).ToList();
            return res;
        }

        public List<data.Entities.TWoredas> GetWoredas(string csaworedaid)
        {
            var res = Context.TWoredas.Where(m => m.NrlaisZoneid == csaworedaid).ToList();
            return res;
        }

        public void SetRegionUrl(string regionId, string regionUrl, string username, string password)
        {
            var report = Context.TRegions.Where(m => m.Csaregionid == regionId).First();
            report.Url = regionUrl;
            report.Username = username;
            report.Password = password;
            Context.SaveChanges(_session.Username, (int)UserActionType.AddRegionUrl);
        }

        public void UpdateRegionConfig(RegionConfigModel Model)
        {
            var reg = Context.TRegions.Where(m => m.Csaregionid == Model.regionid).First();
            reg.Url = Model.url;
            reg.Username = Model.username;
            reg.Password = Model.password;
            Context.SaveChanges(_session.Username, (int)UserActionType.AddRegionUrl);
        }

        public void SaveReport(ReportData data)
        {
            Context.ReportData.Add(data);
            Context.SaveChanges();
        }

        public ReportData GetReportDataModel(ReportResponseModel response)
        {
            ReportData data = new ReportData()
            {
                SummerizedBy = (int)response.Request.SummerizedBy,
                FilteredBy = (int)response.Request.FilteredBy,
                Region = response.Request.Region,
                Zone = response.Request.Zone,
                Woreda = response.Request.Woreda,
                ReportResponse = JsonConvert.SerializeObject(response, SerializerSetting.Get()),
                ReportType = (int)response.Request.SelectedReportType,
                Timestamp = DateTime.Now.Ticks,
                ReportRequest = JsonConvert.SerializeObject(response.Request, SerializerSetting.Get())
            };
            return data;
        }

        public ReportResponseModel GetReportResponseModel(ReportData data)
        {
            return JsonConvert.DeserializeObject<ReportResponseModel>(data.ReportResponse, SerializerSetting.Get());
        }

        public ReportData GetSavedReport(ReportRequestModel Request, out bool found)
        {
            int[] onlyFilteredReports = new int[] { 2, 3, 12, 13,  15, 16, 17,  20 };
            int[] summerizedReports = new int[] { 1, 4, 7, 8, 9 , 14, 18, 19, 21};

            if (onlyFilteredReports.Contains((int)Request.SelectedReportType))
            {
                var r = Context.ReportData.Where(m => m.ReportType == (int)Request.SelectedReportType && m.Region == Request.Region && m.Woreda == Request.Woreda && m.Zone == Request.Zone);
                if(r.Count() > 0)
                {
                    found = true;
                    return  r.OrderBy(m => m.Timestamp).Last();
                    //return JsonConvert.DeserializeObject<ReportResponseModel>(resp.ReportResponse, SerializerSetting.Get());
                }
                else
                {
                    found = false;
                    return new ReportData();
                }                
            }
            
            else if (summerizedReports.Contains((int)Request.SelectedReportType))
            {
                var r = Context.ReportData.Where(m => m.ReportType == (int)Request.SelectedReportType && m.Region == Request.Region && m.Woreda == Request.Woreda && m.Zone == Request.Zone );
                if(Request.SummerizedBy == SummerizedBy.Region || Request.SummerizedBy == 0)
                {
                    r = r.Where(m => m.SummerizedBy == 0 || m.SummerizedBy == (int)SummerizedBy.Region);
                }
                else
                {
                    r = r.Where(m => m.SummerizedBy == (int)SummerizedBy.Region);
                }
                if (r.Count() > 0)
                {
                    found = true;
                    return r.OrderBy(m => m.Timestamp).Last();
                }

                else
                {
                    found = false;
                    return new ReportData();
                }
            }

            else if(Request.SelectedReportType == ReportTypes.LandTransferredAtDifferentTimes || Request.SelectedReportType == ReportTypes.LandTransferreToInvestorsByFarmSize)
            {
                var r = Context.ReportData.Where(m => m.ReportType == (int)Request.SelectedReportType && m.Region == Request.Region && m.Woreda == Request.Woreda && m.Zone == Request.Zone);
                if (Request.SummerizedBy == SummerizedBy.Region || Request.SummerizedBy == 0)
                {
                    r = r.Where(m => m.SummerizedBy == 0 || m.SummerizedBy == (int)SummerizedBy.Region);
                }
                else
                {
                    r = r.Where(m => m.SummerizedBy == (int)SummerizedBy.Region);
                }
                if(r.Count() > 0)
                {
                    var list = r.Select(m => new { id = m.Id , data =  GetReportResponseModel(m) }).ToList().ToDictionary(o => o.id, o => o.data);
                    var ids = new List<int>();
                    if(Request.SelectedReportType == ReportTypes.LandTransferredAtDifferentTimes)
                    {
                        ids = list.Where(m => m.Value.Request.DateEquals(Request.Dates)).Select(m => m.Key).ToList();
                    }
                    else if(Request.SelectedReportType == ReportTypes.LandTransferreToInvestorsByFarmSize)
                    {
                        ids = list.Where(m => m.Value.Request.SizeEquals(Request.FarmSizes)).Select(m => m.Key).ToList();
                    }
                    var resp = r.Where(m => ids.Contains(m.Id)).OrderBy(m => m.Timestamp);
                    if(resp.Count() > 0)
                    {
                        found = true;
                        return resp.OrderBy(m => m.Timestamp).Last();
                    }
                    else
                    {
                        found = false;
                        return new ReportData();
                    }

                }
                else
                {
                    found = false;
                    return new ReportData();
                }
            }
            else if (Request.SelectedReportType == ReportTypes.CropProductionList)
            {
                var r = Context.ReportData.Where(m => m.ReportType == (int)Request.SelectedReportType && m.Region == Request.Region && m.Woreda == Request.Woreda && m.Zone == Request.Zone);
                var list = r.Select(m => new { id = m.Id, data = GetReportResponseModel(m) }).ToList().ToDictionary(o => o.id, o => o.data);
                var ids = list.Where(m => m.Value.Request.FromDate == Request.FromDate && m.Value.Request.EndDate == Request.EndDate).Select(m => m.Key).ToList();
                var resp = r.Where(m => ids.Contains(m.Id));
                if (resp.Count() > 0)
                {
                    found = true;
                    return resp.OrderBy(m => m.Timestamp).Last();
                }
                else
                {
                    found = false;
                    return new ReportData();
                }
            }
            else if (Request.SelectedReportType == ReportTypes.TimeSeriesInvestorCropProduction)
            {
                var r = Context.ReportData.Where(m => m.ReportType == (int)Request.SelectedReportType && m.Region == Request.Region && m.Woreda == Request.Woreda && m.Zone == Request.Zone);
                var list = r.Select(m => new { id = m.Id, data = GetReportResponseModel(m) }).ToList().ToDictionary(o => o.id, o => o.data);
                var ids = list.Where(m => m.Value.Request.StartYear == Request.StartYear && m.Value.Request.EndYear == Request.EndYear && m.Value.Request.FarmId == Request.FarmId).Select(m => m.Key).ToList();
                var resp = r.Where(m => ids.Contains(m.Id));
                if (resp.Count() > 0)
                {
                    found = true;
                    return resp.OrderBy(m => m.Timestamp).Last();
                }
                else
                {
                    found = false;
                    return new ReportData();
                }
            }

            else
            {
                found = false;
                return new ReportData();
            }





        }

        
    }
}
