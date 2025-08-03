using System;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Farms;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.Filters;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers
{
    [Roles]
    public class FarmsController : BaseController
    {
        private readonly IFarmsFacade _facade;

        public FarmsController(IFarmsFacade facade)
        {
            _facade = facade;
        }


        [HttpGet]
        public IActionResult FarmOperatorTypes()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetFarmOperatorTypes());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult FarmTypes()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetFarmTypes());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult RegistrationAuthorities()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetRegistrationAuthorities());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult RegistrationTypes()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetRegistrationTypes());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult FarmOperatorOrigins()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetFarmOperatorOrigins());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult FarmOperators(int? skip, int? take)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetFarmOperators(skip??0, take??100));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult Farms(int? skip, int? take)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetFarms(skip??0, take??100));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult UPINs()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetUPINs());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        
        [HttpGet]
        public IActionResult SearchFarmOperators(string term, int? skip, int? take)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.SearchFarmOperators(term??"", skip??0, take??100));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult SearchFarms(string term, int? skip, int? take)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.SearchFarms(term??"", skip??0, take??100));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpGet]
        public IActionResult FarmOperator(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetFarmOperator(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult Farm(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetFarm(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult FarmByActivity(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetFarmByActivity(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [Roles(UserRoles.FarmClerk, UserRoles.FarmSupervisor, UserRoles.LandAdmin, UserRoles.LandCertificateIssuer)]
        [HttpGet]
        public IActionResult LastWorkItem(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetLastWorkItem(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult InWorkItemRegistrationFile(string id, int regId)
        {
            try
            {
                _facade.SetSession(GetSession());
                var doc = _facade.InWorkItemRegistrationFile(id.ToGuid(), regId);
                return File(doc.File, doc.Mimetype, null); // the filename is null to support in-browser view
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult InWorkItemOperatorRegistrationFile(string id, int regId)
        {
            try
            {
                _facade.SetSession(GetSession());
                var doc = _facade.InWorkItemOperatorRegistrationFile(id.ToGuid(), regId);
                return File(doc.File, doc.Mimetype, null); // the filename is null to support in-browser view
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult InWorkItemActivityPlanFile(string id, string documentId)
        {
            try
            {
                _facade.SetSession(GetSession());
                var doc = _facade.InWorkItemActivityPlanFile(id.ToGuid(), documentId.ToGuid());
                return File(doc.File, doc.Mimetype, null); // the filename is null to support in-browser view
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult InWorkItemActivityPlanFileForPlanUpdate(string id, string documentId)
        {
            try
            {
                _facade.SetSession(GetSession());
                var doc = _facade.InWorkItemActivityPlanFileForPlanUpdate(id.ToGuid(), documentId.ToGuid());
                return File(doc.File, doc.Mimetype, null); // the filename is null to support in-browser view
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }


        [Roles(UserRoles.FarmClerk)]
        [HttpPost]
        public IActionResult SaveNewFarmRegistration([FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                var workflowId = _facade.SaveNewFarmRegistration(body, description);
                return Json(new {success = true, workflowId});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmClerk)]
        [HttpPost]
        public IActionResult SaveFarmRegistration(string id, [FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.SaveFarmRegistration(id.ToGuid(), body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmClerk)]
        [HttpPost]
        public IActionResult RequestNewFarmRegistration([FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                var workflowId = _facade.RequestNewFarmRegistration(body, description);
                return Json(new {success = true, workflowId});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmClerk)]
        [HttpPost]
        public IActionResult CancelFarmRegistration(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.CancelFarmRegistration(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmClerk)]
        [HttpPost]
        public IActionResult RequestFarmRegistration(string id, [FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.RequestFarmRegistration(id.ToGuid(), body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmSupervisor)]
        [HttpPost]
        public IActionResult RejectFarmRegistration(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.RejectFarmRegistration(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmSupervisor)]
        [HttpPost]
        public IActionResult ApproveFarmRegistration(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.ApproveFarmRegistration(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }


        [Roles(UserRoles.FarmClerk)]
        [HttpPut]
        public IActionResult RequestNewFarmModification([FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                var workflowId = _facade.RequestNewFarmModification(body, description);
                return Json(new {success = true, workflowId});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmClerk)]
        [HttpPut]
        public IActionResult CancelFarmModification(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.CancelFarmModification(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmClerk)]
        [HttpPut]
        public IActionResult RequestFarmModification(string id, [FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.RequestFarmModification(id.ToGuid(), body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [Roles(UserRoles.FarmSupervisor)]
        [HttpPut]
        public IActionResult RejectFarmModification(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.RejectFarmModification(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmSupervisor)]
        [HttpPut]
        public IActionResult ApproveFarmModification(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.ApproveFarmModification(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }


        [Roles(UserRoles.FarmClerk)]
        [HttpDelete]
        [HttpPut]
        public IActionResult RequestNewFarmDeletion([FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                var workflowId = _facade.RequestNewFarmDeletion(body, description);
                return Json(new {success = true, workflowId});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmSupervisor)]
        [HttpDelete]
        [HttpPut]
        public IActionResult RejectFarmDeletion(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.RejectFarmDeletion(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.FarmSupervisor)]
        [HttpDelete]
        [HttpPut]
        public IActionResult ApproveFarmDeletion(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.ApproveFarmDeletion(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        

        [Roles(UserRoles.LandAdmin)]
        [HttpPost]
        public IActionResult NewWaitLandAssignment([FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.NewWaitLandAssignment(body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [Roles(UserRoles.LandAdmin)]
        [HttpPost]
        public IActionResult WaitLandAssignment(string id, [FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.WaitLandAssignment(id.ToGuid(), body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [Roles(UserRoles.LandAdmin, UserRoles.LandCertificateIssuer)]
        [HttpGet]
        public IActionResult TransferStatus(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                var status = _facade.GetTransferStatus(id.ToGuid());
                return Json(new {success = true, status});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.LandCertificateIssuer)]
        [HttpPost]
        public IActionResult CertifyLandAssignment(string id, [FromBody] FarmRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.CertifyLandAssignment(id.ToGuid(), body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
    }
}