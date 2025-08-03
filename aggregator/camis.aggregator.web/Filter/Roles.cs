using camis.aggregator.domain.Infrastructure;
using camis.aggregator.web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace camis.aggregator.web.Filter
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
                    context.Result = new JsonResult(new { status = 403, message = "Forbidden" }) { StatusCode = 403 };
            }

            catch (ArgumentNullException e)
            {
                Console.Error.WriteLine(e);
                context.Result = new JsonResult(new { status = 403, message = "Forbidden" }) { StatusCode = 403 };
            }
            
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
