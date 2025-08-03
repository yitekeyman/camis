using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using camis.aggregator.domain.Infrastructure;
using camis.aggregator.web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace camis.aggregator.web.Controllers
{
    public class BaseController : Controller
    {
        public UserSession GetSession()
        {

            var session = HttpContext.Session.GetString("sessionInfo");
            if (session == null)
            {
                throw new NotAuthenticatedException("Not Authenticated");
            }
            return JsonConvert.DeserializeObject<UserSession>(session);
        }


        public IActionResult ErrorResponse(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.ToString());

            if (ex is NotAuthenticatedException)
            {
                return StatusCode(403, ex.Message);
            }
            else
            {
                return StatusCode(501, ex.Message);
            }

        }

        public IActionResult SuccessfulResponse(Object response)
        {
            return Ok(response);

        }
    }
}