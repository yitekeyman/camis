using intaps.camisPortal.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace intaps.camisPortal.Controllers
{
    public class InvestorController:PortalControllerbase
    {
        [HttpPost]
        public IActionResult RegisterInvestor([FromBody]Entities.Investor investor)
        {
            try
            {
                assertSession();
                investor.UserName = GetSession().userName;
                return Json(Service.RegisterInvestor(investor));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateInvestor([FromBody]Entities.Investor investor)
        {
            try
            {
                assertSession();
                investor.UserName = GetSession().userName;
                Service.UpdateInvestor(investor);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult GetInvestors([FromBody]PortalModel.InvestorSearchPar par)
        {
            try
            {
                assertSession();
                return Json(Service.GetInvestors(par));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetInvestor([FromQuery]String userName)
        {
            try
            {
                assertSession();
                return Json(Service.GetInvestor(userName));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetInvestorById([FromQuery] String investorId)
        {
            try
            {
                assertSession();
                if (investorId != null && investorId !="undefined")
                {
                    return Json(Service.GetInvestorByID(Guid.Parse(investorId)));
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetLatestInvestor([FromQuery]String userName, string promotionUnitId)
        {
            try
            {
                assertSession();
                var user = assertUserRole(PortalModel.UserRole.PromotionManager);
                var pu = Service.GetPromotionUnit(Guid.Parse(promotionUnitId));
                var regionCode = pu.Promotion.Region;
                return Json(Service.GetLatestInvestor(user, userName, regionCode));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetRegistrationDoc([FromQuery] String investorId, int index)
        {
            try
            {
                var investor = Service.GetInvestorByID(Guid.Parse(investorId));
                var defaultValue = JsonConvert.DeserializeObject<PortalModel.DefaultProfile>(investor.DefaultProfile);
                var document = defaultValue.Registrations[index].Document;
                var data = Convert.FromBase64String(document.File);
                return File(data, document.Mimetype, null);
            }
            catch(Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
    }
}
