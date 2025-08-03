using intapscamis.camis.domain.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers
{
    public class LookupController : BaseController
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService service)
        {
            _lookupService = service;
        }

        [HttpGet]
        public IActionResult Role()
        {
            return Json(_lookupService.GetRoles());
        }
    }
}