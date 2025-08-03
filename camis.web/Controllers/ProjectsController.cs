using System;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Projects;
using intapscamis.camis.domain.Projects.Models;
using intapscamis.camis.Filters;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers
{
    [Roles]
    public class ProjectsController : BaseController
    {
        private readonly IProjectFacade _facade;

        public ProjectsController(IProjectFacade facade)
        {
            _facade = facade;
        }
        
        
        [HttpGet]
        public IActionResult ActivityStatusTypes()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivityStatusTypes());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityProgressMeasuringUnits()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivityProgressMeasuringUnits());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityProgressVariables()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivityProgressVariables());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityProgressVariableTypes()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivityProgressVariableTypes());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityVariableValueLists()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivityVariableValueLists());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityTags()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivityTags());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityPlanDetailTags()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivityPlanDetailTags());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpGet]
        public IActionResult ActivityPlan(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivityPlan(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult PlanFromRootActivity(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetPlanFromRootActivity(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult Activity(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetActivity(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ProgressReport(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetProgressReport(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [HttpGet]
        public IActionResult SearchReports(string id, string term, int? skip, int? take)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.SearchReports(id.ToGuid(), term??"", skip??0, take??100));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        
        [HttpGet]
        public IActionResult CalculateProgress(string id, long? reportTime)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(new {value = _facade.CalculateProgress(id.ToGuid(), reportTime)});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult CalculateResourceProgress(string id, long? reportTime)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.CalculateResourceProgress(id.ToGuid(), reportTime));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult CalculateOutcomeProgress(string id, long? reportTime)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.CalculateOutcomeProgress(id.ToGuid(), reportTime));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [Roles(UserRoles.MnESupervisor, UserRoles.MnEExpert, UserRoles.MnEDataEncoder, UserRoles.FarmClerk, UserRoles.FarmSupervisor)]
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
        public IActionResult InWorkItemReportFile(string id, string documentId)
        {
            try
            {
                _facade.SetSession(GetSession());
                var doc = _facade.InWorkItemReportFile(id.ToGuid(), documentId.ToGuid());
                return File(doc.File, doc.Mimetype, null); // the filename is null to support in-browser view
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        

        [Roles(UserRoles.MnESupervisor)]
        [HttpPost]
        public IActionResult RequestNewProgressReport([FromBody] ActivityPlanRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                var workflowId = _facade.RequestNewProgressReport(body, description);
                return Json(new {success = true, workflowId});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [Roles(UserRoles.MnEExpert)]
        [HttpPost]
        public IActionResult AcceptProgressReport(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.AcceptProgressReport(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnEExpert)]
        [HttpPost]
        public IActionResult SurveyProgressReport(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.SurveyProgressReport(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnEExpert)]
        [HttpPost]
        public IActionResult SurveyedProgressReport(string id, [FromBody] ActivityPlanRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.SurveyedProgressReport(id.ToGuid(), body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnEDataEncoder)]
        [HttpPost]
        public IActionResult EncodeProgressReport(string id, [FromBody] ActivityPlanRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.EncodeProgressReport(id.ToGuid(), body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnEExpert)]
        [HttpPost]
        public IActionResult RejectProgressReport(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.RejectProgressReport(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnEExpert, UserRoles.MnEDataEncoder)]
        [HttpPost]
        public IActionResult SubmitProgressReport(string id, [FromBody] ActivityPlanRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.SubmitProgressReport(id.ToGuid(), body, description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnEExpert, UserRoles.MnEDataEncoder)]
        [HttpPost]
        public IActionResult SubmitNewProgressReport([FromBody] ActivityPlanRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                
                var workflowId = _facade.SubmitNewProgressReport(body, description);
                return Json(new {success = true, workflowId});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnEExpert)]
        [HttpPost]
        public IActionResult ReportProgressReport(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.ReportProgressReport(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnEExpert, UserRoles.MnESupervisor)]
        [HttpPost]
        public IActionResult CancelProgressReport(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.CancelProgressReport(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

        [Roles(UserRoles.MnESupervisor)]
        [HttpPost]
        public IActionResult ApproveProgressReport(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.ApproveProgressReport(id.ToGuid(), description);
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
        public IActionResult RequestNewUpdatePlan([FromBody] ActivityPlanRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                var workflowId = _facade.RequestNewUpdatePlan(body, description);
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
        public IActionResult CancelUpdatePlan(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.CancelUpdatePlan(id.ToGuid(), description);
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
        public IActionResult RequestUpdatePlan(string id, [FromBody] ActivityPlanRequest body, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.RequestUpdatePlan(id.ToGuid(), body, description);
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
        public IActionResult RejectUpdatePlan(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.RejectUpdatePlan(id.ToGuid(), description);
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
        public IActionResult ApproveUpdatePlan(string id, string description)
        {
            try
            {
                _facade.SetSession(GetSession());
                _facade.ApproveUpdatePlan(id.ToGuid(), description);
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        
        [Roles(UserRoles.ConfigurationAdmin)]
        [HttpPost]
        public IActionResult ActivityPlanTemplate([FromBody] ActivityPlanTemplateRequest body)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.CreateActivityPlanTemplate(body));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [HttpGet]
        public IActionResult ActivityPlanTemplates()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetAllActivityPlanTemplates());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [Roles(UserRoles.ConfigurationAdmin)]
        [HttpPut]
        public IActionResult ActivityPlanTemplate(string id, [FromBody] ActivityPlanTemplateRequest body)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.UpdateActivityPlanTemplate(id.ToGuid(), body));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
        
        [Roles(UserRoles.ConfigurationAdmin)]
        [HttpDelete]
        public IActionResult ActivityPlanTemplate(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.DeleteActivityPlanTemplate(id.ToGuid()));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
    }
}