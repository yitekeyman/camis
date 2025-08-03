using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.LandBank;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace intapscamis.camis.domain.Report
{
    public partial class ReportService
    {

        public ReportResponseModel GenerateReport(ReportRequestModel Model)
        {
            var response = new ReportResponseModel();
            //Model.EndDate = Model.Dates[1].date;
            switch (Model.SelectedReportType)
            {
                case ReportTypes.SummaryOfLandByLandStatus:
                    response = SummaryOfLandByLandStatus(Model);
                    break;
                case ReportTypes.ListOfLands:
                    response = LandListReport(Model);
                    break;
                case ReportTypes.ListOfCommercialFarmOwners:
                    response = GetFarmOperatorListReport(Model);
                    break;
                case ReportTypes.AnnualLeaseAmountByAdministrativeLocationInvType:
                    response = LeaseAmountByLocationAndInvestmentType(Model);
                    break;
                case ReportTypes.LandTransferredAtDifferentTimes:
                    response = LandTransferedAtDifferentTimes(Model);
                    break;
                case ReportTypes.LandTransferreToInvestorsByFarmSize:
                    response = GetFarmCountNAreaReport(Model);
                    break;
                case ReportTypes.SummaryOfAgricultureInvestmentByOriginOfInvestors:
                    response = SummaryOfAgricultureInvestmentByInvestorOrigin(Model);
                    break;
                case ReportTypes.SummaryOfLandByAdministrativeLocationandWaterSource:
                    response = GetWaterSourceSummary(Model);
                    break;
                case ReportTypes.SummaryOfSubLeasedLandByLocation:
                    response = GetSummaryOfSubLeasedLand(Model);
                    break;
                case ReportTypes.CropProductionList:
                    response = GetCropProductionList(Model);
                    break;
                case ReportTypes.TimeSeriesInvestorCropProduction:
                    response = GetTimeSeriesInvestorCropProduction(Model);
                    break;
                case ReportTypes.FarmMachineryList:
                    response = GetFarmMachineryList(Model);
                    break;
                case ReportTypes.CampingFacilitesList:
                    response = GetCampingFacilitiesList(Model);
                    break;
                case ReportTypes.CampingFacilitySummary:
                    response = GetCampingFacilitySummary(Model);
                    break;
                case ReportTypes.JobCreationListByProfession:
                    response = GetJobCreationListByProfession(Model);
                    break;
                case ReportTypes.JobCreationListByType:
                    response = GetJobCreationListByType(Model);
                    break;
                case ReportTypes.LandDevelopmentProgressList:
                    response = GetLandDevelopmentProgressList(Model);
                    break;
                case ReportTypes.LandDevelopmentProgressSummary:
                    response = GetLandDevelopmentProgressSummary(Model);
                    break;
                case ReportTypes.InvstmentStatusSummery:
                    response = GetInvestmentStatusByAdministrativeLoaction(Model);
                    break;
                case ReportTypes.ContractFarmingList:
                    response = GetContractFarmingList(Model);
                    break;
                case ReportTypes.ContractFarmingSummary:
                    response = GetContractFarmingSummary(Model);
                    break;
                case ReportTypes.ListOfAllFarmOperators:
                    response = GetFarmOperatorWihoutLandListReport(Model);
                    break;
                case ReportTypes.OperatorSummaryByOrigin:
                    response = GetOperatorSummaryByOrigin(Model);
                    break;
                default:
                    break;
            }

            response.ReferenceData = GetReferenceData();

            return response;
        }

      

        private void pupulateChildList(Land l)
        {
            l.LandAccessibility = Context.LandAccessibility.Where(x => x.LandId == l.Id).ToList();
            l.LandUpin = Context.LandUpin.Where(x => x.LandId == l.Id).ToList();
            l.LandInvestment = Context.LandInvestment.Where(x => x.LandId == l.Id).ToList();
            l.LandMoisture = Context.LandMoisture.Where(x => x.LandId == l.Id).ToList();
            l.LandUsage = Context.LandUsage.Where(x => x.LandId == l.Id).ToList();
            l.AgriculturalZone = Context.AgriculturalZone.FirstOrDefault(x => x.LandId == l.Id);
        }

        LandBankFacadeModel.LandData buildLandData(Land l, bool geom, bool dd)
        {
            var ret = new LandBankFacadeModel.LandData();

            ret.LandID = l.Id.ToString();
            ret.WID = l.Wid.ToString();
            ret.LandType = l.LandType;
            ret.Accessablity = new List<int>();
            ret.Description = l.Description;


            foreach (var a in l.LandAccessibility)
                ret.Accessablity.Add(a.Accessibility.Value);

            ret.Upins = new List<string>();
            ret.parcels = new Dictionary<string, NrlaisInterfaceModel.Parcel>();
            ret.Area = 0;
            ret.CentroidX = 0;
            ret.CentroidY = 0;
            Context.Database.OpenConnection();
            var sql = $"Select {(dd ? "ST_AsText(ST_Transform(geometry,4326))" : "ST_AsText(geometry)")} from lb.land_upin where land_id=@lid and upin=@upin";
            var cmd = new Npgsql.NpgsqlCommand(sql, (Npgsql.NpgsqlConnection)Context.Database.GetDbConnection());
            var plid = cmd.Parameters.Add("@lid", NpgsqlTypes.NpgsqlDbType.Uuid);
            var pupin = cmd.Parameters.Add("@upin", NpgsqlTypes.NpgsqlDbType.Varchar);
            plid.Value = l.Id;

            foreach (var u in l.LandUpin)
            {
                ret.Upins.Add(u.Upin);
                if (!string.IsNullOrEmpty(u.Profile))
                {
                    pupin.Value = u.Upin;
                    var p = Newtonsoft.Json.JsonConvert.DeserializeObject<NrlaisInterfaceModel.Parcel>(u.Profile);
                    if (geom)
                        p.geometry = cmd.ExecuteScalar().ToString();
                    ret.parcels.Add(u.Upin, p);
                    ret.landHolderType = p.GetHolder().partyType;
                }
                ret.Area += u.Area.Value;
                ret.CentroidX += u.CentroidX.Value;
                ret.CentroidY += u.CentroidY.Value;
            }

            if (ret.parcels.Count > 0)
            {
                var p = ret.parcels[l.LandUpin.First().Upin];
                if (p.IsStateLand())
                {
                    ret.Holdership = "State Land";
                }
                else
                {
                    ret.Holdership = "Private Land";
                }
            }
            if (l.LandUpin.Count > 0)
            {
                ret.CentroidX /= l.LandUpin.Count;
                ret.CentroidY /= l.LandUpin.Count;
            }


            #region Adding Additional data


            ret.InvestmentType = new List<int>();
            foreach (var inv in l.LandInvestment)
            {
                ret.InvestmentType.Add(inv.Investment);
            }

            ret.MoistureSource = l.LandMoisture.FirstOrDefault(m => m.LandId == l.Id).Moisture;


            ret.ExistLandUse = new List<int>();
            if (l.LandUsage.Count != 0)
                foreach (var elu in l.LandUsage)
                {
                    ret.ExistLandUse.Add(elu.Use);
                }


            var agrZone = l.AgriculturalZone;
            ret.IsAgriculturalZone = agrZone != null ? agrZone.IsAgriZone : "Yes";

            #endregion

            return ret;
        }

        private FarmOperatorResponse ParseFarmOperatorResponse(FarmOperator farmOperator)
        {
            var res = new FarmOperatorResponse
            {
                Id = farmOperator.Id,
                Name = farmOperator.Name,
                Nationality = farmOperator.Nationality,
                TypeId = farmOperator.TypeId,
                AddressId = farmOperator.AddressId,
                Phone = farmOperator.Phone,
                Email = farmOperator.Email,
                OriginId = farmOperator.OriginId,
                Capital = farmOperator.Capital
            };

            switch (farmOperator.TypeId)
            {
                case 1:
                    res.Gender = farmOperator.Gender;
                    res.MartialStatus = farmOperator.MartialStatus;
                    res.Birthdate = farmOperator.Birthdate;
                    break;
                case 6:
                    res.Ventures = farmOperator.Ventures;
                    break;
            }

            var operatorType = Context.FarmOperatorType.Find(farmOperator.TypeId);
            res.Type = new FarmOperatorTypeResponse
            {
                Id = operatorType.Id,
                Name = operatorType.Name
            };

            var operatorOrigin = Context.FarmOperatorOrigin.Find(farmOperator.OriginId);
            res.Origin = new FarmOperatorOriginResponse
            {
                Id = operatorOrigin.Id,
                Name = operatorOrigin.Name
            };

            return res;
        }

        private FarmResponse ParseFarmResponse(Farm farm)
        {
            var res = new FarmResponse
            {
                Id = farm.Id,
                OperatorId = farm.OperatorId,
                TypeId = farm.TypeId,
                ActivityId = farm.ActivityId,
                InvestedCapital = farm.InvestedCapital,
                Description = farm.Description,
                OtherTypeIds = farm.OtherTypeIds,

                //FarmLands = Context.FarmLand.Where(fl => fl.FarmId == farm.Id).AsEnumerable()
                //    .Select(ParseFarmLandResponse).ToList()
            };

            var farmRegistrations = Context.FarmRegistration.Where(fr => fr.FarmId == farm.Id);
            res.Registrations = new List<FarmRegistrationResponse>();
            foreach (var farmRegistration in farmRegistrations)
            {
                var registrationAuthority =
                    Context.RegistrationAuthority.First(ra => ra.Id == farmRegistration.AuthorityId);
                var registrationType = Context.RegistrationType.First(rt => rt.Id == farmRegistration.TypeId);
                res.Registrations.Add(new FarmRegistrationResponse
                {
                    Id = farmRegistration.Id,
                    RegistrationNumber = farmRegistration.RegistrationNumber,
                    AuthorityId = farmRegistration.AuthorityId,
                    TypeId = farmRegistration.TypeId,
                    Authority = new RegistrationAuthorityResponse
                    {
                        Id = registrationAuthority.Id,
                        Name = registrationAuthority.Name
                    },
                    Type = new RegistrationTypeResponse
                    {
                        Id = registrationType.Id,
                        Name = registrationType.Name
                    },
                });
            }

            var farmOperator = Context.FarmOperator.First(op => op.Id == farm.OperatorId);
            res.Operator = ParseFarmOperatorResponse(farmOperator);

            var farmType = Context.FarmType.First(ft => ft.Id == farm.TypeId);
            res.Type = new FarmTypeResponse
            {
                Id = farmType.Id,
                Name = farmType.Name
            };

            return res;
        }
    }
}
