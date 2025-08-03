using System;
using intaps.camisPortal.Service;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Projects.Models;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace intaps.camisPortal.Controllers
{
    public class ProjectsController : PortalControllerbase
    {
        [HttpGet]
        public IActionResult ActivityStatusTypes(string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.ActivityStatusTypes(usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityProgressMeasuringUnits(string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.ActivityProgressMeasuringUnits(usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityProgressVariables(string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.ActivityProgressVariables(usr,regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityProgressVariableTypes(string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.ActivityProgressVariableTypes(usr,regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityVariableValueLists(string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.ActivityVariableValueLists(usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpGet]
        public IActionResult ActivityPlan(string id, string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.ActivityPlan(id.ToGuid(), usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult PlanFromRootActivity(string id, string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.PlanFromRootActivity(id.ToGuid(), usr,regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult Activity(string id, string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.Activity(id.ToGuid(), usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ProgressReport(string id, string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.ProgressReport(id.ToGuid(), usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpGet]
        public IActionResult SearchReports(string id, string promotionUnitId, string term, int? skip, int? take)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.SearchReports(id.ToGuid(), term??"", skip??0, take??100, usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpGet]
        public IActionResult CalculateProgress(string id, string promotionUnitId, long? reportTime)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.CalculateProgress(id.ToGuid(), reportTime, usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult CalculateResourceProgress(string id, string promotionUnitId,long? reportTime)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.CalculateResourceProgress(id.ToGuid(), reportTime, usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult CalculateOutcomeProgress(string id, string promotionUnitId, long? reportTime)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.CalculateOutcomeProgress(id.ToGuid(), reportTime, usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpPost]
        public IActionResult SubmitNewProgressReport([FromBody]ActivityPlanRequest body, string promotionUnitId)
        {
            try
            {
                assertSession();
                string description = "self evaluation from CAMIS portal";
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                var workflowId = Service.SubmitNewProgressReport(body, description, usr, regionCode);
                return Json(new {success = true, workflowId});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpPut]
        public IActionResult RequestNewUpdatePlan([FromBody] ActivityPlanRequest body, string description, string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                var workflowId = Service.RequestNewUpdatePlan(body, description, usr, regionCode);
                return Json(new {success = true, workflowId});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpGet]
        public IActionResult ActivityPlanTemplates(string promotionUnitId)
        {
            try
            {
                assertSession();
                var usr = assertUserRole(PortalModel.UserRole.Public);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var p = Service.GetPromotion(pu.PromotionId);
                var regionCode = p.Region;
                return Json(Service.ActivityVariableValueLists(usr, regionCode));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
    }
}
