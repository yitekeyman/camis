using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using intaps.camisPortal.model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using intaps.camisPortal.Service;

namespace intaps.camisPortal.Controllers
{
    public class AddressesController:PortalControllerbase
    {
        private readonly AddressService _facade;


        public AddressesController()
        {
            _facade = Service.GetAddressService();
        }

        [HttpGet]
        public IActionResult AllSchemes()
        {
            try
            {
                return Json(_facade.GetAllSchemes());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return Json(new { errorCode = 500, message = e.Message });
            }
        }
        
        [HttpGet]
        public IActionResult AddressUnits(int schemeId)
        {
            try
            {
                return Json(_facade.GetAddressUnits(schemeId));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult Addresses(int schemeId, string parentId)
        {
            try
            {
                return Json(_facade.GetAddresses(schemeId, parentId==null?null as Guid?:Guid.Parse(parentId)));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return Json(new { errorCode = 500, message = e.Message });
            }
        }


        [HttpGet]
        public IActionResult AddressPairs(string leafId)
        {
            try
            {
                return Json(_facade.GetAddressPairs(Guid.Parse(leafId)));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return Json(new { errorCode = 500, message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult SaveAddress([FromBody] AddressServiceModel.CustomAddressRequest body)
        {
            try
            {
                return Json(_facade.SaveAddress(body));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
    }
}
