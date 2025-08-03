using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace intapscamis.camis.domain.Report
{
    public partial class ReportService
    {


        public ReportResponseModel SummaryOfLandByLandStatus(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var summerizedBy = GetSummerizedBySqlString(Request);
            var condition = GetFilterSqlCommand(Request);

            var sql = $"select l.address, l.\"type\", sum(l.area)/10000 from  (select {summerizedBy} address, " +
                $" la.land_type \"type\", lu.area from lb.land_upin lu inner join lb.land la on lu.land_id = la.id {condition} ) l " +
                $"group by address, \"type\" ";

            var cmd = ParametrizeFilterValue(Request, sql, connection);

            List<LandAreaReportModel> Model = new List<LandAreaReportModel>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Model.Add(new LandAreaReportModel()
                {
                    location = dr[0].ToString(),
                    type = Int32.Parse(dr[1].ToString()),
                    area = Math.Round(double.Parse(dr[2].ToString()),2),

                });
            }
            Context.Database.CloseConnection();
            string loc = "";
            var lookup = GetLocationNames(Model.Select(m => m.location).ToList(), out loc);
            return new ReportResponseModel() { LandArea = Model, LocationLookup = lookup, AddressType = loc  };

        }

        public string GetFilterSqlCommand(ReportRequestModel Request, bool includeWhere = true)
        {
            var condition = " ";
            if (Request.FilteredBy == FilteredBy.Region)
            {
                condition = $" split_part(lu.upin,'/',1) = @filter ";
            }
            else if (Request.FilteredBy == FilteredBy.Zone)
            {
                condition = $"  CONCAT(split_part(lu.upin,'/',1),'/',split_part(lu.upin,'/',2))  = @filter ";
            }
            else if (Request.FilteredBy == FilteredBy.Woreda)
            {
                condition = $" CONCAT(split_part(lu.upin,'/',1),'/',split_part(lu.upin,'/',2),'/',split_part(lu.upin,'/',3)) = @filter ";
            }
            return (includeWhere && !string.IsNullOrWhiteSpace(condition) ? " where " + condition : condition);
        }

        public Npgsql.NpgsqlCommand ParametrizeFilterValue(ReportRequestModel Request,string sql, Npgsql.NpgsqlConnection connection)
        {
            var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            if (Request.FilteredBy == FilteredBy.Region)
            {
                var rg = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);
                rg.Value = Request.Region;
            }
            else if (Request.FilteredBy == FilteredBy.Zone)
            {
                var rg = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);
                rg.Value = Request.Zone;
            }
            else if (Request.FilteredBy == FilteredBy.Woreda)
            {
                var rg = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);
                rg.Value = Request.Woreda;
            }

            return cmd;
        }

        public Npgsql.NpgsqlCommand ParametrizeFilterValue2(ReportRequestModel Request, Npgsql.NpgsqlCommand cmd)
        {

            if (Request.FilteredBy == FilteredBy.Region)
            {
                var rg = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);
                rg.Value = Request.Region;
            }
            else if (Request.FilteredBy == FilteredBy.Zone)
            {
                var rg = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);
                rg.Value = Request.Zone;
            }
            else if (Request.FilteredBy == FilteredBy.Woreda)
            {
                var rg = cmd.Parameters.Add("@filter", NpgsqlTypes.NpgsqlDbType.Varchar);
                rg.Value = Request.Woreda;
            }

            return cmd;
        }

        public List<SurfaceWaterItem> GetSurfaceWaterList(Guid[] land_ids)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();
            var sql = $@" select ir.land_id, sw.type, sw.result from lb.irrigation ir inner join lb.surface_water sw on ir.id = sw.irrigation
                where ir.land_id = ANY(@land_id) ";

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var ids = cmd.Parameters.Add("@land_id", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Uuid);
            cmd.Prepare();
            ids.Value = land_ids;
            List<SurfaceWaterItem> resp = new List<SurfaceWaterItem>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                resp.Add(new SurfaceWaterItem()
                {
                    land_id = new Guid(dr[0].ToString()),
                    type = int.Parse(dr[1].ToString()),
                    result = dr[2].ToString()
                });
            }
            Context.Database.CloseConnection();
            return resp;
        }

        public List<GroundWaterItem> GetGroundWaterList(Guid[] land_ids)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();
            var sql = $@" select ir.land_id, gd.grnd_type from lb.irrigation ir inner join lb.ground_data gd on ir.id = gd.irrigation
                where ir.land_id = ANY(@land_id) ";

            var cmd = new Npgsql.NpgsqlCommand(sql, connection);

            var ids = cmd.Parameters.Add("@land_id", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Uuid);
            cmd.Prepare();
            ids.Value = land_ids;
            List<GroundWaterItem> resp = new List<GroundWaterItem>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                resp.Add(new GroundWaterItem()
                {
                    land_id = new Guid(dr[0].ToString()),
                    type = int.Parse(dr[1].ToString()),
                });
            }
            Context.Database.CloseConnection();
            return resp;
        }



        public ReportResponseModel LandListReport(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var condition = GetFilterSqlCommand(Request);

            var sql = $" select la.id, lu.upin, lu.area/10000 from lb.land la inner join lb.land_upin lu on la.id = lu.land_id {condition}";

            var cmd = ParametrizeFilterValue(Request, sql, connection);

            List<LandListReport> Model = new List<LandListReport>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Model.Add(new Farms.Models.LandListReport()
                {
                    id = new Guid(dr[0].ToString()),
                    region = dr[1].ToString().Region(),
                    zone = dr[1].ToString().Zone(),
                    woreda = dr[1].ToString().Woreda(),
                    upin = dr[1].ToString(),
                    area = Math.Round(double.Parse(dr[2].ToString()), 2),
                    SurfaceWater = new List<SurfaceWaterItem>(),
                    GroundWater = new List<GroundWaterItem>()

                });
            }
            Context.Database.CloseConnection();

            

            foreach (var item in Model)
            {
                item.LandInvestment = Context.LandInvestment.Where(x => x.LandId == item.id).ToList();
                item.LandMoisture = Context.LandMoisture.Where(x => x.LandId == item.id).Select(m => new LandMoisture() {
                        Id = m.Id, LandId = m.LandId, Moisture = m.Moisture
                }).ToList();

                item.Irrigation = Context.Irrigation.Where(x => x.LandId == item.id).ToList();
                
            }

            foreach (var item in Model)
            {
                foreach (var lm in item.LandMoisture)
                {
                    lm.MoistureNavigation = null;
                }
            }

            var irrigated = Model.SelectMany(m => m.LandMoisture).Where(m => m.Moisture == 2).Select(m => m.LandId).ToArray();
            var surfaceWaterSource = GetSurfaceWaterList(irrigated);
            var uniqueLandIds = surfaceWaterSource.Select(m => m.land_id).Distinct().ToList();
            foreach (var id in uniqueLandIds)
            {
                var data = surfaceWaterSource.Where(ws => ws.land_id == id).ToList();
                var m = Model.Where(li => li.id == id);
                if(m.Count() > 0)
                {
                    m.First().SurfaceWater = data;
                }
            }


            var groundWaterSource = GetGroundWaterList(irrigated);
            var uniqueLandIds2 = groundWaterSource.Select(m => m.land_id).Distinct().ToList();
            foreach (var id in uniqueLandIds2)
            {
                var data = groundWaterSource.Where(ws => ws.land_id == id).ToList();
                var m = Model.Where(li => li.id == id);
                if (m.Count() > 0)
                {
                    m.First().GroundWater = data;
                }
            }
            var locs = Model.Select(m => m.upin.Woreda()).ToList();
            string loc = "";
            var lookups = GetLocationNames(locs, out loc);

            return new ReportResponseModel()
            {
                LandList = Model,
                LocationLookup = lookups,
                AddressType = loc
            };
        }

        public ReportResponseModel GetFarmOperatorListReport(ReportRequestModel Request)
        {


            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();

            var condition = GetFilterSqlCommand(Request);

            var sql = "select CONCAT(split_part(lu.upin,'/',1),'/',split_part(lu.upin,'/',2),'/',split_part(lu.upin,'/',3)), fo.name, lu.area, fo.origin_id," +
                " fa.type_id, ua.timestamp, fa.invested_capital, fr.registration_number from lb.land_upin lu inner " +
                "join frm.farm_land fl on lu.land_id = fl.land_id inner join frm.farm fa on fl.farm_id = fa.id inner join frm.farm_operator fo on fa.operator_id = fo.id " +
                $"inner join sys.user_action ua on fa.aid = ua.id left outer join frm.farm_registration fr on fa.id = fr.farm_id {condition}";



            var cmd = ParametrizeFilterValue(Request, sql, connection);                                                                                                                                                                       

            List<OperatorResponse> Model = new List<OperatorResponse>();
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Model.Add(new OperatorResponse() {
                    Woreda = dr[0].ToString(),
                    Name = dr[1].ToString(),
                    Area = double.Parse(dr[2].ToString()),
                    OriginId = int.Parse(dr[3].ToString()),
                    InvestmentType = int.Parse(dr[4].ToString()),
                    StartDate = new DateTime(long.Parse(dr[5].ToString())),
                    InvestedCapital = double.Parse(dr[6].ToString()),
                    LicenseNumber = dr[7].ToString(),
                });
            }
            Context.Database.CloseConnection();
            string loc = "";
            var lookup = GetLocationNames(Model.Select(m => m.Woreda).ToList(), out loc);
            return new ReportResponseModel()
            {
                OperatorList = Model,
                LocationLookup = lookup,
                AddressType = loc
            };
        }

        public ReportResponseModel GetFarmOperatorWihoutLandListReport(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();


            var sql = $@"select fo.name, fo.nationality, fot.name, fo.phone, fo.email, fo.capital, fori.name, s.rnum, s.authority, s.name, fo.id
                                from frm.farm_operator fo inner join frm.farm_operator_origin fori on fo.origin_id = fori.id
                                inner join frm.farm_operator_type  fot on fo.type_id = fot.id 
                                left outer join ( select reg.registration_number rnum, auth.name authority, regt.name, reg.operator_id 
                                from frm.farm_operator_registration reg inner join frm.registration_authority auth on reg.authority_id = auth.id
                                inner join frm.registration_type regt on reg.type_id = regt.id) s on s.operator_id = fo.id ";


            List<OperatorResponse> Model = new List<OperatorResponse>();
            var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Model.Add(new OperatorResponse()
                {
                    Name = dr[0].ToString(),
                    Nationality = dr[1].ToString(),
                    Type = dr[2].ToString(),
                    Phone = dr[3].ToString(),
                    Email = dr[4].ToString(),
                    InvestedCapital = double.Parse(dr[5].ToString()),
                    Origin = dr[6].ToString(),
                    LicenseNumber = dr[7].ToString(),
                    Authroity = dr[8].ToString(),
                    RegistrationType = dr[9].ToString(),
                    Id = new Guid(dr[10].ToString()),
                });
            }
            Context.Database.CloseConnection();
            return new ReportResponseModel()
            {
                OperatorList = Model,
            };
        }

        public ReportResponseModel GetOperatorSummaryByOrigin(ReportRequestModel Request)
        {
            Context.Database.OpenConnection();
            var connection = (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection();


            var sql = $@"select fori.name, count(fo.id)
                        from frm.farm_operator fo inner join frm.farm_operator_origin fori on fo.origin_id = fori.id group by fori.name ";


            List<OperatorSummaryByOrigin> Model = new List<OperatorSummaryByOrigin>();
            var cmd = new Npgsql.NpgsqlCommand(sql, connection);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Model.Add(new OperatorSummaryByOrigin()
                {
                    Origin = dr[0].ToString(),
                    Count = int.Parse(dr[1].ToString())
                });
            }
            Context.Database.CloseConnection();
            Console.WriteLine(JsonConvert.SerializeObject(Model));
            return new ReportResponseModel()
            {
                OperatorSummaryByOrigins = Model,
            };
        }


    }
}
