using intapscamis.camis.domain.Farms.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace intapscamis.camis.domain.Report
{
    public partial class ReportService
    {

        string GetFilterParameter(ReportRequestModel Model)
        {
            switch (Model.FilteredBy)
            {
                case FilteredBy.None:
                    return Model.Region;
                case FilteredBy.Region:
                    return Model.Region;
                case FilteredBy.Zone:
                    return Model.Zone;
                case FilteredBy.Woreda:
                    return Model.Woreda;
                default:
                    return "";
            }
        }


        public string GetFilterSqlCommand2(ReportRequestModel Request)
        {
            var condition = " ";
            if (Request.FilteredBy == FilteredBy.Region)
            {
                condition = $" and split_part(lu.upin,'/',1) = @filter ";
            }
            else if (Request.FilteredBy == FilteredBy.Zone)
            {
                condition = $" and  CONCAT(split_part(lu.upin,'/',1),'/',split_part(lu.upin,'/',2))  = @filter ";
            }
            else if (Request.FilteredBy == FilteredBy.Woreda)
            {
                condition = $" and CONCAT(split_part(lu.upin,'/',1),'/',split_part(lu.upin,'/',2),'/',split_part(lu.upin,'/',3)) = @filter ";
            }
            return condition;


        }

        public string GetSqlQueryString(ReportRequestModel Request, bool toDate, bool filter, bool farmId, bool fromDate = false)
        {
            var condition = "";
            if (filter)
            {
                condition = GetFilterSqlCommand2(Request);
            }

            var date = "";
            if (toDate)
            {
                date = " where report_time <= @todate ";
            }
            var from = "";
            if (fromDate)
            {
                from = " and report_time >= @fromDate";
            }
            var farm = " ";
            if (farmId)
            {
                farm = " and fa.id = @farmid ";
            }

            var sql = $@"
select at.farmid, at.upin, at.area, at.root_activity_id, at.plan_id, at.actid, at.target, at.tag, at.oeprator_name, k.activity_id, k.progress, k.rt
                    from (select fa.id farmid, lu.upin upin, (lu.area/10000) area ,ap.root_activity_id root_activity_id, ap.id plan_id, 
                    apd.activity_id actid, apd.target target, apd.tag tag, fo.name oeprator_name, apd.variable_id vi from frm.farm fa left outer join frm.farm_land fl on fa.id = fl.farm_id inner join frm.farm_operator fo on fa.operator_id = fo.id left outer join
                    lb.land_upin lu on fl.land_id = lu.land_id left outer join pm.activity_plan ap on fa.activity_id = ap.root_activity_id left outer join 
                    pm.activity_plan_detail apd on ap.id = apd.plan_id where tag like @tag  {condition} {farm} ) at left outer join(select prog.activity_id, prog.progress progress, 
                    report.report_time rt, prog.variable_id from pm.activity_progress prog inner join pm.activity_progress_report report on prog.report_id = report.id where progress > 0 and
                    report_time <= (select Max(prog.report_time) reptime from (select prog.activity_id, prog.progress, report.report_time, prog.variable_id from pm.activity_progress 
                    prog inner join pm.activity_progress_report report on prog.report_id = report.id {date} {from} ) prog) ) k on at.actid = k.activity_id and at.vi = k.variable_id
                    order by at.farmid, at.tag, k.rt
";


            return sql;
        }


        public double GetActivityProgress(Guid activityid, long time)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var sql = " select time, progress from pm.activity_progress where time < @time and activity_id = @activity_id ;";
            List<PlanProgressItem> prog = new List<PlanProgressItem>();
            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var t = cmd.Parameters.Add("@time", NpgsqlTypes.NpgsqlDbType.Bigint);
            var act_id = cmd.Parameters.Add("@activity_id", NpgsqlTypes.NpgsqlDbType.Uuid);

            cmd.Prepare();

            t.Value = time;
            act_id.Value = activityid;
            using (var dr = cmd.ExecuteReader())
            {
                double tempProgress = 0;
                while (dr.Read())
                {
                    prog.Add(new PlanProgressItem()
                    {
                        report_time = long.TryParse(dr[0].ToString(), out time) ? time : -1,
                        progress = double.TryParse(dr[1].ToString(), out tempProgress) ? tempProgress : 0,
                    });
                }
            }
            if (prog.Count() > 0)
            {
                var maxtime = prog.Count() > 0 ? prog.Max(M => M.report_time) : 0;
                var record = prog.Where(m => m.report_time == maxtime);
                if (record.Count() > 0)
                {
                    return record.LastOrDefault().progress;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }

        public List<PlanProgressItem> GetContractFarmingProgressItems(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var condition = GetFilterSqlCommand2(Request);
            var sql = GetSqlQueryString(Request, false, true, false);
            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var tag = cmd.Parameters.Add("@tag", NpgsqlTypes.NpgsqlDbType.Varchar);
            var filter = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);

            cmd.Prepare();
            filter.Value = GetFilterParameter(Request);
            var tags = new string[] { "contract-farming%" };

            List<PlanProgressItem> Productions = new List<PlanProgressItem>();
            foreach (var t in tags)
            {
                tag.Value = t;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Guid temp = new Guid();
                        double temp2 = 0;
                        double tempProgress = -1;
                        long time = 1;
                        double areatemp = 0;
                        Productions.Add(new PlanProgressItem()
                        {
                            farmid = Guid.Parse(dr[0].ToString()),
                            upin = String.IsNullOrWhiteSpace(dr[1].ToString()) ? "" : dr[1].ToString(),
                            area = double.TryParse(dr[2].ToString(), out areatemp) ? areatemp : 0,
                            rootActivityId = Guid.TryParse(dr[3].ToString(), out temp) ? (Guid?)temp : null,
                            planId = Guid.TryParse(dr[4].ToString(), out temp) ? (Guid?)temp : null,
                            activityId = Guid.TryParse(dr[5].ToString(), out temp) ? (Guid?)temp : null,
                            target = double.TryParse(dr[6].ToString(), out temp2) ? temp2 : -1,
                            tag = dr[7].ToString(),
                            OpeartorName = dr[8].ToString(),
                            progress = double.TryParse(dr[10].ToString(), out tempProgress) ? tempProgress : -1,
                            report_time = long.TryParse(dr[11].ToString(), out time) ? time : -1

                        }
                        );
                    }
                }


            }

            Context.Database.CloseConnection();
            return Productions;
        }

        public ReportResponseModel GetContractFarmingSummary(ReportRequestModel Request)
        {
            var Production = GetContractFarmingProgressItems(Request);
            var location =  new List<string>();
            switch (Request.SummerizedBy)
            {
                case SummerizedBy.Region:
                    location = Production.Select(m => m.upin.Region()).Distinct().ToList();
                    break;
                case SummerizedBy.Zone:
                    location = Production.Select(m => m.upin.Zone()).Distinct().ToList();
                    break;
                case SummerizedBy.Woreda:
                    location = Production.Select(m => m.upin.Woreda()).Distinct().ToList();
                    break;
                default:
                    location = Production.Select(m => m.upin.Region()).Distinct().ToList();
                    break;
            }


            List<ContractFarmSummaryModel> Summary = new List<ContractFarmSummaryModel>();

            foreach (var l in location)
            {
                var pis = new List<PlanProgressItem>();
                switch (Request.SummerizedBy)
                {
                    case SummerizedBy.Region:
                        pis = Production.Where(m => m.upin.Region() == l).ToList();
                        break;
                    case SummerizedBy.Zone:
                        pis = Production.Where(m => m.upin.Zone() == l).ToList();
                        break;
                    case SummerizedBy.Woreda:
                        pis = Production.Where(m => m.upin.Woreda() == l).ToList();
                        break;
                    default:
                        pis = Production.Where(m => m.upin.Region() == l).ToList();
                        break;
                }

                var farms = Production.Select(m => m.farmid).Distinct().ToList();
                var upins = pis.Select(m => m.tag.GetUpin()).Distinct().ToList();
                var uniqueTags = pis.Select(m => m.tag.GetTag()).Distinct().ToList();
                var inputTags = uniqueTags.Where(m => m.Substring(17, 5).ToLower() == "input").ToList();
                var outputTags = uniqueTags.Where(m => m.Substring(17, 6).ToLower() == "output").ToList();
                List<ProgressItem> input = new List<ProgressItem>();
                List<ProgressItem> output = new List<ProgressItem>();
                foreach (var it in inputTags)
                {
                    var p = pis.Where(m => m.tag.GetTag() == it).Sum(m => m.progress);
                    input.Add(new ProgressItem() { tag = it, progress = p });
                }
                foreach (var ot in outputTags)
                {
                    var p = pis.Where(m => m.tag.GetTag() == ot).Sum(m => m.progress);
                    output.Add(new ProgressItem() { tag = ot, progress = p });
                }
                Summary.Add(new ContractFarmSummaryModel()
                {
                    location = l,
                    numberOfFarms = farms.Count(),
                    numberOfContractors = upins.Count(),
                    input = input,
                    output = output
                });
            }


            string loc = "";
            var lookup = GetLocationNames(Summary.Select(m => m.location).ToList(), out loc);

            return new ReportResponseModel()
            {
                Request = Request,
                ContractFarmSummary = Summary,
                AddressType = loc,
                LocationLookup = lookup
            };

        }

        public ReportResponseModel GetContractFarmingList(ReportRequestModel Request)
        {
            var Productions = GetContractFarmingProgressItems(Request);
            List<ContractFarmModel> ContractFarming = new List<ContractFarmModel>();
            var uniqueFarmsId = Productions.Select(m => m.farmid).Distinct().ToArray();
            foreach (var id in uniqueFarmsId)
            {

                var ppi = Productions.Where(m => m.farmid == id).ToList();
                if (ppi.Count() > 0)
                {
                    ContractFarmModel cfm = new ContractFarmModel()
                    {
                        farmId = ppi[0].farmid,
                        farmName = ppi[0].OpeartorName,
                        upin = ppi[0].upin,
                        area = ppi[0].area,
                        woreda = ppi[0].upin.Woreda(),
                        //woreda = "",
                        ContractedFarms = new List<ContractedFarmModel>()
                    };
                    var uniqueTags = ppi.Select(m => m.tag).Distinct().ToList();
                    var contractedFarmUpins = uniqueTags.Select(m => m.GetUpin()).Distinct().ToList();
                    foreach (var upin in contractedFarmUpins)
                    {
                        var input_pi = ppi.Where(m => m.tag.Substring(17, 5).ToLower() == "input" && m.tag.GetUpin() == upin).ToList();
                        var output_pi = ppi.Where(m => m.tag.Substring(17, 6).ToLower() == "output" && m.tag.GetUpin() == upin).ToList();
                        ContractedFarmModel cf = new ContractedFarmModel()
                        {
                            upin = upin,
                            Input = input_pi,
                            Output = output_pi
                        };
                        cfm.ContractedFarms.Add(cf);
                    }
                    ContractFarming.Add(cfm);
                }


            }


            string loc = "";
            var lookup = GetLocationNames(ContractFarming.Select(m => m.woreda).ToList(), out loc);
            //var lookup = new Dictionary<string, string>();
            return new ReportResponseModel()
            {
                Request = Request,
                ContractFarmingList = ContractFarming,
                AddressType = loc,
                LocationLookup = lookup
            };
        }


        public ReportResponseModel GetCropProductionList(ReportRequestModel Model)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var condition = GetFilterSqlCommand2(Model);
            var sql = GetSqlQueryString(Model, true, true, false);

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var tag = cmd.Parameters.Add("@tag", NpgsqlTypes.NpgsqlDbType.Varchar);
            var toDate = cmd.Parameters.Add("@toDate", NpgsqlTypes.NpgsqlDbType.Bigint);
            var filter = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);

            //cmd.Prepare();

            filter.Value = GetFilterParameter(Model);
            var tags = new string[] { "production-crop-%", "coverage-crop-%" };
            List<PlanProgressItem> Productions = new List<PlanProgressItem>();
            foreach (var t in tags)
            {
                tag.Value = t;
                toDate.Value = ((DateTime)Model.EndDate).Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000;



                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Guid temp = new Guid();
                        double temp2 = 0;
                        double tempProgress = -1;
                        long time = 1;
                        double areatemp = 0;
                        Productions.Add(new PlanProgressItem()
                        {
                            farmid = Guid.Parse(dr[0].ToString()),
                            upin = String.IsNullOrWhiteSpace(dr[1].ToString()) ? "" : dr[1].ToString(),
                            area = double.TryParse(dr[2].ToString(), out areatemp) ? areatemp : 0,
                            rootActivityId = Guid.TryParse(dr[3].ToString(), out temp) ? (Guid?)temp : null,
                            planId = Guid.TryParse(dr[4].ToString(), out temp) ? (Guid?)temp : null,
                            activityId = Guid.TryParse(dr[5].ToString(), out temp) ? (Guid?)temp : null,
                            target = double.TryParse(dr[6].ToString(), out temp2) ? temp2 : -1,
                            tag = dr[7].ToString(),
                            OpeartorName = dr[8].ToString(),
                            progress = double.TryParse(dr[10].ToString(), out tempProgress) ? tempProgress : -1,
                            report_time = long.TryParse(dr[11].ToString(), out time) ? time : -1

                        }
                        );
                    }
                }


            }

            //var fromDate = (long)((DateTime)Model.FromDate).Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000;

            Context.Database.CloseConnection();

            Dictionary<Guid, List<PlanProgressItem>> res = new Dictionary<Guid, List<PlanProgressItem>>();

            var farmIds = Productions.Select(m => m.farmid).Distinct().ToList();
            foreach (var item in farmIds)
            {
                var items = Productions.Where(m => m.progress != -1 && m.activityId != null && m.farmid == item).ToList();
                List<PlanProgressItem> performance = FilterProgressItems(items);
                //  performance = ProcessProductionProgress(performance,fromDate);
                res.Add(item, performance);
            }

            string loc = "";
            var lookup = GetLocationNames(Productions.Select(m => m.upin.Woreda()).Distinct().ToList(), out loc);


            return new ReportResponseModel()
            {
                Request = Model,
                CropProductionList = res,
                LocationLookup = lookup,
                AddressType = loc
            };
        }

        List<PlanProgressItem> ProcessProductionProgress(List<PlanProgressItem> prog, long time)
        {
            List<PlanProgressItem> items = new List<PlanProgressItem>();
            foreach (var item in prog)
            {
                if (item.tag.Substring(0, 10) == "production")
                {
                    var previousProg = GetActivityProgress((Guid)item.activityId, time);
                    var prod = item.progress - previousProg;
                    item.progress = prod;
                    items.Add(item);
                }
                else
                {
                    items.Add(item);
                }
            }
            return items;
        }

        List<PlanProgressItem> FilterProgressItems(List<PlanProgressItem> items)
        {
            List<PlanProgressItem> performance = new List<PlanProgressItem>();
            var uniqueTags = items.Select(m => m.tag).Distinct().ToList();
            var uniqueFarms = items.Select(m => m.farmid).Distinct().ToList();
            foreach (var t in uniqueTags)
            {
                foreach (var id in uniqueFarms)
                {
                    var x = items.Where(m => m.tag == t && m.farmid == id).Max(m => m.report_time);

                    performance.Add(items.Where(m => m.tag == t && m.report_time == x && m.farmid == id).Last());
                }

            }
            return performance;
        }


        public ReportResponseModel GetTimeSeriesInvestorCropProduction(ReportRequestModel Model)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();


            var sql = GetSqlQueryString(Model, true, false, true, true);

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var tag = cmd.Parameters.Add("@tag", NpgsqlTypes.NpgsqlDbType.Varchar);
            var toDate = cmd.Parameters.Add("@toDate", NpgsqlTypes.NpgsqlDbType.Bigint);
            var fromDate = cmd.Parameters.Add("@fromDate", NpgsqlTypes.NpgsqlDbType.Bigint);
            var farmId = cmd.Parameters.Add("@farmid", NpgsqlTypes.NpgsqlDbType.Uuid);

            cmd.Prepare();

            Dictionary<int, List<PlanProgressItem>> InvestorCropProduction = new Dictionary<int, List<PlanProgressItem>>();

            var tags = new string[] { "production-crop-%", "coverage-crop-%" };
            foreach (var t in tags)
            {
                tag.Value = t;
                farmId.Value = Model.FarmId;


                for (int i = Model.StartYear; i <= Model.EndYear; i++)
                {
                    if (!InvestorCropProduction.ContainsKey(i))
                    {
                        InvestorCropProduction.Add(i, new List<PlanProgressItem>());
                    }

                    toDate.Value = new DateTime(i, 12, 31).Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000;
                    fromDate.Value = new DateTime(i, 1, 1).Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000;
                    List<PlanProgressItem> Productions = new List<PlanProgressItem>();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Guid temp = new Guid();
                            double temp2 = 0;
                            double tempProgress = -1;
                            long time = 1;
                            double areatemp = 0;
                            Productions.Add(new PlanProgressItem()
                            {
                                farmid = Guid.Parse(dr[0].ToString()),
                                upin = dr[1].ToString(),
                                area = double.TryParse(dr[2].ToString(), out areatemp) ? areatemp : 0,
                                rootActivityId = Guid.TryParse(dr[3].ToString(), out temp) ? (Guid?)temp : null,
                                planId = Guid.TryParse(dr[4].ToString(), out temp) ? (Guid?)temp : null,
                                activityId = Guid.TryParse(dr[5].ToString(), out temp) ? (Guid?)temp : null,
                                target = double.TryParse(dr[6].ToString(), out temp2) ? temp2 : -1,
                                tag = dr[7].ToString(),
                                OpeartorName = dr[8].ToString(),
                                progress = double.TryParse(dr[10].ToString(), out tempProgress) ? tempProgress : -1,
                                report_time = long.TryParse(dr[11].ToString(), out time) ? time : -1

                            }
                            );
                        }
                    }



                    var items = Productions.Where(m => m.progress != -1 && m.activityId != null).ToList();
                    InvestorCropProduction[i].AddRange(FilterProgressItems(items));

                }

            }


            Context.Database.CloseConnection();
            //new DateTime()

            return new ReportResponseModel()
            {
                Request = Model,
                InvestorTimeSeriesCropProduction = InvestorCropProduction
            };
        }

        public ReportResponseModel GetFarmMachineryList(ReportRequestModel Model)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();


            var sql = GetSqlQueryString(Model, false, true, false); var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var tag = cmd.Parameters.Add("@tag", NpgsqlTypes.NpgsqlDbType.Varchar);
            var filter = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);

            cmd.Prepare();


            tag.Value = "asset-machinery-%";
            filter.Value = GetFilterParameter(Model);
            List<PlanProgressItem> Productions = new List<PlanProgressItem>();

            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Guid temp = new Guid();
                double temp2 = 0;
                double tempProgress = -1;
                long time = 1;
                double areatemp = 0;
                Productions.Add(new PlanProgressItem()
                {
                    farmid = Guid.Parse(dr[0].ToString()),
                    upin = String.IsNullOrWhiteSpace(dr[1].ToString()) ? "" : dr[1].ToString(),
                    area = double.TryParse(dr[2].ToString(), out areatemp) ? areatemp : 0,
                    rootActivityId = Guid.TryParse(dr[3].ToString(), out temp) ? (Guid?)temp : null,
                    planId = Guid.TryParse(dr[4].ToString(), out temp) ? (Guid?)temp : null,
                    activityId = Guid.TryParse(dr[5].ToString(), out temp) ? (Guid?)temp : null,
                    target = double.TryParse(dr[6].ToString(), out temp2) ? temp2 : -1,
                    tag = dr[7].ToString(),
                    OpeartorName = dr[8].ToString(),
                    progress = double.TryParse(dr[10].ToString(), out tempProgress) ? tempProgress : -1,
                    report_time = long.TryParse(dr[11].ToString(), out time) ? time : -1

                }
                );
            }

            Context.Database.CloseConnection();

            Dictionary<Guid, List<PlanProgressItem>> res = new Dictionary<Guid, List<PlanProgressItem>>();

            var farmIds = Productions.Select(m => m.farmid).Distinct().ToList();
            foreach (var item in farmIds)
            {
                var items = Productions.Where(m => m.progress != -1 && m.activityId != null && m.farmid == item).ToList();
                res.Add(item, FilterProgressItems(items));
            }


            string loc = "";
            var lookup = GetLocationNames(Productions.Select(m => m.upin.Woreda()).Distinct().ToList(), out loc);

            return new ReportResponseModel()
            {
                Request = Model,
                MachineryList = res,
                LocationLookup = lookup,
                AddressType = loc
            };
        }

        public ReportResponseModel GetCampingFacilitiesList(ReportRequestModel Model, bool useFilter = true)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var condition = GetFilterSqlCommand2(Model);
            var sql = "";
            if (useFilter)
                sql = GetSqlQueryString(Model, false, true, false);
            else
                sql = GetSqlQueryString(Model, false, false, false);


            var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            var s = new Npgsql.NpgsqlParameter();
            if (useFilter) {
                s = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);
            }
            var tag = cmd.Parameters.Add("@tag", NpgsqlTypes.NpgsqlDbType.Varchar);


            cmd.Prepare();

            if (useFilter)
                s.Value = GetFilterParameter(Model);

            tag.Value = "asset-camping-area%";

            List<PlanProgressItem> Productions = new List<PlanProgressItem>();

            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Guid temp = new Guid();
                double temp2 = 0;
                double tempProgress = -1;
                long time = 1;
                double areatemp = 0;
                Productions.Add(new PlanProgressItem()
                {
                    farmid = Guid.Parse(dr[0].ToString()),
                    upin = dr[1].ToString(),
                    area = double.TryParse(dr[2].ToString(), out areatemp) ? areatemp : 0,
                    rootActivityId = Guid.TryParse(dr[3].ToString(), out temp) ? (Guid?)temp : null,
                    planId = Guid.TryParse(dr[4].ToString(), out temp) ? (Guid?)temp : null,
                    activityId = Guid.TryParse(dr[5].ToString(), out temp) ? (Guid?)temp : null,
                    target = double.TryParse(dr[6].ToString(), out temp2) ? temp2 : -1,
                    tag = dr[7].ToString(),
                    OpeartorName = dr[8].ToString(),
                    progress = double.TryParse(dr[10].ToString(), out tempProgress) ? tempProgress : -1,
                    report_time = long.TryParse(dr[11].ToString(), out time) ? time : -1

                }
                );
            }

            Context.Database.CloseConnection();

            Dictionary<Guid, List<PlanProgressItem>> res = new Dictionary<Guid, List<PlanProgressItem>>();

            var farmIds = Productions.Select(m => m.farmid).Distinct().ToList();
            foreach (var item in farmIds)
            {
                var items = Productions.Where(m => m.progress != -1 && m.activityId != null && m.farmid == item).ToList();
                items = FilterProgressItems(items);
                res.Add(item, items);
            }


            string loc = "";
            var lookup = GetLocationNames(Productions.Select(m => m.upin.Woreda()).Distinct().ToList(), out loc);

            return new ReportResponseModel()
            {
                Request = Model,
                CampingFacilityList = res,
                LocationLookup = lookup,
                AddressType = loc
            };
        }


        internal class CampSummaryItem
        {
            public string location { get; set; }
            public int count { get; set; }

        }

        public ReportResponseModel GetCampingFacilitySummary(ReportRequestModel Model)
        {
           

            var list = GetCampingFacilitiesList(Model, false).CampingFacilityList;

            var farms = GetFarmListWithLand(Model).FarmListReport;

            IEnumerable<CampSummaryItem> s = new List<CampSummaryItem>();

            var summerizer = 8;
            if (Model.SummerizedBy == SummerizedBy.Region)
                s = from b in farms group b by b.Upin.Region() into g select new CampSummaryItem() { location = g.Key, count = g.Count() };
            else if (Model.SummerizedBy == SummerizedBy.Zone)
            {
                s = from b in farms group b by b.Upin.Zone() into g select new CampSummaryItem() { location = g.Key, count = g.Count() };
            }
            else if (Model.SummerizedBy == SummerizedBy.Woreda)
            {
                s = from b in farms group b by b.Upin.Woreda() into g select new CampSummaryItem() { location = g.Key, count = g.Count() };
            }
            else
            {
                s = from b in farms group b by b.Upin.Region() into g select new CampSummaryItem() { location = g.Key, count = g.Count() };

            }


            List<CampingFacilitySummary> Summary = new List<CampingFacilitySummary>();
            var k = list.SelectMany(m => m.Value).Distinct().ToList();
            foreach (var i in s)
            {
                var num = 0;
                if (Model.SummerizedBy == SummerizedBy.Region)
                    num = k.Where(m => m.upin.Region() == i.location).Select(m => m.farmid).Distinct().Count();
                else if (Model.SummerizedBy == SummerizedBy.Zone)
                {
                    num = k.Where(m => m.upin.Zone() == i.location).Select(m => m.farmid).Distinct().Count();
                }
                else if (Model.SummerizedBy == SummerizedBy.Woreda)
                {
                    num = k.Where(m => m.upin.Woreda() == i.location).Select(m => m.farmid).Distinct().Count();
                }
                else
                    num = k.Where(m => m.upin.Region() == i.location).Select(m => m.farmid).Distinct().Count();
                Summary.Add(new CampingFacilitySummary()
                {
                    Location = i.location,
                    farmsWithCamps = num,
                    farmsWithoutCamps = i.count - num

                });
            }


            string loc = "";
            var lookup = GetLocationNames(Summary.Select(m => m.Location).ToList(), out loc);

            return new ReportResponseModel()
            {
                CampSummary = Summary,
                Request = Model,
                LocationLookup = lookup,
                AddressType = loc
            };

        }

        public ReportResponseModel GetJobCreationListByProfession(ReportRequestModel Request)
        {

            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();


            var sql = GetSqlQueryString(Request, false, false, false);

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var tag = cmd.Parameters.Add("@tag", NpgsqlTypes.NpgsqlDbType.Varchar);

            cmd.Prepare();

            tag.Value = "job-professional-%";

            List<PlanProgressItem> Productions = new List<PlanProgressItem>();

            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Guid temp = new Guid();
                double temp2 = 0;
                double tempProgress = -1;
                long time = 1;
                double areatemp = 0;
                Productions.Add(new PlanProgressItem()
                {
                    farmid = Guid.Parse(dr[0].ToString()),
                    upin = dr[1].ToString(),
                    area = double.TryParse(dr[2].ToString(), out areatemp) ? areatemp : 0,
                    rootActivityId = Guid.TryParse(dr[3].ToString(), out temp) ? (Guid?)temp : null,
                    planId = Guid.TryParse(dr[4].ToString(), out temp) ? (Guid?)temp : null,
                    activityId = Guid.TryParse(dr[5].ToString(), out temp) ? (Guid?)temp : null,
                    target = double.TryParse(dr[6].ToString(), out temp2) ? temp2 : -1,
                    tag = dr[7].ToString(),
                    OpeartorName = dr[8].ToString(),
                    progress = double.TryParse(dr[10].ToString(), out tempProgress) ? tempProgress : -1,
                    report_time = long.TryParse(dr[11].ToString(), out time) ? time : -1

                }
                );
            }
            Productions = FilterProgressItems(Productions);
            Context.Database.CloseConnection();

            List<JobCreationItem> JobList = new List<JobCreationItem>();
            //Productions = Productions.Where(m => m.progress != -1 && m.activityId != null).ToList();
            var professions = Productions.Select(m => m.tag.Substring(24).ToLower().Split()[0]).ToList();
            var farmIds = Productions.Select(m => m.farmid).Distinct().ToList();
            professions = professions.Distinct().ToList();
            foreach (var farm in farmIds)
            {
                foreach (var p in professions)
                {
                    var femalePermTag = $"job-professional-perm-f-{p}";
                    var femaleTempTag = $"job-professional-temp-f-{p}";
                    var malePerTag = $"job-professional-perm-m-{p}";
                    var maleTempTab = $"job-professional-temp-m-{p}";
                    JobList.Add(new JobCreationItem()
                    {
                        farmId = farm,
                        upin = Productions.Where(m => m.farmid == farm).First().upin,
                        name = Productions.Where(m => m.farmid == farm).First().OpeartorName,
                        profession = p,
                        male = Productions.Where(m => m.farmid == farm && (m.tag.Split()[0] == malePerTag || m.tag == maleTempTab)).Count(),
                        female = Productions.Where(m => m.farmid == farm && (m.tag.Split()[0] == femalePermTag || m.tag == femaleTempTag)).Count()

                    });
            }
            }


            string loc = "";
            var lookup = GetLocationNames(Productions.Select(m => m.upin.Woreda()).Distinct().ToList(), out loc);

            return new ReportResponseModel()
            {
                Request = Request,
                JobCreationList = JobList,
                LocationLookup = lookup,
                AddressType = loc
            };

        }

        public ReportResponseModel GetJobCreationListByType(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();


            var sql = GetSqlQueryString(Request, false, false, false);

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var tag = cmd.Parameters.Add("@tag", NpgsqlTypes.NpgsqlDbType.Varchar);

            cmd.Prepare();

            tag.Value = "job-professional-%";

            List<PlanProgressItem> Productions = new List<PlanProgressItem>();

            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {  
                Guid temp = new Guid();
                double temp2 = 0;
                double tempProgress = -1;
                long time = 1;
                double areatemp = 0;
                Productions.Add(new PlanProgressItem()
                {
                    farmid = Guid.Parse(dr[0].ToString()),
                    upin = dr[1].ToString(),
                    area = double.TryParse(dr[2].ToString(), out areatemp) ? areatemp : 0,
                    rootActivityId = Guid.TryParse(dr[3].ToString(), out temp) ? (Guid?)temp : null,
                    planId = Guid.TryParse(dr[4].ToString(), out temp) ? (Guid?)temp : null,
                    activityId = Guid.TryParse(dr[5].ToString(), out temp) ? (Guid?)temp : null,
                    target = double.TryParse(dr[6].ToString(), out temp2) ? temp2 : -1,
                    tag = dr[7].ToString(),
                    OpeartorName = dr[8].ToString(),
                    progress = double.TryParse(dr[10].ToString(), out tempProgress) ? tempProgress : -1,
                    report_time = long.TryParse(dr[11].ToString(), out time) ? time : -1

                }
                );
            }
            Productions = FilterProgressItems(Productions);
            Context.Database.CloseConnection();

            List<JobCreationItem> JobList = new List<JobCreationItem>();

            var types = new string[] { "perm", "temp" };
            var farmIds = Productions.Select(m => m.farmid).Distinct().ToList();
            foreach (var farm in farmIds)
            {
                foreach (var p in types)
                {
                    var maleTag = $"job-professional-{p}-m";
                    var femaleTag = $"job-professional-{p}-f";
                   
                    JobList.Add(new JobCreationItem()
                    {
                        farmId = farm,
                        upin = Productions.Where(m => m.farmid == farm).First().upin,
                        name = Productions.Where(m => m.farmid == farm).First().OpeartorName,
                        type = p,
                        male = Productions.Where(m => m.farmid == farm && m.tag.Substring(0,23) == maleTag).Count(),
                        female = Productions.Where(m => m.farmid == farm && m.tag.Substring(0,23) == femaleTag).Count()

                    });
                }
            }


            string loc = "";
            var lookup = GetLocationNames(Productions.Select(m => m.upin.Woreda()).Distinct().ToList(), out loc);

            return new ReportResponseModel()
            {
                Request = Request,
                JobCreationList = JobList,
                LocationLookup = lookup,
                AddressType = loc
            };
        }

        public ReportResponseModel GetLandDevelopmentProgressList(ReportRequestModel Request,bool filtered = false)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();


            var sql = GetSqlQueryString(Request, false, filtered, false);

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var tag = cmd.Parameters.Add("@tag", NpgsqlTypes.NpgsqlDbType.Varchar);
            var filter = new Npgsql.NpgsqlParameter();
            if(filtered)
                filter = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);
            cmd.Prepare();

            tag.Value = "land-development%";
            if (filtered)
                filter.Value = GetFilterParameter(Request);
            List<PlanProgressItem> Productions = new List<PlanProgressItem>();

            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Guid temp = new Guid();
                double temp2 = 0;
                double tempProgress = -1;
                long time = 1;
                double areatemp = 0;
                Productions.Add(new PlanProgressItem()
                {
                    farmid = Guid.Parse(dr[0].ToString()),
                    upin = dr[1].ToString(),
                    area = double.TryParse(dr[2].ToString(), out areatemp) ? areatemp : 0,
                    rootActivityId = Guid.TryParse(dr[3].ToString(), out temp) ? (Guid?)temp : null,
                    planId = Guid.TryParse(dr[4].ToString(), out temp) ? (Guid?)temp : null,
                    activityId = Guid.TryParse(dr[5].ToString(), out temp) ? (Guid?)temp : null,
                    target = double.TryParse(dr[6].ToString(), out temp2) ? temp2 : -1,
                    tag = dr[7].ToString(),
                    OpeartorName = dr[8].ToString(),
                    progress = double.TryParse(dr[10].ToString(), out tempProgress) ? tempProgress : -1,
                    report_time = long.TryParse(dr[11].ToString(), out time) ? time : -1

                }
                );
            }
            Productions = FilterProgressItems(Productions);
            Context.Database.CloseConnection();

            string loc = "";
            var lookup = GetLocationNames(Productions.Select(m => m.upin.Woreda()).Distinct().ToList(), out loc);


            return new ReportResponseModel()
            {
                Request = Request,
                LandDevelopmentList = Productions,
                LocationLookup = lookup,
                AddressType = loc
            };
        }

        public ReportResponseModel GetLandDevelopmentProgressSummary(ReportRequestModel Request)
        {
            var developmentList = GetLandDevelopmentProgressList(Request,true).LandDevelopmentList;
            var farms = GetFarmListWithLand(Request).FarmListReport;

            IEnumerable<LandDevelopmentSummaryItem> s = new List<LandDevelopmentSummaryItem>();

            if (Request.SummerizedBy == SummerizedBy.Region)
            {
                s = from b in farms
                    group b by b.Upin.Region() into g
                    select new LandDevelopmentSummaryItem
                    {
                        location = g.Key,
                        totalArea = g.Sum(m => m.Area),
                        developedArea = developmentList.Where(m => m.upin.Region() == g.Key).Sum(m => m.progress),
                        undevelopedArea = g.Sum(m => m.Area) - (developmentList.Where(m => m.upin.Region() == g.Key).Sum(m => m.progress)),
                        efficiency = Math.Round(((developmentList.Where(m => m.upin.Region() == g.Key).Sum(m => m.progress))/(g.Sum(m => m.Area))) * 100, 2)
                    };
            }

            else if (Request.SummerizedBy == SummerizedBy.Zone)
            {
                s = from b in farms
                    group b by b.Upin.Zone() into g
                    select new LandDevelopmentSummaryItem
                    {
                        location = g.Key,
                        totalArea = g.Sum(m => m.Area),
                        developedArea = developmentList.Where(m => m.upin.Zone() == g.Key).Sum(m => m.progress),
                        undevelopedArea = g.Sum(m => m.Area) - (developmentList.Where(m => m.upin.Zone() == g.Key).Sum(m => m.progress)),
                        efficiency = Math.Round(((developmentList.Where(m => m.upin.Zone() == g.Key).Sum(m => m.progress)) / (g.Sum(m => m.Area))) * 100, 2)
                    };
            }
            else if (Request.SummerizedBy == SummerizedBy.Woreda)
            {
                s = from b in farms
                    group b by b.Upin.Woreda() into g
                    select new LandDevelopmentSummaryItem
                    {
                        location = g.Key,
                        totalArea = g.Sum(m => m.Area),
                        developedArea = developmentList.Where(m => m.upin.Woreda() == g.Key).Sum(m => m.progress),
                        undevelopedArea = g.Sum(m => m.Area) - (developmentList.Where(m => m.upin.Woreda() == g.Key).Sum(m => m.progress)),
                        efficiency = Math.Round(((developmentList.Where(m => m.upin.Woreda() == g.Key).Sum(m => m.progress)) / (g.Sum(m => m.Area))) * 100, 2)
                    };
            }
            else
            {
                s = from b in farms
                    group b by b.Upin.Region() into g
                    select new LandDevelopmentSummaryItem
                    {
                        location = g.Key,
                        totalArea = g.Sum(m => m.Area),
                        developedArea = developmentList.Where(m => m.upin.Region() == g.Key).Sum(m => m.progress),
                        undevelopedArea = g.Sum(m => m.Area) - (developmentList.Where(m => m.upin.Region() == g.Key).Sum(m => m.progress)),
                        efficiency = Math.Round(((developmentList.Where(m => m.upin.Region() == g.Key).Sum(m => m.progress)) / (g.Sum(m => m.Area))) * 100, 2)

                    };
            }


 


            string loc = "";
            var lookup = GetLocationNames(s.Select(m => m.location).ToList(), out loc);

            return new ReportResponseModel()
            {
                Request = Request,
                LandDevelopmentSummary = s.ToList(),
                AddressType = loc,
                LocationLookup = lookup
            };
        }

        public ReportResponseModel GetInvestmentStatusByAdministrativeLoaction(ReportRequestModel Request)
        {


            var developmentList = GetLandDevelopmentProgressList(Request, true).LandDevelopmentList;
            var farms = GetFarmListWithLand(Request).FarmListReport;
            var activeList = GetActiveFarms(Request);

            IEnumerable<InvestmentStatusSummaryItem> s = new List<InvestmentStatusSummaryItem>();

            if (Request.SummerizedBy == SummerizedBy.Region)
            {
                s = from b in farms
                    group b by b.Upin.Region() into g
                    select new InvestmentStatusSummaryItem
                    {
                        location = g.Key,
                        totalNoOfFarms = farms.Where(m => m.Upin.Region() == g.Key).Count(),
                        developedFarms = developmentList.Where(m => m.upin.Region() == g.Key && m.progress > 0).Count(),
                        activeFarms = activeList.Where(m => m.upin.Region() == g.Key && m.active == true).Count()
                    };
            }
            else if (Request.SummerizedBy == SummerizedBy.Zone)
            {
                s = from b in farms
                    group b by b.Upin.Zone() into g
                    select new InvestmentStatusSummaryItem
                    {
                        location = g.Key,
                        totalNoOfFarms = farms.Where(m => m.Upin.Zone() == g.Key).Count(),
                        developedFarms = developmentList.Where(m => m.upin.Zone() == g.Key && m.progress > 0).Count(),
                        activeFarms = activeList.Where(m => m.upin.Zone() == g.Key && m.active == true).Count()
                    };
            }
            else if (Request.SummerizedBy == SummerizedBy.Woreda)
            {
                s = from b in farms
                    group b by b.Upin.Woreda() into g
                    select new InvestmentStatusSummaryItem
                    {
                        location = g.Key,
                        totalNoOfFarms = farms.Where(m => m.Upin.Woreda() == g.Key).Count(),
                        developedFarms = developmentList.Where(m => m.upin.Woreda() == g.Key && m.progress > 0).Count(),
                        activeFarms = activeList.Where(m => m.upin.Woreda() == g.Key && m.active == true).Count()
                    };
            }
            else
            {
                {
                    s = from b in farms
                        group b by b.Upin.Region() into g
                        select new InvestmentStatusSummaryItem
                        {
                            location = g.Key,
                            totalNoOfFarms = farms.Where(m => m.Upin.Region() == g.Key).Count(),
                            developedFarms = developmentList.Where(m => m.upin.Region() == g.Key && m.progress > 0).Count(),
                            activeFarms = activeList.Where(m => m.upin.Region() == g.Key && m.active == true).Count()
                        };
                }
            }

            string loc = "";
            var lookup = GetLocationNames(s.Select(m => m.location).Distinct().ToList(), out loc);

            return new ReportResponseModel()
            {
                Request = Request,
                InvestmentStatusSummary = s.ToList(),
                LocationLookup = lookup,
                AddressType = loc
            };
        }

        public List<FarmActiveStatus> GetActiveFarms(ReportRequestModel Request)
        {
            var farms = GetFarmListWithLand(Request).FarmListReport;
            var activities = Context.Activity.ToList();

            var activeTagActivities = activities.Where(m => m.Tag == "development_land").ToList();
            var rootActivityId = farms.Select(m => m.ActivityId).ToList();

            Dictionary<Guid, Guid> farmTagDictionary = new Dictionary<Guid, Guid>();
            foreach (var item in activeTagActivities)
            {
                var parent = activities.Where(m => m.Id == item.ParentActivityId).First();
                while(parent != null || !rootActivityId.Contains(parent.Id) || parent.ParentActivityId != null)
                {
                    parent = activities.Where(m => m.Id == parent.ParentActivityId).First();
                }
                if(parent != null && parent.ParentActivityId != null && rootActivityId.Contains(parent.Id))
                {
                    farmTagDictionary.Add(farms.Where(m => m.ActivityId == parent.Id).First().FarmId, parent.Id);
                }
            }

            List<FarmActiveStatus> activeList = new List<FarmActiveStatus>();

            foreach (var f in farms)
            {
                var contains = farmTagDictionary.ContainsKey(f.FarmId);
                var status = false;
                if (contains)
                    status = _projFacade.CalculateProgress(farmTagDictionary[f.FarmId], DateTime.Now.Ticks) > 0;
                activeList.Add(new FarmActiveStatus()
                {
                    farmId = f.FarmId,
                    active = status,
                    upin = f.Upin
                });
            }

            return activeList;
            
        }
    }
}
