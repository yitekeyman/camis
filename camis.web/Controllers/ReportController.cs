using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using intapscamis.camis.Controllers;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Report;
using Microsoft.AspNetCore.Mvc;
using camis.reportviews.MyFeature.Pages;
using Newtonsoft.Json;

namespace camis.web.Controllers
{
    public class ReportController : BaseController
    {
        private IReportService _report;    
        private CamisContext _context;
        public ReportController(IReportService facade,CamisContext Context)
        {
            _report = facade;
            _context = Context;
        }

        public IActionResult PlanForm(Guid id)
        {
            _report.SetContext(_context);
            //_report.SetSession(GetSession());
             var plan = _report.GetPlanViewModel(id);
            return View(plan);
            //return Ok(id);
        }
        [HttpPost]
        public IActionResult GenerateReport([FromQuery]String report_name,[FromBody]String parameter)
        {
            var v = View(report_name, (object)(parameter ?? ""));
            return v;
        }
        

        [HttpPost]
        public IActionResult GetReport([FromBody] ReportRequestModel Model)
        {

            _report.SetContext(_context);
            _report.SetSession(GetSession());
            var response = _report.GenerateReport(Model);
            response.Request = Model;
            return View("ReportView",response);

        }

        

        [HttpPost]
        public IActionResult GetReport2([FromBody] ReportRequestModel Model)
        {

            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                if(Model.FilteredBy == FilteredBy.Region)
                {
                    Model.FilteredBy = FilteredBy.None;
                }
                var response = _report.GenerateReport(Model);
                response.Request = Model;
                //var res = response;
                var res = (JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                }));
                return Ok(new
                {
                    status = true,
                    response = res,
                    message = ""
                });
            }
            catch (Exception ex)
            {

                return Ok(new
                {
                    status = false,
                    response = "",
                    message = ex.Message
                });
            }


        }

        [HttpGet]
        public IActionResult GetAllRegions()
        {
            _report.SetContext(_context);
            _report.SetSession(GetSession());
            var response = _report.GetAllRegions();
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAllFarms()
        {
            _report.SetContext(_context);
            _report.SetSession(GetSession());
            var response = _report.GetAllFarms();
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAllFarms2()
        {
            try
            {
                _report.SetContext(_context);
                _report.SetSession(GetSession());
                var response = _report.GetAllFarms();
                var res = JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.None,
                                    new JsonSerializerSettings
                                    {
                                        NullValueHandling = NullValueHandling.Ignore
                                    });
                return Ok(new
                {
                    status = true,
                    response = res,
                    message = ""
                });
            
            }
            catch (Exception ex)
            {

                return Ok(new
                {
                    status = false,
                    response = "",
                    message = ex.Message
                });
            }


        }


        [HttpGet]
        public IActionResult GetZones(string regionid)
        {
            _report.SetContext(_context);
            _report.SetSession(GetSession());
            var response = _report.GetZones(regionid);
            return Ok(response);
        }


        [HttpGet]
        public IActionResult GetWoredas(string zoneid)
        {
            _report.SetContext(_context);
            _report.SetSession(GetSession());
            var response = _report.GetWoredas(zoneid);
            return Ok(response);
        }

        //[HttpGet]
        //public IActionResult GetLandReport()
        //{
        //    _
        //}

        //[HttpPost] 
        //public IActionResult Download([FromBody] ReportResponseModel Model)
        //{
        //    _exporter.SetContext(_context);
        //    var stream = _exporter.GetReportDocument(Model);
        //    return File(stream, "application/msword");
        //}
    }
}