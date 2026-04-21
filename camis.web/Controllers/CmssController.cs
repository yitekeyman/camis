using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using intapscamis.camis.Controllers;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Report;
using Microsoft.AspNetCore.Mvc;

namespace camis.web.Controllers
{
    public class CmssController : BaseController
    {
        
        private readonly ILandBankFacade _facade;
        public CmssController(ILandBankFacade facade)
        {
            _facade = facade;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
            //return Ok(id);
        }
        UserSession assertSession(String sid)
        {
            var us = AdminController.GetSession(sid);
            if (us == null)
                throw new UnauthorizedAccessException("User not logged in");
            return us;
        }
        [HttpGet]
        public IActionResult Home(String sid)
        {
            assertSession(sid);
            var ret=View((object)sid);
            return ret;
        }
        [HttpGet]
        public IActionResult TaskList(String sid)
        {
            assertSession(sid);
            var regionCode = _facade.GetRegionCode();
            var tl=_facade.GetSplitTaskList();
            if (regionCode.Equals("AM"))
            {
                tl = _facade.GetParcelSplitTaskList();
            }
            
            return View(tl);
        }
        [HttpGet]
        public IActionResult ProcessSplit(String taskid, String sid)
        {
            assertSession(sid);
            var data=_facade.GetSplitData(Guid.Parse(taskid));
            var regionCode = _facade.GetRegionCode();
            if (regionCode.Equals("AM"))
            {
                data = _facade.GetParcelSplitData(Guid.Parse(taskid));
            }
            return View(data);
        }
        class GetTaskGeomRes
        {
            public String error = null;
            public LandBankFacadeModel.SplitTaskGeom res = null;
        }
        class GetTaskGeomRes2
        {
            public String error = null;
            public List<LandBankFacadeModel.SplitTaskGeom> res = null;
        }
        [HttpGet]
        public IActionResult GetTaskGeom(String taskid)
        {
            try
            {
                var res = new object();
                var regionCode = _facade.GetRegionCode();
                if (regionCode.Equals("AM"))
                {
                    var data = _facade.GetParcelSplitTaskGeom(Guid.Parse(taskid));
                    res = new GetTaskGeomRes2()
                    {
                        res=data
                    };
                }
                else
                {
                    var data = _facade.GetTaskGeom(Guid.Parse(taskid));
                     res = new GetTaskGeomRes()
                    {
                        res=data
                    };
                }

               
                return Json(res);
            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
       

        
        [HttpPost]
        public IActionResult SplitParcel([FromBody] LandBankFacadeModel.SplitParcelData data,[FromQuery] String sid, [FromQuery] string note)
        {
            try
            {
                _facade.SetSession(this.assertSession(sid));
                var regionCode = _facade.GetRegionCode();
                if (regionCode.Equals("AM"))
                {
                    _facade.CmssDoneSplitting(data, Guid.Parse(data.taskID), note);
                }
                else
                {
                    _facade.SplitParcel(Guid.Parse(data.taskID),data.geoms);
                   
                }
                return Json(new { res="ok"});
              
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public class RejectData
        {
            public String taskId {get;set;}
            public String reason {get;set;}
            public string sid {get;set;}
        }
        [HttpPost]
        public IActionResult CmssRejectTask([FromBody] RejectData data)
        {
            try
            {
                _facade.SetSession(this.assertSession(data.sid));
                _facade.CmssRejectSplitting(Guid.Parse(data.taskId),data.reason);
                return Json(new { res="ok"});
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}