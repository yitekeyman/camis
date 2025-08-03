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

        public ReportResponseModel FarmListReportByTypeAndLandSize(ReportRequestModel Model)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();


            var sql = " select split_part(lu.upin,'/',1) region, fo.name \"name\", fo.type_id \"type\" ,lu.area area from lb.land_upin lu inner " +
                "  join frm.farm_land fl on lu.land_id = fl.land_id inner join frm.farm fa on fl.farm_id = fa.id inner join frm.farm_operator fo on fa.operator_id = fo.id";

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            List<FarmListReport> response = new List<FarmListReport>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                response.Add(new FarmListReport()
                {
                    Region = dr[0].ToString(),
                    Name = dr[1].ToString(),
                    Type = Int32.Parse(dr[2].ToString()),
                    Area = double.Parse(dr[3].ToString()),
                });
            }
            Context.Database.CloseConnection();
            return new ReportResponseModel()
            {
                FarmList = response
            };
        }

        //public ReportResponseModel FarmListSummaryByTypeAndLandSize(ReportRequestModel Model)
        //{
        //    Context.Database.OpenConnection();
        //    var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

        //    var sql = "select substring(lu.upin, 1, 2) region, fo.type_id \"type\" ,sum(lu.area) area, count(fo.id) number from lb.land_upin lu " +
        //        "inner join frm.farm_land fl on lu.land_id = fl.land_id inner join frm.farm fa on fl.farm_id = fa.id inner join frm.farm_operator fo " +
        //        "on fa.operator_id = fo.id group by region, type";


        //    var cmd = new Npgsql.NpgsqlCommand(sql, connection);
        //    List<FarmListReport> response = new List<FarmListReport>();
        //    var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //    {
        //        response.Add(new FarmListReport()
        //        {
        //            Region = dr[0].ToString(),
        //            Type = Int32.Parse(dr[1].ToString()),
        //            Area = double.Parse(dr[2].ToString()),
        //            Number = Int32.Parse(dr[3].ToString()),
        //        });
        //    }
        //    Context.Database.CloseConnection();
        //    return new ReportResponseModel()
        //    {
        //        FarmList = response
        //    };
        //}

        public string GetSummerizedBySqlString(ReportRequestModel Model)
        {
            string summerizedBy = " ";
            if (Model.SummerizedBy == 0 || Model.SummerizedBy == SummerizedBy.Region)
            {
                summerizedBy = " split_part(lu.upin,'/',1) ";
            }
            else if (Model.SummerizedBy == SummerizedBy.Zone)
            {
                summerizedBy = " CONCAT(split_part(lu.upin,'/',1),'/',split_part(lu.upin,'/',2))  ";
            }

            else if (Model.SummerizedBy == SummerizedBy.Woreda)
            {
                summerizedBy = " CONCAT(split_part(lu.upin,'/',1),'/',split_part(lu.upin,'/',2),'/',split_part(lu.upin,'/',3))  ";
            }
            return summerizedBy;
        }

        public ReportResponseModel LandTransferedAtDifferentTimes(ReportRequestModel Model)
        {
            string summerizedBy = GetSummerizedBySqlString(Model);


            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var sql = $" select {summerizedBy} region, sum(lu.area) area from(select lu.upin, lu.area  from(select fa.id farm_id " +
                "from frm.farm fa inner join sys.user_action ua on fa.aid = ua.id where timestamp > @t1 and timestamp < @t2) " +
                "f inner join frm.farm_land fl on f.farm_id = fl.farm_id inner join lb.land_upin lu on lu.land_id = fl.land_id) lu " +
                $"group by {summerizedBy}";


            var sql2 = $" select {summerizedBy} region, sum(lu.area) area from(select lu.upin, lu.area  from(select fa.id farm_id " +
    "from frm.farm fa inner join sys.user_action ua on fa.aid = ua.id where timestamp > @t1) " +
    "f inner join frm.farm_land fl on f.farm_id = fl.farm_id inner join lb.land_upin lu on lu.land_id = fl.land_id) lu " +
    $"group by {summerizedBy}";

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            var cmd2 = new Npgsql.NpgsqlCommand(sql2, connection);
            var t1 = cmd.Parameters.Add("@t1", NpgsqlTypes.NpgsqlDbType.Bigint);
            var t2 = cmd.Parameters.Add("@t2", NpgsqlTypes.NpgsqlDbType.Bigint);
            var cmd2t1 = cmd2.Parameters.Add("@t1", NpgsqlTypes.NpgsqlDbType.Bigint);

            Model.Dates = Model.Dates.OrderBy(m => m.date).ToList();
            var paramLength = Model.Dates.Count;
            List<AreaInDateInverval> report = new List<AreaInDateInverval>();
            for (int i = 0; i < paramLength -1 ; i++)
            {
                var time1 = Model.Dates[i].date.Ticks;
                var time2 = Model.Dates[i + 1].date.Ticks;
                var interv = new AreaInDateInverval()
                {
                    T1 = time1,
                    T2 = time2
                };
                List<AreaRegionReport> rep = new List<AreaRegionReport>();
                t1.Value = time1;
                t2.Value = time2;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        rep.Add(new AreaRegionReport() { region = dr[0].ToString(), area = double.Parse(dr[1].ToString()) });
                    }
                    interv.RegionAreas = rep;
                    report.Add(interv);
                }
               // var dr = cmd.ExecuteReader();

            }
            cmd2t1.Value = Model.Dates.Last().date.Ticks;
            var interv2 = new AreaInDateInverval()
            {
                T1 = Model.Dates.Last().date.Ticks,
                T2 = -1,
            };
            List<AreaRegionReport> rep2 = new List<AreaRegionReport>();
            using (var dr = cmd2.ExecuteReader())
            {
                while (dr.Read())
                {
                    rep2.Add(new AreaRegionReport() { region = dr[0].ToString(), area = double.Parse(dr[1].ToString()) });
                }
                interv2.RegionAreas = rep2;
                report.Add(interv2);
            }

            Context.Database.CloseConnection();

            string loc = "";
            var lookup = GetLocationNames(report.SelectMany(m => m.RegionAreas).Select(m => m.region).ToList(), out loc);
            return new ReportResponseModel()
            {
                RegionAreaSummery = report,
                LocationLookup = lookup,
                AddressType = loc
            };
           
        }

        public List<FarmAreaInReigion> FarmAreaInRegionReport(ReportRequestModel Model)
        {
            string summerizedBy = GetSummerizedBySqlString(Model);

            var sql = $" select {summerizedBy} region, sum(lu.area) area " +
                "from(select lu.upin, lu.area  from frm.farm f inner join frm.farm_land fl on f.id = fl.farm_id inner join " +
                $"lb.land_upin lu on lu.land_id = fl.land_id) lu group by {summerizedBy}";

            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();
            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            List<FarmAreaInReigion> FarmAreas = new List<FarmAreaInReigion>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                FarmAreas.Add(new FarmAreaInReigion()
                {
                    Region = dr[0].ToString(),
                    TotalArea = double.Parse(dr[1].ToString())
                });
            }
            Context.Database.CloseConnection();
            return FarmAreas;

        }

        public ReportResponseModel GetFarmCountNAreaReport(ReportRequestModel Model)
        {
            var farmCount = FarmCountInAreaRange(Model);
            var FarmArea = FarmAreaInRegionReport(Model);

            string loc = "";
            var lookup = GetLocationNames(farmCount.SelectMany(m => m.RegionAreaCount).Select(m => m.Key).ToList(), out loc);
            return new ReportResponseModel()
            {
                FarmCount = farmCount,
                FarmArea = FarmArea,
                AddressType = loc,
                LocationLookup = lookup
            };
        }

        public ReportResponseModel GetSummaryOfSubLeasedLand(ReportRequestModel Model)
        {
            string summerizedBy = GetSummerizedBySqlString(Model);
            string condition = GetFilterSqlCommand(Model);

            var sql = $@"  select {summerizedBy} region, count(lu.land_id), sum(lu.area/10000), sum(lu.land_section_area)/10000 sa from 
            (select lu.upin, lu.area, lu.land_id, lr.land_section_area from frm.farm f inner join frm.farm_land fl 
            on f.id = fl.farm_id inner join lb.land_upin lu on lu.land_id = fl.land_id left outer join lb.land_right lr
            on lu.land_id = lr.land_id ) lu  {condition} group by region ";

            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();
            var cmd = ParametrizeFilterValue(Model, sql, connection);

            List<FarmLandSummary> fl = new List<FarmLandSummary>();
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    double x = 0.0;
                    fl.Add(new FarmLandSummary()
                    {
                        Location = dr[0].ToString(),
                        Count = int.Parse(dr[1].ToString()),
                        Area = double.Parse(dr[2].ToString()),
                        SubleasedArea = double.TryParse(dr[3].ToString(), out x) ? double.Parse(dr[3].ToString()) : 0
                    });
                }
            }


            string loc = "";
            var lookup = GetLocationNames(fl.Select(m => m.Location).ToList(), out loc);

            return new ReportResponseModel()
            {
                Request = Model,
                FarmLandSummary = fl,
                LocationLookup = lookup,
                AddressType = loc
            };
        }

        public List<FarmInfoInAreaRange> FarmCountInAreaRange(ReportRequestModel Model)
        {
            string summerizedBy = GetSummerizedBySqlString(Model);
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var sql = $" select {summerizedBy} region, count(lu.land_id),sum(lu.area)/10000  from(select lu.upin, lu.area, lu.land_id  from frm.farm f inner " +
                "join frm.farm_land fl on f.id = fl.farm_id inner join lb.land_upin lu on lu.land_id = fl.land_id) lu " +
                "where lu.area > @area1 and lu.area <= @area2 group by region";

            var sql2 = $" select {summerizedBy} region, count(lu.land_id),sum(lu.area)/10000  from(select lu.upin, lu.area, lu.land_id  from frm.farm f inner " +
                "join frm.farm_land fl on f.id = fl.farm_id inner join lb.land_upin lu on lu.land_id = fl.land_id) lu " +
                "where lu.area > @area1  group by region";

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            var t1 = cmd.Parameters.Add("@area1", NpgsqlTypes.NpgsqlDbType.Double);
            var t2 = cmd.Parameters.Add("@area2", NpgsqlTypes.NpgsqlDbType.Double);

            var cmd2 = new Npgsql.NpgsqlCommand(sql2, connection);
            var cmd2t1 = cmd2.Parameters.Add("@area1", NpgsqlTypes.NpgsqlDbType.Double);

            var sizes = Model.FarmSizes.Select(m => m.size).ToList();
            sizes.Add(0);
            var s = sizes.Distinct().OrderBy(m => m).ToArray();
            List<FarmInfoInAreaRange> Count = new List<FarmInfoInAreaRange>();
            for (int i = 0; i < s.Length -1; i++)
            {
                t1.Value = s[i]*10000;
                t2.Value = s[i + 1]*10000;
                FarmInfoInAreaRange farmCount = new FarmInfoInAreaRange()
                {
                    areaFrom = s[i],
                    areaTo = s[i + 1],
                    RegionAreaCount = new Dictionary<string, FarmAreaCount>()
                };
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        farmCount.RegionAreaCount.Add(dr[0].ToString(),
                            new FarmAreaCount()
                            {
                                count = int.Parse(dr[1].ToString()),
                                area = double.Parse(dr[2].ToString()),
                            });                       
                        
                    }
                     Count.Add(farmCount);
                }
                
              
                  
            }

            cmd2t1.Value = s[s.Length-1] * 10000;
            FarmInfoInAreaRange farmCount2 = new FarmInfoInAreaRange()
            {
                areaFrom = s[s.Length -1],
                areaTo = -1,
                RegionAreaCount = new Dictionary<string, FarmAreaCount>()
            };
            using (var dr = cmd2.ExecuteReader())
            {
                while (dr.Read())
                {
                    farmCount2.RegionAreaCount.Add(dr[0].ToString(),
                        new FarmAreaCount()
                        {
                            count = int.Parse(dr[1].ToString()),
                            area = double.Parse(dr[2].ToString()),
                        });

                }
                Count.Add(farmCount2);
            }

            Context.Database.CloseConnection();
            return Count;
        }

        public ReportResponseModel SummaryOfAgricultureInvestmentByInvestorOrigin(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var summerizedBy = GetSummerizedBySqlString(Request);
            var where = GetFilterSqlCommand(Request);

            var sql = $"select  {summerizedBy} \"location\", lu.area, fo.origin_id from lb.land_upin lu inner " +
                " join frm.farm_land fl on lu.land_id = fl.land_id inner join frm.farm fa on fl.farm_id = fa.id inner join frm.farm_operator fo on fa.operator_id = fo.id " +
                $" {where} ";

            var cmd = ParametrizeFilterValue(Request, sql, connection);
            List<OperatorResponse> Model = new List<OperatorResponse>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Model.Add(new OperatorResponse()
                {
                    Location = dr[0].ToString(),
                    Area = double.Parse(dr[1].ToString()),
                    OriginId = int.Parse(dr[2].ToString()),
                });
            }
            Context.Database.CloseConnection();

            string loc = "";
            var lookup = GetLocationNames(Model.Select(m => m.Location).ToList(), out loc);

            return new ReportResponseModel()
            {
                OperatorSummary = Model,
                LocationLookup = lookup,
                AddressType = loc
            };
        }

        public List<RainfedSummaryItem> GetRainfedSummary(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var summerizedBy = GetSummerizedBySqlString(Request);
            var filter = GetFilterSqlCommand(Request,false);
            if (String.IsNullOrWhiteSpace(filter))
                filter = "";
            else
                filter = $"and {filter} ";

            var sql = $@" select {summerizedBy} loc, sum(lu.area)/10000 area, count(lu.land_id) count from 
                    lb.land_upin lu inner join lb.land_moisture mo on lu.land_id = mo.land_id
                    where mo.moisture = 1 {filter} group by   {summerizedBy} ";
            var cmd = ParametrizeFilterValue(Request, sql, connection);
            List<RainfedSummaryItem> Response = new List<RainfedSummaryItem>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Response.Add(new RainfedSummaryItem()
                {
                    location = dr[0].ToString(),
                    area = double.Parse(dr[1].ToString()),
                    count = int.Parse(dr[2].ToString())
                });
            }

            Context.Database.CloseConnection();
            return Response;
        }

        public List<SurfaceWaterSummrayItem> GetSurfaceWaterSummary(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var summerizedBy = GetSummerizedBySqlString(Request);
            var filter = GetFilterSqlCommand(Request, false);
            if (String.IsNullOrWhiteSpace(filter))
                filter = "";
            else
                filter = $"where {filter} ";

            var sql = $@" select {summerizedBy} loc, sw.type t, sum(lu.area)/10000 area, count(lu.land_id) count from 
                    lb.land_upin lu inner join lb.irrigation ir on lu.land_id = ir.land_id inner join 
                    lb.surface_water sw on ir.id = sw.irrigation {filter}
                     group by  {summerizedBy}, t
                    ";
            var cmd = ParametrizeFilterValue(Request, sql, connection);
            List<SurfaceWaterSummrayItem> Response = new List<SurfaceWaterSummrayItem>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Response.Add(new SurfaceWaterSummrayItem()
                {
                    location = dr[0].ToString(),
                    type = int.Parse(dr[1].ToString()),
                    area = double.Parse(dr[2].ToString()),
                    count = int.Parse(dr[3].ToString())
                });
            }


            Context.Database.CloseConnection();
            return Response;
        }


        public List<GroundWaterSummaryItem> GetGroundWaterSummary(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var summerizedBy = GetSummerizedBySqlString(Request);
            var filter = GetFilterSqlCommand(Request, false);
            if (String.IsNullOrWhiteSpace(filter))
                filter = "";
            else
                filter = $"where {filter} ";

            var sql = $@" select {summerizedBy} loc,  gd.grnd_type t, sum(lu.area)/10000 area, count(lu.land_id) count from 
                    lb.land_upin lu inner join lb.irrigation ir on lu.land_id = ir.land_id inner join 
                    lb.ground_data gd on ir.id = gd.irrigation {filter}
                     group by  {summerizedBy}, t
                    ";
            var cmd = ParametrizeFilterValue(Request, sql, connection);
            List<GroundWaterSummaryItem> Response = new List<GroundWaterSummaryItem>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Response.Add(new GroundWaterSummaryItem()
                {
                    location = dr[0].ToString(),
                    type = int.Parse(dr[1].ToString()),
                    area = double.Parse(dr[2].ToString()),
                    count = int.Parse(dr[3].ToString())
                });
            }

            Context.Database.CloseConnection();
            return Response;
        }

        public ReportResponseModel GetWaterSourceSummary(ReportRequestModel Request)
        {
            var rainfed = GetRainfedSummary(Request);
            var surface = GetSurfaceWaterSummary(Request);
            var ground = GetGroundWaterSummary(Request);

            var rainfed_location = rainfed.Select(m => m.location).Distinct().ToList();
            var surface_location = surface.Select(sl => sl.location).Distinct().ToList();
            var ground_locations = ground.Select(gl => gl.location).Distinct().ToList();

            var locations = rainfed_location.Concat(surface_location).Concat(ground_locations).Distinct().ToList();
            List<WaterSourceSummary> Summary = new List<WaterSourceSummary>();
            foreach (var l in locations)
            {
                var rf = rainfed.Where(m => m.location == l).ToList();
                var sw = surface.Where(m => m.location == l).ToList();
                var gw = ground.Where(m => m.location == l).ToList();
                Summary.Add(new WaterSourceSummary()
                {
                    location = l,
                    SurfaceWaterSummary = sw,
                    GroundWaterSummary = gw,
                    RainfedSummary = rf
                });
            }

            string loc = "";
            var lookup = GetLocationNames(Summary.Select(m => m.location).ToList(), out loc);

            return new ReportResponseModel()
            {
                WaterSourceSummary = Summary,
                LocationLookup = lookup,
                AddressType = loc
            };
        }



        public ReportResponseModel LeaseAmountByLocationAndInvestmentType(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var summerizedBy = GetSummerizedBySqlString(Request);
            var where = GetFilterSqlCommand(Request);

            var sql = $" select {summerizedBy} \"location\", li.investment, sum(lr.yearly_rent) lease from lb.land la inner join lb.land_upin lu on " +
                $"la.id = lu.land_id left join lb.land_right lr on la.id = lr.land_id inner join lb.land_investment li on la.id = li.land_id " +
                $" { where } group by {summerizedBy}, investment ";

            var cmd = ParametrizeFilterValue(Request, sql, connection);
            List<LeaseSummaryResonse> Model = new List<LeaseSummaryResonse>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var location = dr[0].ToString();
                var land_type = dr[1].ToString();
                var lease = dr[2].ToString();
                Model.Add(new LeaseSummaryResonse() {
                    location = location,
                    land_type = int.Parse(land_type),
                    lease = lease != "" ? Math.Round(double.Parse(lease),2) : 0

                });
            }
            Context.Database.CloseConnection();
            string loc = "";
            var lookup = GetLocationNames(Model.Select(m => m.location).ToList(), out loc);
            return new ReportResponseModel()
            {
                LeaseSummary = Model,
                LocationLookup = lookup,
                AddressType = loc
            };

        }

        public ReportResponseModel GetFarmListWithLand(ReportRequestModel Model)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var sql = " select t.*, (lu.area/10000) area, split_part(lu.upin,'/',1) region, split_part(lu.upin,'/',2) \"zone\", split_part(lu.upin,'/',3) woreda , lu.upin upin from " +
                " (select fa.id id, fo.name \"name\", fa.type_id farm_type, fo.origin_id origin, ua.timestamp startDate, fa.activity_id, fa.id farmid from frm.farm fa inner " +
                " join frm.farm_operator fo on fa.operator_id = fo.id inner  join sys.user_action ua on fa.aid = ua.id) t inner join " +
                "frm.farm_land fl on t.id = fl.farm_id inner join lb.land_upin lu on fl.land_id = lu.land_id order by region, \"zone\", woreda";



            var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            List<FarmListByDateAndArea> List = new List<FarmListByDateAndArea>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Guid temp = new Guid();
                List.Add(new Farms.Models.FarmListByDateAndArea()
                {
                    Name = dr[1].ToString(),
                    Type = Int32.Parse(dr[2].ToString()),
                    Origin = Int32.Parse(dr[3].ToString()),
                    StartDate = long.Parse(dr[4].ToString()),
                    ActivityId = Guid.TryParse(dr[5].ToString(),out temp) ? (Guid?)Guid.Parse(dr[5].ToString()) : null,
                    FarmId = Guid.Parse(dr[6].ToString()),
                    Area = double.Parse(dr[7].ToString()),
                    Region = dr[8].ToString(),
                    Zone = dr[9].ToString(),
                    Woreda = dr[10].ToString(),
                    Upin = dr[11].ToString(),
                });
            }
            Context.Database.CloseConnection();
            return new ReportResponseModel()
            {
                FarmListReport = List
            };
        }


    }
}
