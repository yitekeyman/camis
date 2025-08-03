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
                throw new UnauthorizedAccessException("User not loged in");
            return us;
        }
        [HttpGet]
        public IActionResult Home(String sid)
        {
            assertSession(sid);
            return View((object)sid);
        }
        [HttpGet]
        public IActionResult TaskList(String sid)
        {
            assertSession(sid);
            var tl=_facade.GetSplitTaskList();
            return View(tl);
        }
        [HttpGet]
        public IActionResult ProcessSplit(String taskid, String sid)
        {
            assertSession(sid);
            var data=_facade.GetSplitData(Guid.Parse(taskid));
            return View(data);
        }
        class GetTaskGeomRes
        {
            public String error = null;
            public LandBankFacadeModel.SplitTaskGeom res = null;
        }

        [HttpGet]
        public IActionResult GetTaskGeom(String taskid)
        {
            try
            {
                var data = _facade.GetTaskGeom(Guid.Parse(taskid));
                var res = new GetTaskGeomRes()
                {
                    res=data
                };
                return Json(res);
            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public class SplitParcelData
        {
            public String taskID;
            public List<String> geoms;
        }
        [HttpPost]
        public IActionResult SplitParcel([FromBody] SplitParcelData data,[FromQuery] String sid)
        {
            try
            {
                _facade.SetSession(this.assertSession(sid));
                _facade.SplitParcel(Guid.Parse(data.taskID),data.geoms);
                return Json(new { res="ok"});
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}