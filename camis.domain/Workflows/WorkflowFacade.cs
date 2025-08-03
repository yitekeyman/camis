using System;
using System.Collections.Generic;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Workflows.Models;

namespace intapscamis.camis.domain.Workflows
{
    public interface IWorkflowFacade : ICamisFacade
    {
        void SetSession(UserSession session);

        void CreateWorkflow(WorkflowRequest request);
        void UpdateWorkflow(Guid id, int currentState, string description = null);
        WorkflowResponse GetWorkflow(Guid id);
        IList<WorkflowResponse> GetUserWorkflows();

        void CreateWorkItem(WorkItemRequest request);
        IList<WorkItemResponse> GetWorkItems(Guid workflowId);
        WorkItemResponse GetLastWorkItem(Guid workflowId);
    }

    public class WorkflowFacade : CamisFacade, IWorkflowFacade
    {
        private readonly IWorkflowService _service;
        private UserSession _session;

        private readonly CamisContext _context;

        public WorkflowFacade(CamisContext context, IWorkflowService service)
        {
            _context = context;
            _service = service;
        }


        public void SetSession(UserSession session)
        {
            _session = session;
            _service.SetSession(_session);
        }


        public void CreateWorkflow(WorkflowRequest request)
        {
            Transact(_context, t =>
            {
                PassContext(_service, _context);
                _service.CreateWorkflow(request);
            });
        }

        public void UpdateWorkflow(Guid id, int currentState, string description = null)
        {
            Transact(_context, t =>
            {
                PassContext(_service, _context);
                _service.UpdateWorkflow(id, currentState, description);
            });
        }

        public WorkflowResponse GetWorkflow(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetWorkflow(id);
        }

        public IList<WorkflowResponse> GetUserWorkflows()
        {
            PassContext(_service, _context);
            return _service.GetUserWorkflows(_session);
        }


        public void CreateWorkItem(WorkItemRequest request)
        {
            Transact(_context, t =>
            {
                PassContext(_service, _context);
                _service.CreateWorkItem(request);
            });
        }

        public IList<WorkItemResponse> GetWorkItems(Guid workflowId)
        {
            PassContext(_service, _context);
            return _service.GetWorkItems(workflowId);
        }

        public WorkItemResponse GetLastWorkItem(Guid workflowId)
        {
            PassContext(_service, _context);
            return _service.GetLastWorkItem(workflowId);
        }
    }
}