using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Workflows.Models;
using intapscamis.camis.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace intapscamis.camis.Controllers
{
    [Roles]
    public class LandBankController : BaseController
    {
        //Guid RequestLandRegistration(LandBankFacadeModel.LandData data);
        //List<LandBankFacadeModel.LandBankWorkItem> GetUserWorkItems();
        //LandBankFacadeModel.LandData GetWorkFlowLand(Guid wfid);
        //Guid ApproveRegistration(Guid wfid, String note);
        private readonly ILandBankFacade _facade;

        public LandBankController(ILandBankFacade facade)
        {
            _facade = facade;
        }
        [HttpPost]
        public IActionResult RequestLandRegistration([FromBody] LandBankFacadeModel.LandData data, [FromQuery]String wfid)
        {
            try
            {
                _facade.SetSession(GetSession());

                //var temp = data == null ? new LandBankFacadeModel.LandData() : data;
                
                //filter empty climate data
                if (data.Climate != null)
                {
                    data.Climate = data.Climate.Where(x => x != null && x.month != 0).ToList();
                }
                return Json(_facade.RequestLandRegistration(data, wfid).ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.InnerException);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetWorkFlowLand([FromQuery]String wfid)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetWorkFlowLand(Guid.Parse(wfid)));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult ApproveRegistration([FromQuery]String wfid, [FromBody]String note)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.ApproveRegistration(Guid.Parse(wfid), note).ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult ApprovePreparation([FromQuery]String wfid, [FromBody]String note)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.ApprovePreparation(Guid.Parse(wfid), note).ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetUserWorkItems()
        {
            try
            {
                _facade.SetSession(GetSession());
                var redo = false;
                var ws = _facade.GetUserWorkItems();
                foreach (var w in ws)
                {
                    if(w.workFlowType== (int)WorkflowTypes.PrepareLand)
                    {
                        _facade.GetPreparationStatus(Guid.Parse(w.wfid));
                        redo = true;
                    }
                }
                if (redo)
                    ws = _facade.GetUserWorkItems();
                return Json(_facade.GetUserWorkItems());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult SearchLand([FromBody]LandBankFacadeModel.LandSearchPar par)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.SearchLand(par));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetLand([FromQuery]String landId,[FromQuery]String upin,[FromQuery]bool geom,[FromQuery]bool dd)
        {
            try
            {
                _facade.SetSession(GetSession());
                if (String.IsNullOrEmpty(upin))
                    return Json(_facade.GetLand(Guid.Parse(landId),geom,dd));
                else
                {
                    var res=_facade.SearchLand(new LandBankFacadeModel.LandSearchPar() { Upin = upin });
                    if (res.Result.Count == 0)
                        return Json(null);
                    return Json(_facade.GetLand(Guid.Parse(res.Result[0].LandID), geom, dd));
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetLandCoordinate([FromQuery]String landId)
        {
            try
            {
                _facade.SetSession(GetSession());                
                return Json(_facade.GetLandCoordinate(Guid.Parse(landId)));
                
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult RejectRegistrationRequest([FromQuery]String wfid,[FromBody]LandBankFacadeModel.LandBankWorkItemNote note)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.RejectRegistrationRequest(Guid.Parse(wfid),note.note));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult CancelRegistrationRequest([FromQuery]String wfid, [FromBody]String note)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.CancelRegistrationRequest(Guid.Parse(wfid),note));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult RejectPreparationRequest([FromQuery]String wfid, [FromBody]String note)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.RejectLandPreparationRequest(Guid.Parse(wfid), note));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult CancelPreparationRequest([FromQuery]String wfid, [FromBody]String note)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.CancelLandPreparationRequest(Guid.Parse(wfid), note));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult RequestLandPreparation([FromBody]LandBankFacadeModel.LandPreparationRequest request)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.RequestLandPreparation(request));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetPreparationStatus([FromQuery]String wfid)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetPreparationStatus(Guid.Parse(wfid)));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult RequestLandTransfer([FromBody]LandBankFacadeModel.TransferRequest request)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.RequestLandTransfer(request));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }
        [HttpGet]
        public IActionResult GetTransaferStatus([FromQuery]String wfid)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetTransferStatus(Guid.Parse(wfid)));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetSplitWorkItem([FromQuery]String wfid)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetLastWorkItem<LandBankFacadeModel.SetPrepareGeometries>(Guid.Parse(wfid)));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { success = false, message = e.Message });
            }
        }

        [HttpPost]
        public  IActionResult GetLandData([FromBody] Guid[] ids)
        {
            try
            {
                _facade.SetSession(GetSession());
                var response = _facade.GetLandData(ids);
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
    }
}
