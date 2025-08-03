using System;
using System.Linq;
using intaps.camisPortal.Entities;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Projects.Models;
using Newtonsoft.Json;

namespace intaps.camisPortal.Service
{
    public partial class PortalService
    {
        public Investor GetLatestInvestor(PortalUser user, string userName, string regionCode)
        {
            ConnectDB();

            var investor = context.Investor.FirstOrDefault(x => x.UserName.Equals(userName));
            if (investor == null) return null;

            try
            {
                var older = JsonConvert.DeserializeObject<FarmOperatorRequest>(investor.DefaultProfile);
                var newer = new CamisInterface(user, regionCode).GetLatestFarmOperator(older.Id.ToGuid());
                if (newer == null) throw new Exception("The retrieved investor default profile was null.");

                investor.DefaultProfile = JsonConvert.SerializeObject(newer);
                context.Investor.Update(investor);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                // fallback (do nothing, because investor.DefaultProfile is already here)
                Console.WriteLine("GetLatestApplication did not update from camis.web. Falling back");
                Console.Error.WriteLine(e);
            }

            return investor;
        }

        public PortalModel.Application GetLatestApplication(PortalUser user, Guid promoUnitId, Guid investorId)
        {
            ConnectDB();

            var promotionUnit = context.PromotionUnit.Find(promoUnitId);
            var promotion = GetPromotion(promotionUnit.PromotionId);
            var regcode = promotion.Region;
            if (promotionUnit == null) throw new Exception($"No promotion unit found using ID '{promoUnitId}'");
            
            var investorApplication = context.InvestorApplication.First(x =>
                x.InvestorId == investorId && x.PromotionUnitId == promotionUnit.Id);
            var application = toApplication(investorApplication);

            application.proposalDocument = context.InvestorApplicationDocument
                .Where(x => x.InvestorId == investorId && x.PromotionUnitId == promotionUnit.Id).OrderBy(x => x.Order)
                .Select(d => JsonConvert.DeserializeObject<PortalModel.DocumentData>(d.Data)).ToList();

            try
            {
                var camisInterface = new CamisInterface(user, regcode);
                var farmRequest = JsonConvert.DeserializeObject<FarmRequest>(investorApplication.Investment);
                application.investment = camisInterface.GetLatestFarm(farmRequest.Id.ToGuid());

               
                    application.activityPlan =
                        camisInterface.GetLatestPlanFromRootActivity(application.investment.ActivityId.ToGuid());

                application.isApproved = true; // because no error thrown (i.e. it is definitely approved in camis.web)

                investorApplication.Investment = JsonConvert.SerializeObject(application.investment);
                investorApplication.ActivityPlan = JsonConvert.SerializeObject(application.activityPlan);
                investorApplication.IsApproved = application.isApproved;
                context.InvestorApplication.Update(investorApplication);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                // fallback (do nothing, because application.investment and application.activityPlan are already here)
                Console.WriteLine("GetLatestApplication did not update from camis.web. Falling back");
                Console.Error.WriteLine(e);
            }

            return application;
        }

        public Guid SubmitNewProgressReport(PortalUser user, ActivityPlanRequest body, string description, string regionCode)
        {
            ConnectDB();

            var camisInterface = new CamisInterface(user, regionCode);
            return camisInterface.SubmitNewProgressReport(body, description);
        }
    }
}