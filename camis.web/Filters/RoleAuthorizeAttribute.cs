using System;
using System.Linq;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace intapscamis.camis.Filters
{
    public class Roles : ActionFilterAttribute
    {
        public Roles(params long[] values)
        {
            AllowedUsers = values;
        }

        public long[] AllowedUsers { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            try
            {
                var userSession = session.GetSession<UserSession>("sessionInfo");
                userSession.LastSeen = DateTime.Now;
                session.SetSession("sessionInfo", userSession);

                if (AllowedUsers == null || AllowedUsers.Length <= 0) return;
                
                if (!AllowedUsers.Contains(userSession.Role))
                    context.Result = new JsonResult(new {status = 403, message = "Forbidden"}) {StatusCode = 403};
            }

            catch (ArgumentNullException e)
            {
                Console.Error.WriteLine(e);
                context.Result = new BadRequestObjectResult(new {status = 403, message = "Forbidden"});
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}