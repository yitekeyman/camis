using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace intaps.camisPortal.Controllers
{
    public class ListController:PortalControllerbase
    {
        [HttpGet]
        public IActionResult GetRegions()
        {
            try
            {
                return Json(Service.GetRegions());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }

        }
        [HttpGet]
        public IActionResult GetRegion(String code)
        {
            try
            {
                return Json(Service.GetRegion(code));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetCamisList(String key)
        {
            try
            {
                return Json(Service.GetCamisList(key));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
    }
}
