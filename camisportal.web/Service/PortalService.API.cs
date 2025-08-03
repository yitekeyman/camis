using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using camis.types.Utils;
using intaps.camisPortal.Entities;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Projects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace intaps.camisPortal.Service
{
    public partial class PortalService
    {
        bool isValid(int role)
        {
            foreach (PortalModel.UserRole r in Enum.GetValues(typeof(PortalModel.UserRole)))
            {
                if ((int)r == role)
                    return true;
            }
            return false;
        }
        void assertActiveUser(Entities.PortalUser user)
        {
            if (!user.Active)
                throw new InvalidOperationException("User not active");
        }
        void assertUserDataConsistency(Entities.PortalUser user)
        {
            if ((user.Role == (int)PortalModel.UserRole.WebMaster
                 || user.Role == (int)PortalModel.UserRole.Public)
                ^ (user.Region.Equals(PortalModel.REGION_CENTERAL))
            )
                throw new InvalidOperationException("Invalid user data");


        }



        internal void UpdateInvestor(Investor investor)
        {
            ConnectDB();
            assertInvestorPublicUser(investor);
            var existing = GetInvestor(investor.UserName);
            CamisUtils.Assert(existing != null, $"Investor profile not in database");
            var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<intapscamis.camis.domain.Farms.Models.FarmOperatorRequest>(investor.DefaultProfile);
            if (profile == null)
                throw new InvalidOperationException("Investor profile is empty");
            investor.Id = existing.Id;
            context.Investor.Update(investor);
            foreach (var r in context.InvestorRegistrationNumber.Where(x => x.InvestorId == investor.Id))
            {
                context.InvestorRegistrationNumber.Remove(r);
            }
            foreach (var r in profile.Registrations)
            {
                if (context.InvestorRegistrationNumber.Where(m => m.RegisrationNumber.Equals(r.RegistrationNumber) && m.RegistrationTypeId == r.TypeId && m.InvestorId != investor.Id).Any())
                    throw new InvalidOperationException($"Registration number {r.RegistrationNumber} is already used in another investor profile");
                context.InvestorRegistrationNumber.Add(new Entities.InvestorRegistrationNumber()
                {
                    InvestorId = investor.Id,
                    RegisrationNumber = r.RegistrationNumber,
                    RegistrationTypeId = r.TypeId,
                });
            }

            context.SaveChanges();
        }

        public PortalModel.SearchResult<Entities.Investor> GetInvestors(PortalModel.InvestorSearchPar par)
        {
            ConnectDB();
            String cr = null;
            if (par != null || String.IsNullOrEmpty(par.registrationNumber))
                cr = $"id in (Select investor_id from investor_registration_number where regisration_number='{par.registrationNumber}'";
            var sql = $"Select * from investor {(cr == null ? "" : " where " + cr)}";
            var ret = new PortalModel.SearchResult<Entities.Investor>();
            ret.result = context.Investor.FromSqlRaw(sql).ToList();
            ret.NRec = ret.result.Count;
            return ret;
        }

        public PortalModel.PromotionSeachResult SearchPromotions(PortalModel.PromotionSearchFilter filter)
        {
            ConnectDB();
            ClosePromotionAccordingToSubmissionTime();
            this.context.Database.OpenConnection();
            String cr = null;
            if (filter != null)
            {
                if (!String.IsNullOrEmpty(filter.region))
                {
                    cr = StringExtensions.addDelimitedListItem(cr, " and ", $" p.region='{filter.region}'");
                }
                if (filter.states != null && filter.states.Length > 0)
                {
                    String f = null;
                    foreach (var s in filter.states)
                        f = StringExtensions.addDelimitedListItem(f, ",", s.ToString());
                    cr = StringExtensions.addDelimitedListItem(cr, " and ", $" p.status in ({f})");
                }
            }
            var sql = $@"Select p.id, p.region region_code,p.apply_date_to,p.title,p.summary,p.status,r.name region_name from promotion p inner join regions r on r.code=p.region 
                {(cr == null ? "" : "where " + cr)}
                ";
            var ret = new PortalModel.PromotionSeachResult();
            ret.result = new List<PortalModel.PromotionSearchResultItem>();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                DbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        
//                        var landData = JsonConvert.DeserializeObject<JObject>(reader["landData"].ToString());
//                        var area = landData["Area"];

//                        var picture =
//                            JsonConvert.DeserializeObject<PortalModel.DocumentData>(reader["pic_data"].ToString());var
                        var r = new PortalModel.PromotionSearchResultItem()
                        {
                            address = "",
                            deadline = new DateTime((long)reader["apply_date_to"]),
                            regionCode = (string)reader["region_code"],
                            regionName = (string)reader["region_name"],
                            title = (string)reader["title"],
                            summary = (string)reader["summary"],
                            promotion_id = reader["id"].ToString(),
                            status=(int)reader["status"],
                           
                           
                        };
                       
                        ret.result.Add(r);
                    }
                }
            }
            this.context.Database.CloseConnection();
            
            foreach (var re in ret.result)
            {
               
                var pus = GetPromotionUnits(Guid.Parse(re.promotion_id));
                re.landUPINS=new List<PortalModel.landUPIN>();
                foreach (var p in pus)
                {
                    var landUPIN = getUPIN(p.LandProfile, out var land);
                    var lu = new PortalModel.landUPIN()
                    {
                        prom_id = p.Id.ToString(),
                        upin = landUPIN,
                        status = p.Status
                    };
                    
                    re.landUPINS.Add(lu);
                }
            }
            ret.index = 0;
            ret.NRec = ret.result.Count;
            return ret;
        }


        public Entities.Investor GetInvestor(String userName)
        {
            ConnectDB();
            var ret = context.Investor.AsNoTracking().Where(x => x.UserName.Equals(userName));
            if (ret.Any())
            {
                var res = ret.First();
                return res;
            }
            return null;
        }
        public Entities.Investor GetInvestorByID(Guid id)
        {
            ConnectDB();
            var ret = context.Investor.AsNoTracking().Where(x => x.Id == id);
            if (ret.Any())
            {
                var res = ret.First();
                return res;
            }
            return null;
        }

        public void RegisterUser(Entities.PortalUser user)
        {
            ConnectDB();
            if (GetUser(user.UserName) != null)
                throw new InvalidOperationException($"User name {user.UserName} already used.");
            if (GetUserByEmail(user.EMail) != null)
                throw new InvalidOperationException($"Email {user.EMail} already registered.");
            if (!isValid(user.Role))
                throw new InvalidOperationException($"Invalid user role {user.Role}.");
            assertUserDataConsistency(user);
            user.Active = true;
            context.PortalUser.Add(user);
            context.SaveChanges();
        }
        public void UpdateUserRegistration(Entities.PortalUser user)
        {
            ConnectDB();
            var u = GetUser(user.UserName);
            if (u == null)
                throw new InvalidOperationException($"User profile doesn't exist.");
            assertActiveUser(u);
            assertUserDataConsistency(user);

            u.UserName = user.UserName;
            var e = GetUserByEmail(user.EMail);
            if (e != null && !e.UserName.Equals(u.UserName))
                throw new InvalidOperationException($"Email {user.EMail} already registered.");
            if (!isValid(user.Role))
                throw new InvalidOperationException($"Invalid user role {user.Role}.");

            u.UserName = user.UserName;
            u.Region = user.Region;
            u.CamisPassword = user.CamisPassword;
            u.CamisUserName = user.CamisUserName;
            u.Role = user.Role;
            u.EMail = user.EMail;
            u.PhoneNo = user.PhoneNo;
            u.FullName = user.FullName;
            context.PortalUser.Update(u);
            context.SaveChanges();
        }

        public void AddRegion(Entities.Regions region)
        {
            ConnectDB();
            var r = GetRegion(region.Code);
            if(r!=null)
                throw new InvalidOperationException($"Region Code Already Exist");
            context.Regions.Add(region);
            context.SaveChanges();
        }

        public void UpdateRegion(Entities.Regions region)
        {
            ConnectDB();
            var r = GetRegion(region.Code);
            if(r==null)
                throw new InvalidOperationException($"Region Doesn't Exist");
            r.Name = region.Name;
            r.CamisUrl = region.CamisUrl;
            context.Regions.Update(r);
            context.SaveChanges();
        }

        internal List<PortalModel.EvaluationResultItem> GetAllEvaluationPoint(Guid promoID)
        {
            ConnectDB();
            //var u = GetPromotionUnit(promoID);

            return context.ApplicationEvaluation.Where(x => x.PromotionUnitId == promoID).ToList().Select(
                x =>new PortalModel.EvaluationResultItem()
                {
                    investorId=x.InvestorId.ToString(),
                    teamId=x.TeamId.ToString(),
                    userName=x.EvaluatorUserName,
                    val= GetEvaluationPointByPromoUnit(promoID,x.TeamId,x.EvaluatorUserName,x.InvestorId)
                }
            ).ToList();
        }

        public void DeactivateUser(String userName)
        {
            ConnectDB();
            var u = GetUser(userName);
            if (u == null)
                throw new InvalidOperationException($"User profile doesn't exist.");
            assertActiveUser(u);
            u.Active = false;
            context.PortalUser.Update(u);
            context.SaveChanges();
        }
        public void ActivateUser(String userName)
        {
            ConnectDB();
            var u = GetUser(userName);
            if (u == null)
                throw new InvalidOperationException($"User profile doesn't exist.");
            if (u.Active)
                return;
            u.Active = true;
            context.PortalUser.Update(u);
            context.SaveChanges();
        }
        public Entities.PortalUser GetUser(String userName)
        {
            ConnectDB();
            var u = context.PortalUser.Where(x => x.UserName.Equals(userName));
            if (u.Any())
                return u.First();
            return null;
        }
        public Entities.PortalUser GetUserByEmail(String email)
        {
            var u = context.PortalUser.Where(x => x.EMail.Equals(email));
            if (u.Any())
                return u.First();
            return null;
        }
        public List<Entities.PortalUser> GetUsers()
        {
            ConnectDB();
            return context.PortalUser.Where(x => x.Role != (int)PortalModel.UserRole.Public).ToList();
        }
        public List<Entities.PortalUser> GetPublicUsers()
        {
            ConnectDB();
            return context.PortalUser.Where(x => x.Role == (int)PortalModel.UserRole.Public).ToList();
        }
        public List<Entities.Regions> GetRegions()
        {
            ConnectDB();
            return context.Regions.ToList();
        }
        public List<PortalModel.UserRoleName> GetRoles()
        {
            return new List<PortalModel.UserRoleName>(
                new PortalModel.UserRoleName[]
                {
                    new PortalModel.UserRoleName{ID=(int)PortalModel.UserRole.WebMaster,Name="Web Master"},
                    new PortalModel.UserRoleName{ID=(int)PortalModel.UserRole.Public,Name="Public"},
                    new PortalModel.UserRoleName{ID=(int)PortalModel.UserRole.PromotionManager,Name="Promotion Manager"},
                    new PortalModel.UserRoleName{ID=(int)PortalModel.UserRole.ApplicationEvaluator,Name="Application Evaluator"},
                });
        }

        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {
            ConnectDB();
            var u = GetUser(userName);
            if(u==null)
                throw new InvalidOperationException($"User profile doesn't exist.");
            if(u.Password != oldPassword)
                throw new InvalidOperationException($"Incorrect Old Password.");
            assertActiveUser(u);
            u.Password = newPassword;
            context.PortalUser.Update(u);
            context.SaveChanges();
        }

        public void ResetPassword( PortalUser usr, string userName, string newPassword)
        {
            ConnectDB();
            var u = GetUser(userName);
            if(u==null)
                throw new InvalidOperationException($"User profile doesn't exist.");
          
            assertActiveUser(u);
            u.Password = newPassword;
            context.PortalUser.Update(u);
            context.SaveChanges();
        }
        public Guid RegisterInvestor(Entities.Investor investor)
        {
            ConnectDB();
            assertInvestorPublicUser(investor);

            investor.Id = Guid.NewGuid();
            var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<intapscamis.camis.domain.Farms.Models.FarmOperatorRequest>(investor.DefaultProfile);
            if (profile == null)
                throw new InvalidOperationException("Investor profile is empty");
            context.Investor.Add(investor);
            foreach (var r in profile.Registrations)
            {
                CamisUtils.Assert(!String.IsNullOrEmpty(r.RegistrationNumber), "Registration number is mandatory");
                if (r.Document != null)
                {
                    CamisUtils.Assert(!String.IsNullOrEmpty(r.Document.Mimetype), "MIME type of document not provided");
                    CamisUtils.Assert(!String.IsNullOrEmpty(r.Document.Ref), "Reference of document not provided");
                    CamisUtils.Assert(!String.IsNullOrEmpty(r.Document.File), "data of document not provided");
                }
                if (context.InvestorRegistrationNumber.Where(m => m.RegisrationNumber.Equals(r.RegistrationNumber) && m.RegistrationTypeId == r.TypeId).Any())
                    throw new InvalidOperationException($"Registration number {r.RegistrationNumber} is already used in another investor profile");
                context.InvestorRegistrationNumber.Add(new Entities.InvestorRegistrationNumber()
                {
                    InvestorId = investor.Id,
                    RegisrationNumber = r.RegistrationNumber,
                    RegistrationTypeId = r.TypeId,
                });
            }

            context.SaveChanges();
            return investor.Id;
        }

        private void assertInvestorPublicUser(Entities.Investor investor)
        {
            var usr = GetUser(investor.UserName);
            if (usr.Role != (int)PortalModel.UserRole.Public)
                throw new InvalidOperationException("Only public users can register as investors");
        }
        void loadPromotion(Guid promoId, out Promotion promo, out List<PromotionUnit> promoUnits)
        {
            promo = GetPromotion(promoId);
            CamisUtils.Assert(promo != null, $"Invalid promotion unit ID {promoId}");

            promoUnits = GetPromotionUnits(promoId);
        }
        public PortalModel.SearchResult<PortalModel.Application> SearchApplication(PortalModel.SearchApplicationPar par)
        {
            IQueryable<Entities.InvestorApplication> aps = null;
            if (!String.IsNullOrEmpty(par.promoID) && String.IsNullOrEmpty(par.investorID) && string.IsNullOrEmpty(par.promoUnitID))
            {
                loadPromotion(Guid.Parse(par.promoID), out _, out var promotionUnits);
                if (promotionUnits.Count > 0)
                {
                    aps = context.InvestorApplication.Where(x => x.PromotionUnitId == promotionUnits[0].Id);
                    for (var i = 1; i < promotionUnits.Count; i++)
                    {
                        var index = i;
                        aps = aps.Concat(context.InvestorApplication.Where(x => x.PromotionUnitId == promotionUnits[index].Id));
                    }
                }
            }
            else if (!string.IsNullOrEmpty(par.promoUnitID) && String.IsNullOrEmpty(par.investorID))
            {
                aps = context.InvestorApplication.Where(x => x.PromotionUnitId == Guid.Parse(par.promoUnitID));
            }
            else if (String.IsNullOrEmpty(par.promoID) && !String.IsNullOrEmpty(par.investorID))
            {
                var investorID = Guid.Parse(par.investorID);
                aps = context.InvestorApplication.Where(x => x.InvestorId == investorID);
            }
            else
                throw new InvalidOperationException("Unsupported application search parameter");

            var res = new List<PortalModel.Application>();
            
            if (aps != null)
            {
                foreach (var a in aps)
                {
                    res.Add(toApplication(a));
                }
            }

            return new PortalModel.SearchResult<PortalModel.Application>()
            {
                NRec = res.Count,
                result = res,
            };
        }

        private PortalModel.Application toApplication(InvestorApplication a)
        {
            return new PortalModel.Application
            {
                invProfile = GetInvestorByID(a.InvestorId),
                promoID = context.PromotionUnit.Where(x => x.Id == a.PromotionUnitId).First().PromotionId.ToString(),
                proposalAbstract = a.ProposalAbstract,
                investmentTypes = String.IsNullOrEmpty(a.InvestmentType) ? null : JsonConvert.DeserializeObject<List<int>>(a.InvestmentType),
                proposedCapital = a.InvestmentCapital.Value,
                contactAddress = a.ContactAddress == null ? null : JsonConvert.DeserializeObject<PortalModel.ApplicationContact>(a.ContactAddress),
                applicationTime = new DateTime(a.ApplyTime),
                investment = a.Investment == null ? null : JsonConvert.DeserializeObject<FarmRequest>(a.Investment),
                activityPlan = a.ActivityPlan == null ? null : JsonConvert.DeserializeObject<ActivityPlanResponse>(a.ActivityPlan),
                promotionUnitId = a.PromotionUnitId,
                
                isApproved = a.IsApproved ?? false
            };
        }

        public PortalModel.Application GetApplication(Guid promoUnitId, Guid investorID)
        {
            ConnectDB();
//            Promotion promo;
//            List<PromotionUnit> promotionUnits;
//            loadPromotion(promoID, out promo, out promotionUnits);
           // PromotionUnit punit = GetPromotionUnit(promoUnitId); // NOTE: using the first promotion unit only here..
            // See also: Investor/GetLatestApplication
            
            var docs = new List<PortalModel.DocumentData>();
            foreach (var d in context.InvestorApplicationDocument.Where(x => x.InvestorId == investorID && x.PromotionUnitId == promoUnitId).OrderBy(x => x.Order))
            {
                docs.Add(JsonConvert.DeserializeObject<PortalModel.DocumentData>(d.Data));
            }
            var a = context.InvestorApplication.Where(x => x.InvestorId == investorID && x.PromotionUnitId == promoUnitId).FirstOrDefault();
            
            if (a != null)
            {
               var app = toApplication(a);
                app.proposalDocument = docs;
                return app;
            }
            else
            {
                return null;
            }
        }
        public void AppyForLand(PortalModel.Application application)
        {
            ConnectDB();
            var p = GetPromotion(Guid.Parse(application.promoID));
            CamisUtils.Assert(p != null, $"Invalid promotion unit ID {application.promoID}");
            CamisUtils.Assert(p.Status == (int)PortalModel.PromotionState.Approved, "Application is accepted only when the promotion is on approved state");

//            var pus = GetPromotionUnits(p.Id);
//            CamisUtils.Assert(pus.Count == 1, "Only one to one promotion and promotion unit is supported");
            var pu = GetPromotionUnit(application.promotionUnitId);

            long t = DateTime.Now.Ticks;
            CamisUtils.Assert(t >= p.ApplyDateFrom && t <= p.ApplyDateTo, "Application is not accepted at this time");

            var tran = context.Database.BeginTransaction();
            var existing = GetInvestor(application.invProfile.UserName);
            try
            {
                if (existing != null)
                {
                    UpdateInvestor(application.invProfile);
                }
                else
                {
                    RegisterInvestor(application.invProfile);
                }
                
                context.InvestorApplication.Add(new InvestorApplication
                {
                    InvestorId = application.invProfile.Id,
                    PromotionUnitId = pu.Id,
                    ApplyTime = t,
                    ProposalAbstract = application.proposalAbstract,
                    InvestmentType = application.investmentTypes == null ? null : JsonConvert.SerializeObject(application.investmentTypes),
                    ContactAddress = application.contactAddress == null ? null : JsonConvert.SerializeObject(application.contactAddress),
                    InvestmentCapital = application.proposedCapital,
                    Investment = application.investment == null ? null : JsonConvert.SerializeObject(application.investment),
                    ActivityPlan = application.activityPlan == null ? null : JsonConvert.SerializeObject(application.activityPlan),
                });
                int n = 1;
                foreach (var d in application.proposalDocument)
                {
                    var appDoc = new InvestorApplicationDocument()
                    {
                        Id = Guid.NewGuid(),
                        InvestorId = application.invProfile.Id,
                        PromotionUnitId = pu.Id,
                        Data = Newtonsoft.Json.JsonConvert.SerializeObject(d),
                        Order = n++
                    };
                    CamisUtils.Assert(d != null && d.data != null && d.mime != null, "Proposal document not provided");
                    context.InvestorApplicationDocument.Add(appDoc);
                }
                context.SaveChanges();
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }
        public Guid PostPromotion(PortalUser user, PortalModel.PromotionUnitRequest request)
        {
            ConnectDB();
            if (request.promotion.id == "")
            {
                request.promotion.id = null;
            }
            parsePromotionRequest(request, out var promotion, out var promotionUnit);

            var existingPromotion = context.Promotion.Find(request.promotion?.id?.ToGuid());
            if (existingPromotion == null)
            {
                promotion.PostedOn = DateTime.Now.Ticks;
                if (promotion.ApplyDateFrom >= promotion.ApplyDateTo || promotion.ApplyDateTo < promotion.PostedOn)
                    throw new InvalidOperationException("Invalid promotion validity period");
                promotion.Id = Guid.NewGuid();
                promotion.Status = (int) PortalModel.PromotionState.Posted;
                context.Promotion.Add(promotion);
            }

            savePromotionDetail(user, request, promotion, promotionUnit);
            context.SaveChanges();
            return promotion.Id;
        }
        void parsePromotionRequest(PortalModel.PromotionUnitRequest request, out Promotion promotion, out PromotionUnit unit)
        {
            promotion = context.Promotion.Find(request.promotion?.id?.ToGuid()) ?? new Promotion
            {
                Description = request.promotion.description,
                ApplyDateFrom = request.promotion.applyDateFrom.Ticks,
                ApplyDateTo = request.promotion.applyDateTo.Ticks,
                PostedOn = request.promotion.postedOn.Ticks,
                PromotionRef = request.promotion.promotionRef,
                Summary = request.promotion.Summery,
                Title = request.promotion.title,
                Region = request.promotion.region,
                PhysicalAddress = request.promotion.physicalAddress,
            };
            unit = new PromotionUnit
            {
                Title = request.promotion.title,
                Description = request.promotion.description,
                InvestmentType = request.promotion.investmentTypes == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(request.promotion.investmentTypes),
            };
        }
        List<PromotionUnit> GetPromotionUnits(Guid promotion_id)
        {
            ConnectDB();
            return context.PromotionUnit.Where(x => x.PromotionId.Equals(promotion_id)).ToList();
        }

       public PromotionUnit GetPromotionUnit(Guid PromotionUnit_Id)
        {
            ConnectDB();
            return context.PromotionUnit.Where(x => x.Id==PromotionUnit_Id).First();
        }
        internal void UpdatePromotion(PortalUser user, PortalModel.PromotionUnitRequest request)
        {
            ConnectDB();
            var p = GetPromotion(Guid.Parse(request.promotion.id));
            CamisUtils.Assert(p != null, "Promotion data doesn't not in database");
            var pu = GetPromotionUnit(Guid.Parse(request.id));
            
                foreach (var e in context.ApplicationEvaluationTeam.Where(x => x.PromotionUnitId == pu.Id))
                {
                    context.EvaluationTeamMember.RemoveRange(context.EvaluationTeamMember.Where(x => x.TeamId == e.Id));
                    context.ApplicationEvaluationTeam.Remove(e);
                }
                context.PromotionDoc.RemoveRange(context.PromotionDoc.Where(x => x.PromotionUnitId == pu.Id));
                context.PromotionPicture.RemoveRange(context.PromotionPicture.Where(x => x.PromotionUnitId == pu.Id));
                context.PromotionUnit.Remove(pu);
            
            Entities.Promotion promotion;
            Entities.PromotionUnit detail;
            parsePromotionRequest(request, out promotion, out detail);

            var r = context.Regions.Where(x => x.Code.Equals(promotion.Region)).First();
            promotion.PostedOn = DateTime.Now.Ticks;
            if (promotion.ApplyDateFrom >= promotion.ApplyDateTo || promotion.ApplyDateTo < promotion.PostedOn)
                throw new InvalidOperationException("Invalid promition validaty period");
            promotion.Status = (int)PortalModel.PromotionState.Posted;
            promotion.Id = Guid.Parse(request.promotion.id);
            context.Promotion.Update(promotion);
            savePromotionDetail(user, request, promotion, detail);
            context.SaveChanges();
        }

        public PortalModel.PortalLand GetLandData(PortalUser user,String upin)
        {
            var ld = new PortalModel.PortalLand()
            {
                landData = (new CamisInterface(user, user.Region)).getLandData(upin,true,true),
            };
            ConnectDB();
            context.Database.OpenConnection();
            var sql = $"select ST_AsText(ST_Transform(ST_GeometryFromText('SRID=20137;Point({ld.landData.CentroidX} {ld.landData.CentroidY})'),4326));";
            var point = new Npgsql.NpgsqlCommand(sql,
                (Npgsql.NpgsqlConnection)context.Database.GetDbConnection()
            ).ExecuteScalar().ToString();
            ld.location = LandBankFacadeModel.LatLng.FromWkt(point);
            NetTopologySuite.IO.WKTReader reader = new NetTopologySuite.IO.WKTReader();
            var parcel = ld.landData.parcels[ld.landData.Upins[0]];
            var g=reader.Read(parcel.geometry);
            ld.polygon = new List<List<LandBankFacadeModel.LatLng>>();
            if (g is NetTopologySuite.Geometries.MultiPolygon)
            {
                var mp = (NetTopologySuite.Geometries.MultiPolygon)g;
                foreach(var pg in mp)
                {
                    if(pg is NetTopologySuite.Geometries.Polygon)
                    {
                        ld.polygon.Add(FromNtsPoly((NetTopologySuite.Geometries.Polygon)pg));
                    }
                }
            }
            else if(g is NetTopologySuite.Geometries.Polygon)
            {
                ld.polygon.Add(FromNtsPoly((NetTopologySuite.Geometries.Polygon)g));
            }
            ld.bottomLeft = null;
            ld.topRight = null;
            foreach (var l in ld.polygon)
            {
                foreach (var c in l)
                {
                    if (ld.bottomLeft == null)
                    {
                        ld.bottomLeft = new LandBankFacadeModel.LatLng() { lat = c.lat, lng = c.lng };
                        ld.topRight = new LandBankFacadeModel.LatLng() { lat = c.lat, lng = c.lng };
                    }
                    else
                    {
                        ld.bottomLeft.lng = Math.Min(ld.bottomLeft.lng, c.lng);
                        ld.bottomLeft.lat = Math.Min(ld.bottomLeft.lat, c.lat);

                        ld.topRight.lng = Math.Max(ld.topRight.lng, c.lng);
                        ld.topRight.lat = Math.Max(ld.topRight.lat, c.lat);
                    }
                }
            }
            return ld;
        }

        public PortalModel.PortalLand GetLandDataByPromotionId(string promotionUnitId, Guid promotionId)
        {
            var promotion = this.GetPromotionRequest(promotionId);
            var ld = new PortalModel.PortalLand();
            foreach (var pr in promotion.promotionUnit)
            {
                if (pr.id.Equals(promotionUnitId))
                {
                    ld.landData = pr.landData;
                }
                    
            }
            
            
            ConnectDB();
            context.Database.OpenConnection();
            var sql = $"select ST_AsText(ST_Transform(ST_GeometryFromText('SRID=20137;Point({ld.landData.CentroidX} {ld.landData.CentroidY})'),4326));";
            var point = new Npgsql.NpgsqlCommand(sql,
                (Npgsql.NpgsqlConnection)context.Database.GetDbConnection()
            ).ExecuteScalar().ToString();
            ld.location = LandBankFacadeModel.LatLng.FromWkt(point);
            NetTopologySuite.IO.WKTReader reader = new NetTopologySuite.IO.WKTReader();
            var parcel = ld.landData.parcels[ld.landData.Upins[0]];
            var g=reader.Read(parcel.geometry);
            ld.polygon = new List<List<LandBankFacadeModel.LatLng>>();
            if (g is NetTopologySuite.Geometries.MultiPolygon)
            {
                var mp = (NetTopologySuite.Geometries.MultiPolygon)g;
                foreach(var pg in mp)
                {
                    if(pg is NetTopologySuite.Geometries.Polygon)
                    {
                        ld.polygon.Add(FromNtsPoly((NetTopologySuite.Geometries.Polygon)pg));
                    }
                }
            }
            else if(g is NetTopologySuite.Geometries.Polygon)
            {
                ld.polygon.Add(FromNtsPoly((NetTopologySuite.Geometries.Polygon)g));
            }
            ld.bottomLeft = null;
            ld.topRight = null;
            foreach (var l in ld.polygon)
            {
                foreach (var c in l)
                {
                    if (ld.bottomLeft == null)
                    {
                        ld.bottomLeft = new LandBankFacadeModel.LatLng() { lat = c.lat, lng = c.lng };
                        ld.topRight = new LandBankFacadeModel.LatLng() { lat = c.lat, lng = c.lng };
                    }
                    else
                    {
                        ld.bottomLeft.lng = Math.Min(ld.bottomLeft.lng, c.lng);
                        ld.bottomLeft.lat = Math.Min(ld.bottomLeft.lat, c.lat);

                        ld.topRight.lng = Math.Max(ld.topRight.lng, c.lng);
                        ld.topRight.lat = Math.Max(ld.topRight.lat, c.lat);
                    }
                }
            }
            return ld;
            
        }

        private List<LandBankFacadeModel.LatLng> FromNtsPoly(Polygon pg)
        {
            var ret = new List<LandBankFacadeModel.LatLng>();
            var n = pg.ExteriorRing.NumPoints;
            for(int i=0;i<n;i++)
            {
                var c = pg.ExteriorRing.GetCoordinateN(i);
                ret.Add(new LandBankFacadeModel.LatLng()
                {
                    lng=c.X,
                    lat=c.Y,
                });
            }
            return ret;
        }

        private void savePromotionDetail(PortalUser user, PortalModel.PromotionUnitRequest request, Promotion promotion, PromotionUnit detail)
        {
            String landProfile = validateUPIN(user, request.landUPIN);
            detail.Id = Guid.NewGuid();
            detail.PromotionId = promotion.Id;
            detail.LandProfile = landProfile;
            detail.Status = (int) PortalModel.PromotionState.Posted;
            context.PromotionUnit.Add(detail);
            int o = 1;
            foreach (var p in request.pictures)
            {
                context.PromotionPicture.Add(new PromotionPicture()
                {
                    PromotionUnitId = detail.Id,
                    Order = o++,
                    Picture = Newtonsoft.Json.JsonConvert.SerializeObject(p),
                });
            }
            o = 1;
            foreach (var doc in request.documents)
            {
                context.PromotionDoc.Add(new PromotionDoc()
                {
                    PromotionUnitId = detail.Id,
                    Order = o++,
                    DocData = Newtonsoft.Json.JsonConvert.SerializeObject(doc),
                });
            }

            if (request.evalTeams != null)
                foreach (var t in request.evalTeams)
                {
                    generateId(t.criterion);
                    var et = new Entities.ApplicationEvaluationTeam()
                    {
                        Id = Guid.NewGuid(),
                        EvaluationCriterion = Newtonsoft.Json.JsonConvert.SerializeObject(t.criterion),
                        PromotionUnitId = detail.Id,
                        TeamName = t.teamName,
                        TeamWeight = t.weight,
                    };
                    context.ApplicationEvaluationTeam.Add(et);
                    foreach (var m in t.members)
                    {
                        var em = new Entities.EvaluationTeamMember()
                        {
                            TeamId = et.Id,
                            UserName = m.UserName,
                            Weight = m.Weight,
                        };
                        var usr = GetUser(em.UserName);
                        if (usr == null)
                            throw new InvalidOperationException($"{em.UserName} is not a valid user of camis portal");
                        if (usr.Role != (int)PortalModel.UserRole.ApplicationEvaluator)
                            throw new InvalidOperationException($"{em.UserName} doesn't have evaluation role in camis portal");
                        context.EvaluationTeamMember.Add(em);
                    }
                }
        }

        private void generateId(List<PortalModel.EvaluationCriterion> criterion)
        {
            if (criterion == null)
                return;
            foreach (var c in criterion)
            {
                c.id = Guid.NewGuid().ToString();
                generateId(c.cubCriterion);
            }
        }

        public Entities.Promotion GetPromotion(Guid promo_id)
        {
            var pu = context.Promotion.AsNoTracking().Where(x => x.Id == promo_id);
            if (pu.Any())
                return pu.First();
            return null;
        }
        public PortalModel.PromotionRequest GetPromotionRequest(Guid prom_id)
        {
            ConnectDB();
            var p = GetPromotion(prom_id);
            var pus = GetPromotionUnits(prom_id);
//            CamisUtils.Assert(pus.Count == 1, $"{pus.Count} pomotion unit records found when exactly 1 is expectd for promotion id:{prom_id}");
//            var pu = pus[0];
            var proUnit = new PortalModel.PromotionRequest()
            {
                id = prom_id.ToString(),
                applyDateFrom = new DateTime(p.ApplyDateFrom),
                applyDateTo = new DateTime(p.ApplyDateTo),
                description = p.Description,
                postedOn = new DateTime(p.PostedOn.Value),
                promotionRef = p.PromotionRef,
                region = p.Region,
                status = p.Status,
                Summery = p.Summary,
                title = p.Title,
                physicalAddress = p.PhysicalAddress,
            };
            proUnit.promotionUnit=new List<PortalModel.PromotionUnitRequest>();
            foreach (var pu in pus)
            {
                var ret = new PortalModel.PromotionUnitRequest()
                {
                    landUPIN = getUPIN(pu.LandProfile,out var land),
                    promotion = new PortalModel.PromotionRequest()
                    {
                        id = prom_id.ToString(),
                        applyDateFrom = new DateTime(p.ApplyDateFrom),
                        applyDateTo = new DateTime(p.ApplyDateTo),
                        description = p.Description,
                        postedOn = new DateTime(p.PostedOn.Value),
                        promotionRef = p.PromotionRef,
                        region = p.Region,
                        status = p.Status,
                        Summery = p.Summary,
                        title = p.Title,
                        investmentTypes = String.IsNullOrEmpty(pu.InvestmentType) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(pu.InvestmentType),
                        physicalAddress = p.PhysicalAddress,
                    }
                };
                ret.landData = land;
                ret.id = pu.Id.ToString();
                ret.pictures = new List<PortalModel.DocumentData>();
                foreach (var pic in context.PromotionPicture.Where(x => x.PromotionUnitId == pu.Id).OrderBy(x => x.Order))
                {
                    ret.pictures.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<PortalModel.DocumentData>(pic.Picture)); ;
                }

                ret.documents = new List<PortalModel.DocumentData>();
                foreach (var doc in context.PromotionDoc.Where(x => x.PromotionUnitId == pu.Id).OrderBy(x => x.Order))
                {
                    ret.documents.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<PortalModel.DocumentData>(doc.DocData)); ;
                }

                ret.evalTeams = new List<PortalModel.PromotionUnitRequest.EvaluationTeam>();
                foreach (var t in context.ApplicationEvaluationTeam.Where(x => x.PromotionUnitId == pu.Id))
                {
                    var tt = new PortalModel.PromotionUnitRequest.EvaluationTeam()
                    {
                        id=t.Id.ToString(),
                        teamName = t.TeamName,
                        weight = t.TeamWeight.Value,
                        criterion = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PortalModel.EvaluationCriterion>>(t.EvaluationCriterion)
                    };
                    tt.members = new List<EvaluationTeamMember>();
                    foreach (var m in context.EvaluationTeamMember.Where(x => x.TeamId == t.Id))
                    {
                        tt.members.Add(new EvaluationTeamMember()
                        {
                            TeamId = m.TeamId,
                            UserName = m.UserName,
                            Weight = m.Weight.Value,
                        });
                    }
                    ret.evalTeams.Add(tt);
                   
                }

               
                proUnit.investmentTypes = String.IsNullOrEmpty(pu.InvestmentType)
                    ? null
                    : Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(pu.InvestmentType);
                proUnit.promotionUnit.Add(ret);
            }
            
            
            return proUnit;
        }

        public PortalModel.PromotionUnitRequest GetPromotionUnitRequest(Guid promUnit_id, Guid prom_id)
        {
            ConnectDB();
            var p = GetPromotion(prom_id);
            var pu = GetPromotionUnit(promUnit_id);
            var ret = new PortalModel.PromotionUnitRequest()
                {
                    landUPIN = getUPIN(pu.LandProfile,out var land),
                    promotion = new PortalModel.PromotionRequest()
                    {
                        id = prom_id.ToString(),
                        applyDateFrom = new DateTime(p.ApplyDateFrom),
                        applyDateTo = new DateTime(p.ApplyDateTo),
                        description = p.Description,
                        postedOn = new DateTime(p.PostedOn.Value),
                        promotionRef = p.PromotionRef,
                        region = p.Region,
                        status = p.Status,
                        Summery = p.Summary,
                        title = p.Title,
                        investmentTypes = String.IsNullOrEmpty(pu.InvestmentType) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(pu.InvestmentType),
                        physicalAddress = p.PhysicalAddress,
                    }
                };
                ret.landData = land;
                ret.id = pu.Id.ToString();
                ret.status = pu.Status;
                ret.winnerInvestor = pu.WinnerInvestor;
                ret.pictures = new List<PortalModel.DocumentData>();
                foreach (var pic in context.PromotionPicture.Where(x => x.PromotionUnitId == pu.Id).OrderBy(x => x.Order))
                {
                    ret.pictures.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<PortalModel.DocumentData>(pic.Picture)); ;
                }

                ret.documents = new List<PortalModel.DocumentData>();
                foreach (var doc in context.PromotionDoc.Where(x => x.PromotionUnitId == pu.Id).OrderBy(x => x.Order))
                {
                    ret.documents.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<PortalModel.DocumentData>(doc.DocData)); ;
                }

                ret.evalTeams = new List<PortalModel.PromotionUnitRequest.EvaluationTeam>();
                foreach (var t in context.ApplicationEvaluationTeam.Where(x => x.PromotionUnitId == pu.Id))
                {
                    var tt = new PortalModel.PromotionUnitRequest.EvaluationTeam()
                    {
                        id=t.Id.ToString(),
                        teamName = t.TeamName,
                        weight = t.TeamWeight.Value,
                        criterion = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PortalModel.EvaluationCriterion>>(t.EvaluationCriterion)
                    };
                    tt.members = new List<EvaluationTeamMember>();
                    foreach (var m in context.EvaluationTeamMember.Where(x => x.TeamId == t.Id))
                    {
                        tt.members.Add(new EvaluationTeamMember()
                        {
                            TeamId = m.TeamId,
                            UserName = m.UserName,
                            Weight = m.Weight.Value,
                        });
                    }
                    ret.evalTeams.Add(tt);
                   
                }

            return ret;
        }

        public void ApprovePromotion(Guid promo_id)
        {
            ConnectDB();
            var pu = GetPromotion(promo_id);
            var prs = GetPromotionUnits(promo_id);
            CamisUtils.Assert(pu!=null, $"{promo_id} is not a valid promotion id");
            CamisUtils.Assert(pu.Status==(int)PortalModel.PromotionState.Posted, $"Promotion {promo_id} can't be approved now");
            ChangePromotionStatus(pu, PortalModel.PromotionState.Approved);
            foreach (var pr in prs)
            {
               ChangePromotionUnitStatus(pr, PortalModel.PromotionState.Approved);
            }
        }
        public void CancelPromotion(Guid promo_id)
        {
            ConnectDB();
            var pu = GetPromotion(promo_id);
            var prs = GetPromotionUnits(promo_id);
            CamisUtils.Assert(pu != null, $"{promo_id} is not a valid promotion id");
            CamisUtils.Assert(pu.Status != (int)PortalModel.PromotionState.ProposalAccepted, $"Promotion {promo_id} can't be canceled now");
            ChangePromotionStatus(pu, PortalModel.PromotionState.Canceled);
            foreach (var pr in prs)
            {
                ChangePromotionUnitStatus(pr, PortalModel.PromotionState.Canceled);
            }
            
        }
        public void ClosePromotion(Guid promo_id)
        {
            ConnectDB();
            var pu = GetPromotion(promo_id);
            var prs = GetPromotionUnits(promo_id);
            CamisUtils.Assert(pu != null, $"{promo_id} is not a valid promotion id");
            CamisUtils.Assert(pu.Status == (int)PortalModel.PromotionState.Approved, $"Proposal is not accpeted for promotion {promo_id}");
            ChangePromotionStatus(pu, PortalModel.PromotionState.Closed);
            foreach (var pr in prs)
            {
                ChangePromotionUnitStatus(pr, PortalModel.PromotionState.Closed);
            }
        }
        public void StartPromotion(Guid promo_id)
        {
            ConnectDB();
            var pu = GetPromotion(promo_id);
            CamisUtils.Assert(pu != null, $"{promo_id} is not a valid promotion id");
            CamisUtils.Assert(pu.Status == (int)PortalModel.PromotionState.Closed, $"Promotion {promo_id} must be closed first");
            ChangePromotionStatus(pu, PortalModel.PromotionState.EvaluationStarted);
        }
        public void StartPromotionEvaluation(Guid promo_id)
        {
            ConnectDB();
            var pu = GetPromotion(promo_id);
            CamisUtils.Assert(pu != null, $"{promo_id} is not a valid promotion id");
            CamisUtils.Assert(pu.Status == (int)PortalModel.PromotionState.Closed, $"Promotion {promo_id} must be closed first");
            ChangePromotionStatus(pu, PortalModel.PromotionState.EvaluationStarted);
        }
        public void FinishPromotionEvaluation(Guid promo_id)
        {
            ConnectDB();
            var pu = GetPromotion(promo_id);
            var pus = GetPromotionUnits(promo_id);
            CamisUtils.Assert(pu != null, $"{promo_id} is not a valid promotion id");
            CamisUtils.Assert(pu.Status == (int)PortalModel.PromotionState.EvaluationStarted, $"Evaluation of promotion {promo_id} is not started");
            foreach (var p in pus)
            {
                var sum = GetEvaluationSummary(p.Id, promo_id).evaluations;
                CamisUtils.Assert(sum.Count > 0, "Evaluation can't be finished because there is no winning application");
                if (sum.Count > 1)
                    CamisUtils.Assert(sum[0].rank != sum[1].rank, "Evaluation can't be finished because the more than one application with top point are tied");
                ChangePromotionUnitStatus(p, PortalModel.PromotionState.EvaluationFinished);
            }
            
            ChangePromotionStatus(pu, PortalModel.PromotionState.EvaluationFinished);
        }

        public void FinishPromotion(Guid promo_id)
        {
            ConnectDB();
            var pu = GetPromotion(promo_id);
            var prs = GetPromotionUnits(promo_id);
            CamisUtils.Assert(pu != null, $"{promo_id} is not a valid promotion id");
            CamisUtils.Assert(pu.Status == (int)PortalModel.PromotionState.ProposalAccepted, $"Evaluation of promotion {promo_id} is not complete");
            
            ChangePromotionStatus(pu, PortalModel.PromotionState.Finished);
            foreach (var p in prs)
            {
                ChangePromotionUnitStatus(p, PortalModel.PromotionState.Finished);
            }
            
        }
        private void ChangePromotionStatus(Entities.Promotion pu, PortalModel.PromotionState newStatus)
        {
            int oldStatus = pu.Status;
            pu.Status = (int)newStatus;
            var h = new Entities.PromotionStatusChange()
            {
                Id = Guid.NewGuid(),
                ChangeTime = DateTime.Now.Ticks,
                Data = null,
                NewStatus = pu.Status,
                OldStatus = oldStatus,
                PromotionId = pu.Id,
            };
            context.Promotion.Update(pu);
            context.PromotionStatusChange.Add(h);
            context.SaveChanges();
        }

        private void ChangePromotionUnitStatus(Entities.PromotionUnit pu, PortalModel.PromotionState newStatus)
        {
            pu.Status = (int) newStatus;
            context.PromotionUnit.Update(pu);
            context.SaveChanges();
        }

        private String validateUPIN(PortalUser user, String upin)
        {
            var ld = (new CamisInterface(user, user.Region)).getLandData(upin,true,true);
            if (ld == null)
                throw new InvalidOperationException($"Data from for upin :{upin} couldn't be retreived");

            return Newtonsoft.Json.JsonConvert.SerializeObject(ld);
        }
        private String getUPIN(String profile,out LandBankFacadeModel.LandData ld)
        {
            if (profile == null)
            {
                ld = null;
                return null;
            }
            ld = Newtonsoft.Json.JsonConvert.DeserializeObject<LandBankFacadeModel.LandData>(profile);
            if (ld == null || ld.Upins == null || ld.Upins.Count == 0)
                return null;
            return ld.Upins[0];
        }
        PortalModel.EvaluationData loadValuationData(Entities.ApplicationEvaluation eval)
        {
           
            return new PortalModel.EvaluationData()
            {
                evaluationTeamID=eval.TeamId.ToString(),
                evaluatorUserName=eval.EvaluatorUserName,
                investorID=eval.InvestorId.ToString(),
                promoID=context.PromotionUnit.Where(x=>x.Id==eval.PromotionUnitId).First().Id.ToString(),
                result=Newtonsoft.Json.JsonConvert.DeserializeObject<PortalModel.EvaluationData>(eval.EvaluationDetail).result,
            };
        }
        public String SubmitEvaluation(PortalModel.EvaluationData data)
        {
            ConnectDB();
            var p = this.GetPromotion(Guid.Parse(data.promoID));
            var pu = GetPromotionUnit(Guid.Parse(data.promotionUnitID));
            //var u = this.GetPromotionUnit(Guid.Parse(data.promotionUnitID));
            //CamisUtils.Assert(us.Count == 1, "Exactly one promotion unit per promotion is now suppored");
            

            var inv = this.GetInvestorByID(Guid.Parse(data.investorID));
            CamisUtils.Assert(inv != null, "Investor not found in database id:" + data.investorID);

            
            var team = context.ApplicationEvaluationTeam.Where(x => x.Id == Guid.Parse(data.evaluationTeamID)).First();

            var member = context.EvaluationTeamMember.Where(x => x.UserName.Equals(data.evaluatorUserName)).First();

            var criterion = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PortalModel.EvaluationCriterion>>(team.EvaluationCriterion);
            var points = validateAndCalculatePoint(criterion, data.result);
            

            context.ApplicationEvaluation.Add(new ApplicationEvaluation()
            {
                InvestorId=data.investorID.ToGuid(),
                EvaluationDetail=Newtonsoft.Json.JsonConvert.SerializeObject(data),
                EvaluationPoint=points,
                EvaluatorUserName=data.evaluatorUserName,
                PromotionUnitId=Guid.Parse(data.promotionUnitID),
                TeamId=Guid.Parse(data.evaluationTeamID),
            });
            if (p.Status == (int) PortalModel.PromotionState.Closed)
            {
                this.StartPromotionEvaluation(p.Id);
            }
            ChangePromotionUnitStatus(pu,PortalModel.PromotionState.EvaluationStarted);
            
            context.SaveChanges();
            return points.ToString();
        }
        public double GetEvaluationPoint(Guid promoID,Guid teamID,String evaluatorUserName,Guid investorID)
        {
            var p = GetPromotionUnits(promoID)[0];
            return GetEvaluationPointByPromoUnit(p.Id, teamID, evaluatorUserName, investorID);
        }

        private double GetEvaluationPointByPromoUnit(Guid puid, Guid teamID, string evaluatorUserName, Guid investorID)
        {
            var rs = context.ApplicationEvaluation.Where(x => x.EvaluatorUserName.Equals(evaluatorUserName)
                                                              && x.InvestorId.Equals(investorID)
                                                              && x.TeamId.Equals(teamID)
                                                              && x.PromotionUnitId.Equals(puid)
            );
            if (rs.Any())
                return rs.First().EvaluationPoint ?? 0;
            return 0;
        }

        public PortalModel.EvaluationData GetEvaluationData(Guid promoID, Guid teamID, String evaluatorUserName, Guid investorID)
        {
            var p = GetPromotionUnit(promoID);
            var rs = context.ApplicationEvaluation.Where(x => x.EvaluatorUserName.Equals(evaluatorUserName)
                                                              && x.InvestorId.Equals(investorID)
                                                              && x.TeamId.Equals(teamID)
                                                              && x.PromotionUnitId.Equals(p.Id)
            );
            if (rs.Any())
                return  this.loadValuationData(rs.First());
            return null;
        }
        public Guid AcceptProposal(PortalUser usr, Guid promoID, Guid promoUnitID, Guid investorID,String note)
        {
            var p = GetPromotion(promoID);
            var pu = GetPromotionRequest(promoID);
            var prs = GetPromotionUnits(promoID);
            CamisUtils.Assert(pu != null, $"{promoID} is not a valid promotion id");
            CamisUtils.Assert(p.Status == (int)PortalModel.PromotionState.EvaluationFinished, $"Evaluation of promotion {promoID} is not started");

            var investor = GetInvestorByID(investorID);
            var application = GetApplication(promoUnitID, investorID);
            
            var defaultProfile = JsonConvert.DeserializeObject<FarmOperatorRequest>(investor.DefaultProfile);
            defaultProfile.Id = Guid.NewGuid().ToString();

            var farmRequest = new FarmRequest
            {
                Id = Guid.NewGuid().ToString(),
                Description = "Selected from CAMIS Portal",
                InvestedCapital = application.proposedCapital,
                TypeId = application.investmentTypes.Count > 0 ? application.investmentTypes[0] : 0,
                Registrations = new FarmRegistrationRequest[0],
                Operator = defaultProfile,
                ActivityPlan = new ActivityPlanRequest
                {
                    Documents = application.proposalDocument.Select(x => new DocumentRequest
                    {
                        Date = NrlaisInterfaceModel.ToJavaTicks(application.applicationTime),
                        File = x.data,
                        Filename = "Proposal",
                        Mimetype = x.mime,
                        Note = x.docRef,
                        Ref = x.docRef,
                        Type = (int) DocumentTypes.BusinessPlan
                    }).ToList()
                }
            };
            var camisRequest = new CamisInterface(usr, usr.Region).StartFarmRegistration(farmRequest, "Farm registration from CAMIS Portal");

            investor.DefaultProfile = JsonConvert.SerializeObject(defaultProfile);
            context.Investor.Update(investor);
            context.SaveChanges();

            var investorApplication = context.InvestorApplication.Find(application.promotionUnitId, investor.Id);
            investorApplication.ActivityPlan = JsonConvert.SerializeObject(farmRequest.ActivityPlan);
            farmRequest.ActivityPlan = null;
            investorApplication.Investment = JsonConvert.SerializeObject(farmRequest);
            context.InvestorApplication.Update(investorApplication);
            context.SaveChanges();
            
            
            Boolean areAllAccepted = true;
            foreach (var pr in prs)
            {
                if (pr.Id == promoUnitID)
                {
                    ChangePromotionUnitStatus(pr, PortalModel.PromotionState.ProposalAccepted);
                    pr.WinnerInvestor = investorID;
                    context.PromotionUnit.Update(pr);
                    context.SaveChanges();
                }

                if (pr.Status != (int) PortalModel.PromotionState.ProposalAccepted)
                    areAllAccepted = false;
                
            }
            if(areAllAccepted)
                ChangePromotionStatus(p,PortalModel.PromotionState.Finished);
            return camisRequest;
        }
        private double validateAndCalculatePoint(List<PortalModel.EvaluationCriterion> criterion, List<PortalModel.EvaluationResult> result)
        {
            var total = 0.0;
            int i = 0;
            if (criterion.Count != result.Count)
                throw new InvalidOperationException("Number criterion should be the same as number results");
            foreach(var c in criterion)
            {
                
                var thisPoint=0.0;
                if (c.cubCriterion != null && c.cubCriterion.Count > 0)
                {
                    thisPoint = validateAndCalculatePoint(c.cubCriterion, result[i].subResult);
                    result[i].val = thisPoint;
                }
                else
                {
                    thisPoint = result[i].val;
                    if (thisPoint < 0)
                        throw new InvalidOperationException($"Value for criteria {c.name} is negative");
                    if(thisPoint > c.maxVal)
                        throw new InvalidOperationException($"Value for criteria {c.name} is lareger than the maximum allowed {c.maxVal}");
                    if(c.valueList!=null && c.valueList.Count>0)
                    {
                        var res = c.valueList.Where(x => x.value.Equals(thisPoint));
                        if(!res.Any())
                            throw new InvalidOperationException($"Value for criteria {c.name} should be one of the predefined values");
                    }
                }
                total += thisPoint * c.weight;
                i++;
            }

            return total;
        }
        public PortalModel.EvaulationSummaryData GetEvaluationSummary(Guid promUnitId, Guid promoID)
        {
            var ps=GetPromotionRequest(promoID);
            var p = GetPromotion(promoID);
            var pus = GetPromotionUnits(promoID);
            var pr = new PortalModel.PromotionUnitRequest();
            foreach (var prs in ps.promotionUnit)
            {
                if (prs.id.Equals(promUnitId.ToString()))
                {
                    pr = prs;
                }
            }
            var es=GetAllEvaluationPoint(promUnitId);
            var apps = SearchApplication(new PortalModel.SearchApplicationPar()
            {
                promoID = promoID.ToString()
            }).result.Where(x => !x.rejected);

            var ret = apps.Select(x => GetInvestorEvaluations(x, es, pr.evalTeams)).ToList();


            ret.Sort((a,b)=>b.totalPoint.CompareTo(a.totalPoint));
            int r = 0;
            var prev = -1.0;
            foreach (var x in ret)
            {
                if (prev == -1.0 || x.totalPoint < prev)
                    r++;
                x.rank = r;
                
            }
            return new PortalModel.EvaulationSummaryData()
            {
                teams=pr.evalTeams,
                evaluations=ret
            };
        }

        private PortalModel.EvaluationSummaryItem GetInvestorEvaluations(PortalModel.Application application, List<PortalModel.EvaluationResultItem> es, List<PortalModel.PromotionUnitRequest.EvaluationTeam> evalTeams)
        {
            var prof = Newtonsoft.Json.JsonConvert.DeserializeObject<FarmOperatorRequest>(application.invProfile.DefaultProfile);
            
            var teamVals = new List<PortalModel.EvaluationSummaryItem.TeamEvaluationSummary>();
            foreach(var t in evalTeams)
            {
                var members=context.EvaluationTeamMember.Where(x => x.TeamId == Guid.Parse(t.id));
                double totalWeight = 0;
                double total = 0;
                foreach(var m in members)
                {
                    totalWeight+= m.Weight.Value;
                    foreach (var y in es)
                    {
                        if (y.teamId.Equals(t.id) && y.investorId.Equals(application.invProfile.Id.ToString()) && y.userName.Equals(m.UserName))
                        {
                            total += m.Weight.Value * y.val;
                        }
                    }
                }
                teamVals.Add(new PortalModel.EvaluationSummaryItem.TeamEvaluationSummary()
                {
                    teamName = t.teamName,
                    weightedPoint = total / totalWeight
                });
            }
            
            return new PortalModel.EvaluationSummaryItem()
            {
                investorID = application.invProfile.Id.ToString(),
                investorName = prof.Name,
                teamEvaluation = teamVals.ToList(),
                totalPoint=teamVals.Sum(x=>x.weightedPoint)
            };
        }
        public Guid AutoAcceptApplication(PortalUser usr,Guid promUnitId, Guid promoID)
        {
        
            var sum = GetEvaluationSummary(promUnitId, promoID).evaluations;
            CamisUtils.Assert(sum.Count > 0, "Winning application not found");
            if(sum.Count>1)
                CamisUtils.Assert(sum[0].rank!=sum[1].rank, "The application evaluation top point is tied");
            return AcceptProposal(usr, promoID, promUnitId, Guid.Parse(sum[0].investorID), "Automatically selected proposal based on CAMIS portal proposal evaluation system");
        }

        public string CheckUsername(string username)
        {
            var isValid = "";
            if (GetUser(username) != null)
                isValid = username;
            

            return isValid;
        }

        public string CheckEmail(string email)
        {
            ConnectDB();
            var isValid = "";
            var EMail = GetUserByEmail(email);
            if (EMail != null)
                isValid = email;

            return isValid;
        }

        private void ClosePromotionAccordingToSubmissionTime()
        {
            ConnectDB();
            var currentDate = DateTime.Now.Ticks;
            var pro=context.Promotion.Where(p=>p.Status==(int)PortalModel.PromotionState.Approved);
            foreach (var p in pro)
            {
                if (currentDate > p.ApplyDateTo)
                {
                    var prs = GetPromotionUnits(p.Id);
                    ChangePromotionStatus(p, PortalModel.PromotionState.Closed);
                    foreach (var pr in prs)
                    {
                        ChangePromotionUnitStatus(pr, PortalModel.PromotionState.Closed);
                    }
                }
                
            }

        }

        public void PostBondSubmissionDate(Guid promo_id, long date)
        {
            ConnectDB();
            var pu = GetPromotion(promo_id);
            var prs = GetPromotionUnits(promo_id);
            var currentDate = DateTime.Now.Ticks;
            if (pu.ApplyDateTo > date|| date < currentDate)
                throw new InvalidOperationException("Invalid promotion submission date");

            pu.Status = (int) PortalModel.PromotionState.Approved;
            pu.ApplyDateTo = date;
            context.Promotion.Update(pu);
            foreach (var pr in prs)
            {
                ChangePromotionUnitStatus(pr, PortalModel.PromotionState.Approved);
            }
            context.SaveChanges();

        }
       
    }
}