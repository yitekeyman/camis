using intapscamis.camis.domain.LandBank;
using intapscamis.camis.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace intapscamis.camis.Controllers
{
    [Roles]
    public class MapController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILandBankFacade _facade;

        public MapController(IConfiguration configuration, ILandBankFacade facade)
        {
            _configuration = configuration;
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
            public GSResponse GetJson(string baseUrl, string cmd)
            {
                var client = new HttpClient();
                var res = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, baseUrl + cmd));
                res.Wait();
                if(res.Result==null)
                {
                    return new GSResponse() { error = "no result returned from geoserver" };
                }
                var contentType = "";
                
                foreach (var h in res.Result.Content.Headers)
                {
                    if (h.Key.IndexOf("Content-Type")!=-1)
                    {
                        contentType = h.Value.First() ;
                        break;
                    }
                }
                var msg= res.Result.Content.ReadAsStringAsync();
                msg.Wait();

                
                if (contentType.IndexOf("application/json")!=-1)
                {
                    return new GSResponse() { response = Newtonsoft.Json.JsonConvert.DeserializeObject(msg.Result)};
                }
                else 
                {
                    return new GSResponse() { error= msg.Result };
                }
            }
        }
        public class GSResponse
        {
            public object response;
            public String error;
        }
        [HttpGet]
        public IActionResult WorkFlowGeom([FromQuery] String wfid)
        {
            try
            {
                var wfland=_facade.GetWorkFlowLand(Guid.Parse(wfid));
                String wkt = wfland.parcels[wfland.Upins[0]].geometry;
                return Json(wkt);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult WfsGet()
        {
            try
            {
                var geoserverBaseUrl = _configuration.GetConnectionString("geoserver_base_url");
                var cmd = base.HttpContext.Request.QueryString.ToString();
                GSClient client = new GSClient();
                var resp = client.GetJson(geoserverBaseUrl, cmd);
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
