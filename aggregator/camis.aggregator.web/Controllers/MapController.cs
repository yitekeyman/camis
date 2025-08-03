using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using camis.aggregator.domain.LandBank;
using Microsoft.AspNetCore.Mvc;

namespace camis.aggregator.web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MapController : BaseController
    {
        private ILandBankFacade _facade;
        public MapController(ILandBankFacade facade)
        {
            _facade = facade;
        }

        [HttpGet]
        public IActionResult GetLandMapBound()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetLandMapBound());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        public class GSClient
        {
            String urlBase = "http://localhost:8080/ows"; // todo: may not be localhost
            public GSResponse GetJson(String cmd)
            {
                var client = new HttpClient();
                var res = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, urlBase + cmd));
                res.Wait();
                if (res.Result == null)
                {
                    return new GSResponse() { error = "no result returned from geoserver" };
                }
                var contentType = "";

                foreach (var h in res.Result.Content.Headers)
                {
                    if (h.Key.IndexOf("Content-Type") != -1)
                    {
                        contentType = h.Value.First();
                        break;
                    }
                }
                var msg = res.Result.Content.ReadAsStringAsync();
                msg.Wait();


                if (contentType.IndexOf("application/json") != -1)
                {
                    return new GSResponse() { response = Newtonsoft.Json.JsonConvert.DeserializeObject(msg.Result) };
                }
                else
                {
                    return new GSResponse() { error = msg.Result };
                }
            }
        }
        public class GSResponse
        {
            public object response;
            public String error;
        }

        [HttpGet]
        public IActionResult WfsGet()
        {
            try
            {
                var cmd = base.HttpContext.Request.QueryString.ToString();
                GSClient client = new GSClient();
                var resp = client.GetJson(cmd);
                return Json(resp);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

    }
}