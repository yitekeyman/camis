using System;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.Filters;
using Microsoft.AspNetCore.Mvc;

namespace intapscamis.camis.Controllers
{
    [Roles]
    public class WorkflowsController : BaseController
    {
        private readonly IWorkflowFacade _facade;

        public WorkflowsController(IWorkflowFacade facade)
        {
            _facade = facade;
        }


        [HttpGet]
        public IActionResult UserWorkflows()
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetUserWorkflows());
            }
            catch (Exception e)
            {
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }


        [HttpGet]
        public IActionResult WorkItems(string id)
        {
            try
            {
                _facade.SetSession(GetSession());
                return Json(_facade.GetWorkItems(id.ToGuid()));
            }
            catch (Exception e)
            {
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }

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
                return StatusCode(500, new {success = false, message = e.Message});
            }
        }
    }
}