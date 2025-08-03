using System;
using System.Collections.Generic;
using System.Net.Mime;
using intaps.camisPortal.model;
using intaps.camisPortal.Service;
using intapscamis.camis.domain.Projects.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp.Extensions;

namespace intaps.camisPortal.Controllers
{
    public class BidController:PortalControllerbase
    {
        [HttpGet]
        public IActionResult GetPromotion([FromQuery]String prom_id)
        {
            try
            {
                //assertSession();
                return Json(Service.GetPromotionRequest(Guid.Parse(prom_id)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        
        [HttpGet]
        public IActionResult GetPromotionUnit([FromQuery]String promUnit_id, String prom_id){
            try
            {
                return Json(Service.GetPromotionUnitRequest(Guid.Parse(promUnit_id), Guid.Parse(prom_id)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }
        [HttpPost]
        public IActionResult PostPromotion([FromBody]PortalModel.PromotionUnitRequest promotion)
        {
            try
            {
                assertSession();
                var usr=assertUserRole(PortalModel.UserRole.PromotionManager);
                promotion.promotion.region = usr.Region;  
                return Json(Service.PostPromotion(usr,promotion));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }        

        [HttpPost]
        public IActionResult UpdatePromotion([FromBody]PortalModel.PromotionUnitRequest promotion)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                promotion.promotion.region = usr.Region;
                Service.UpdatePromotion(usr,promotion);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult ApprovePromotion([FromQuery] String promo_id)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                Service.ApprovePromotion(Guid.Parse(promo_id));
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult CancelPromotion([FromQuery]String promo_id)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                Service.CancelPromotion(Guid.Parse(promo_id));
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult ClosePromotion([FromQuery]String promo_id)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                Service.ClosePromotion(Guid.Parse(promo_id));
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult FinishEvaluation([FromQuery]String promo_id)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                Service.FinishPromotionEvaluation(Guid.Parse(promo_id));
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }



        [HttpPost]
        public IActionResult SearchPromotions([FromBody]PortalModel.PromotionSearchFilter filter)
        {
            try
            {
                if (filter == null)
                    filter = new PortalModel.PromotionSearchFilter();
                return Json(Service.SearchPromotions(filter));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult ApplyForPromotion([FromBody]PortalModel.Application application)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                application.invProfile.UserName = usr.UserName;
                Service.AppyForLand(application);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult SearchApplications([FromBody]PortalModel.SearchApplicationPar par)
        {
            try
            {
                assertSession();
                var user = Service.GetUser(GetSession().userName);
                if (!string.IsNullOrEmpty(par.promoID) || string.IsNullOrEmpty(par.promoUnitID))
                {
                    if (user.Role == 4)
                        assertUserRole(PortalModel.UserRole.ApplicationEvaluator);
                    else if(user.Role==3)
                    {
                        assertUserRole(PortalModel.UserRole.PromotionManager);
                    }

                }
                   
                else if (!string.IsNullOrEmpty(par.investorID))
                {
                    var usr = assertUserRole(PortalModel.UserRole.Public);
                    var i = Service.GetInvestorByID(Guid.Parse(par.investorID));
                    if (i == null || !i.UserName.Equals(usr.UserName))
                        throw new UnauthorizedAccessException("This operation is allowed only for an investor");
                }

                return Json(Service.SearchApplication(par));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetApplication([FromQuery]String promUnit_id,[FromQuery]String investor_id)
        {
            try
            {
                assertSession();
                return Json(Service.GetApplication(Guid.Parse(promUnit_id),Guid.Parse(investor_id)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetLatestApplication([FromQuery]string prom_unit_id, [FromQuery]string investor_id)
        {
            try
            {
                assertSession();
                var user = assertUserRole(PortalModel.UserRole.Public);
                return Json(Service.GetLatestApplication(user, Guid.Parse(prom_unit_id), Guid.Parse(investor_id)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        
        [HttpPost]
        public IActionResult SubmitEvaluation([FromBody]PortalModel.EvaluationData data)
        {
            try
            {
                assertSession();
                var usr=assertUserRole(PortalModel.UserRole.ApplicationEvaluator);
                data.evaluatorUserName = usr.UserName;
                return Json(Service.SubmitEvaluation(data));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetLandData([FromQuery]String upin)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                return Json(Service.GetLandData(usr, upin));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetLandDataByPromotionId([FromQuery] string promotionUnitId, String promotionId)
        {
            try
            {
                return Json(Service.GetLandDataByPromotionId(promotionUnitId, Guid.Parse(promotionId)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
           
        }

        [HttpGet]
        public IActionResult GetEvaluationData([FromQuery]String promID, [FromQuery]String teamID, [FromQuery]String evaluatorUserName, [FromQuery]String investorID)
        {
            try
            {
                assertSession();
                //var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                return Json(Service.GetEvaluationData(Guid.Parse(promID), Guid.Parse(teamID),evaluatorUserName,Guid.Parse(investorID)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetEvaluationPoint([FromQuery]String promID, [FromQuery]String teamID, [FromQuery]String evaluatorUserName, [FromQuery]String investorID)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                return Json(Service.GetEvaluationPoint(Guid.Parse(promID), Guid.Parse(teamID), evaluatorUserName, Guid.Parse(investorID)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        

        [HttpGet]
        public IActionResult GetAllEvaluationPoint([FromQuery]String promID)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                return Json(Service.GetAllEvaluationPoint(Guid.Parse(promID)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetEvaluationSummary([FromQuery]String promUnitId, String promID)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                return Json(Service.GetEvaluationSummary(Guid.Parse(promUnitId) , Guid.Parse(promID)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult AutoAcceptApplication([FromQuery]String promUnitId, String promID)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                return Json(Service.AutoAcceptApplication(usr,Guid.Parse(promUnitId),  Guid.Parse(promID)));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetPromotionDocument([FromQuery]string promId, string promUnitId)
        {
            try
            {
                var document = new PortalModel.DocumentData();
                var data = new byte[0];
                var prom = Service.GetPromotionRequest(Guid.Parse(promId));
                foreach (var p in prom.promotionUnit)
                {
                    if (p.id.Equals(promUnitId))
                    {
                        document = p.documents[0];
                        data=Convert.FromBase64String(document.data);
                    }
                }
               
                return File(data, document.mime, null);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetProposalDocument([FromQuery] string promId, string investorId, int index)
        {
            try
            {
                var app = Service.GetApplication(Guid.Parse(promId), Guid.Parse(investorId));
                var document = app.proposalDocument[index];
                var data = Convert.FromBase64String(document.data);
                return File(data, document.mime, null);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult PostBondSubmissionDate([FromQuery] string promId, DateTime date)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.PromotionManager);
                Service.PostBondSubmissionDate(Guid.Parse(promId), date.Ticks);
                return Ok();
                
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        
        
        
    }
}