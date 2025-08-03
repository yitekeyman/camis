using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using camis.aggregator.data.Entities;
using camis.aggregator.domain.Admin;
using camis.aggregator.domain.Report;
using camis.aggregator.web.Filter;
using intapscamis.camis.domain.Farms.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace camis.aggregator.web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : BaseController
    {
        private IReportService _report;
        private aggregatorContext _context;
        public ReportController(IReportService facade, aggregatorContext Context)
        {
            _report = facade;
            _context = Context;
            _report.SetContext(Context);
        }

        [Roles]
        [HttpGet]
        public IActionResult GetAllRegions()
        {
            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                var response = _report.GetAllRegions();
                return SuccessfulResponse(response);
            }
            catch (Exception ex)
            {

                return ErrorResponse(ex);
            }

        }

        [Roles]
        [HttpGet]
        public IActionResult GetZones(string regionid)
        {
            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                var response = _report.GetZones(regionid);
                return SuccessfulResponse(response);
            }
            catch (Exception ex)
            {

                return ErrorResponse(ex);
            }

        }

        [Roles]
        [HttpGet]
        public IActionResult GetWoredas(string zoneid)
        {
            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                var response = _report.GetWoredas(zoneid);
                return SuccessfulResponse(response);
            }
            catch (Exception ex)
            {

                return ErrorResponse(ex);
            }

        }

        [HttpPost]
        [Roles((long)UserRoles.Admin)]
        public IActionResult SetRegionUrl([FromBody] JObject data)
        {
            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                _report.SetRegionUrl(data["regionid"].ToString(), data["url"].ToString(),data["username"].ToString(), data["password"].ToString());
                return SuccessfulResponse(true);
            }
            catch (Exception ex)
            {

                return ErrorResponse(ex);
            }

        }

        [HttpGet]
        public IActionResult GetFarms(string region)
        {
            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                var response = _report.GetFarms(region);
                return SuccessfulResponse(response);
            }
            catch (Exception e)
            {
                return ErrorResponse(e);
            }
        }


        [HttpPost]
        [Roles((long)UserRoles.Admin)]
        public IActionResult UpdateRegionConfig(RegionConfigModel data)
        {
            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                _report.UpdateRegionConfig(data);
                return SuccessfulResponse(true);
            }
            catch (Exception ex)
            {

                return ErrorResponse(ex);
            }

        }

        [HttpPost]
        public IActionResult GetReport(ReportRequestModel Request)
        {
            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                var response = new ReportResponseModel(); 
                var s =  _report.GetReport(Request);
                if (s != null)
                    response = s;
                return View("ReportView",response);
            }
            catch (Exception ex)
            {

                return ErrorResponse(ex);
            }
        }
    }
}