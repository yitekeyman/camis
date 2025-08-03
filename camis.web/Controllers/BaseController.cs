using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers
{
    public class BaseController : Controller
    {
        protected UserSession GetSession()
        {
            return HttpContext.Session.GetSession<UserSession>("sessionInfo");
        }
    }
}