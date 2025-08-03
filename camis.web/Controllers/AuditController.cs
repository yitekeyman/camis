using System;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.Filters;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers
{
    [Roles(UserRoles.Admin)]
    public class AuditController : BaseController
    {
        private readonly IUserFacade _userFacade;

        public AuditController(IUserFacade facade)
        {
            _userFacade = facade;
        }

        [HttpGet]
        public IActionResult GetAudit()
        {
            try
            {
                return Json(_userFacade.GetAllAction(GetSession()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {message = "Internal server error occurred while fetching the results"});
            }
        }
    }
}