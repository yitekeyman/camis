using System;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.System.Addresses;
using intapscamis.camis.domain.System.Addresses.Models;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers
{
    public class AddressesController : BaseController
    {
        private readonly IAddressFacade _facade;

        public AddressesController(IAddressFacade facade)
        {
            _facade = facade;
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
                return StatusCode(500, new {success = false, message = e.Message});
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
                return Json(_facade.GetAddresses(schemeId, parentId?.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }


        [HttpGet]
        public IActionResult AddressPairs(string leafId)
        {
            try
            {
                return Json(_facade.GetAddressPairs(leafId.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }


        [HttpPost]
        public IActionResult SaveAddress([FromBody] CustomAddressRequest body)
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