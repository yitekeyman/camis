using intapscamis.camis.domain.Farms;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace intapscamis.camis.Controllers
{
    public class ListProviderController: BaseController
    {
        private readonly IFarmsFacade _facade;

        public ListProviderController(IFarmsFacade facade)
        {
            _facade = facade;
        }
        [HttpGet]
        public IActionResult GetCamisList(String key)
        {
            try
            {
                switch (key)
                {
                    case "RegistrationType":
                        return Json(_facade.GetRegistrationTypes());
                }
                throw new InvalidOperationException($"Invalid camis list key {key}");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
    }
}
