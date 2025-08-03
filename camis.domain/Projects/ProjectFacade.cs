using System;
using System.Collections.Generic;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Projects.Models;
using intapscamis.camis.domain.Projects.Workflows;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;

namespace intapscamis.camis.domain.Projects
{
    public interface IProjectFacade : ICamisFacade
    {
        void SetSession(UserSession session);
        
        IList<ActivityStatusTypeResponse> GetActivityStatusTypes();
        IList<ActivityProgressMeasuringUnitResponse> GetActivityProgressMeasuringUnits();
        IList<ActivityProgressVariableResponse> GetActivityProgressVariables();
        IList<ActivityProgressVariableTypeResponse> GetActivityProgressVariableTypes();
        IList<ActivityVariableValueListResponse> GetActivityVariableValueLists();
        IList<string> GetActivityTags();
        IList<string> GetActivityPlanDetailTags();

        ActivityPlanResponse GetActivityPlan(Guid id);
        ActivityPlanResponse GetPlanFromRootActivity(Guid id);
        ActivityResponse GetActivity(Guid id);
        ActivityProgressReportResponse GetProgressReport(Guid id);

        PaginatorResponse<ActivityProgressReportResponse> SearchReports(Guid planId, string term, int skip, int take);

        double CalculateProgress(Guid activityId, long? reportTime);
        IList<CalculatedVariableProgressResponse> CalculateResourceProgress(Guid activityId, long? reportTime);
        IList<CalculatedVariableProgressResponse> CalculateOutcomeProgress(Guid activityId, long? reportTime);

        WorkItemResponse GetLastWorkItem(Guid workflowId);
        Document InWorkItemReportFile(Guid workItemId, Guid documentId);
        
        Guid RequestNewProgressReport(ActivityPlanRequest body, string description);
        void AcceptProgressReport(Guid workflowId, string description);
        void SurveyProgressReport(Guid workflowId, string description);
        void SurveyedProgressReport(Guid workflowId, ActivityPlanRequest body, string description);
        void EncodeProgressReport(Guid workflowId, ActivityPlanRequest body, string description);
        void RejectProgressReport(Guid workflowId, string description);
        void SubmitProgressReport(Guid workflowId, ActivityPlanRequest body, string description);
        Guid SubmitNewProgressReport(ActivityPlanRequest body, string description);
        void ReportProgressReport(Guid workflowId, string description);
        void CancelProgressReport(Guid workflowId, string description);
        void ApproveProgressReport(Guid workflowId, string description);
        
        Guid RequestNewUpdatePlan(ActivityPlanRequest body, string description);
        void CancelUpdatePlan(Guid workflowId, string description);
        void RequestUpdatePlan(Guid workflowId, ActivityPlanRequest body, string description);
        void RejectUpdatePlan(Guid workflowId, string description);
        void ApproveUpdatePlan(Guid workflowId, string description);

        ActivityPlanTemplateResponse CreateActivityPlanTemplate(ActivityPlanTemplateRequest body);
        IList<ActivityPlanTemplateResponse> GetAllActivityPlanTemplates();
        ActivityPlanTemplateResponse UpdateActivityPlanTemplate(Guid templateId, ActivityPlanTemplateRequest body);
        ActivityPlanTemplateResponse DeleteActivityPlanTemplate(Guid templateId);
    }

    public class ProjectFacade : CamisFacade, IProjectFacade
    {
        private UserSession _session;
        
        private readonly IProjectService _service;
        private readonly IProgressReportWorkflow _progressReportWorkflow;
        private readonly UpdatePlanWorkflow _updatePlanWorkflow;

        private readonly CamisContext _context;

        public ProjectFacade(
            CamisContext context, 
            IProjectService service,
            IProgressReportWorkflow progressReportWorkflow,
            IWorkflowService workflowService,
            IFarmsService farmsService
        )
        {
            _context = context;
            _service = service;
            _progressReportWorkflow = progressReportWorkflow;
            _updatePlanWorkflow = new UpdatePlanWorkflow(service, workflowService, farmsService);
        }

        public void SetSession(UserSession session)
        {
            _session = session;
            _service.SetSession(_session);
            _progressReportWorkflow.SetSession(_session);
            _updatePlanWorkflow.SetSession(_session);
        }


        public IList<ActivityStatusTypeResponse> GetActivityStatusTypes()
        {
            PassContext(_service, _context);
            return _service.GetActivityStatusTypes();
        }
        
        public IList<ActivityProgressMeasuringUnitResponse> GetActivityProgressMeasuringUnits()
        {
            PassContext(_service, _context);
            return _service.GetActivityProgressMeasuringUnits();
        }

        public IList<ActivityProgressVariableResponse> GetActivityProgressVariables()
        {
            PassContext(_service, _context);
            return _service.GetActivityProgressVariables();
        }

        public IList<ActivityProgressVariableTypeResponse> GetActivityProgressVariableTypes()
        {
            PassContext(_service, _context);
            return _service.GetActivityProgressVariableTypes();
        }

        public IList<ActivityVariableValueListResponse> GetActivityVariableValueLists()
        {
            PassContext(_service, _context);
            return _service.GetActivityVariableValueLists();
        }

        public IList<string> GetActivityTags()
        {
            PassContext(_service, _context);
            return _service.GetActivityTags();
        }

        public IList<string> GetActivityPlanDetailTags()
        {
            PassContext(_service, _context);
            return _service.GetActivityPlanDetailTags();
        }


        public ActivityPlanResponse GetActivityPlan(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetActivityPlan(id);
        }
        
        public ActivityPlanResponse GetPlanFromRootActivity(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetPlanFromRootActivity(id);
        }
        
        public ActivityResponse GetActivity(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetActivity(id);
        }
        
        public ActivityProgressReportResponse GetProgressReport(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetProgressReport(id);
        }
        
        
        public PaginatorResponse<ActivityProgressReportResponse> SearchReports(Guid planId, string term, int skip, int take)
        {
            PassContext(_service, _context);
            return _service.SearchReports(planId, term, skip, take);
        }

        
        public double CalculateProgress(Guid activityId, long? reportTime)
        {
            PassContext(_service, _context);
            return _service.CalculateProgress(activityId, reportTime);
        }

        public IList<CalculatedVariableProgressResponse> CalculateResourceProgress(Guid activityId, long? reportTime)
        {
            PassContext(_service, _context);
            return _service.CalculateResourceProgress(activityId, reportTime);
        }

        public IList<CalculatedVariableProgressResponse> CalculateOutcomeProgress(Guid activityId, long? reportTime)
        {
            PassContext(_service, _context);
            return _service.CalculateOutcomeProgress(activityId, reportTime);
        }

        
        public WorkItemResponse GetLastWorkItem(Guid workflowId)
        {
            PassContext(_service, _context);
            return _service.GetLastWorkItem(workflowId);
        }
        
        public Document InWorkItemReportFile(Guid workItemId, Guid documentId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemReportFile(workItemId, documentId);
        }

        
        public Guid RequestNewProgressReport(ActivityPlanRequest body, string description)
        {
            return Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine();

                var workflowId = _progressReportWorkflow.Workflow.Id;
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Request a new progress report.",
                    null);
                return workflowId;
            });
        }

        public void AcceptProgressReport(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Accept,
                    description ?? "Accept a new progress report request.",
                    null);
            });
        }

        public void SurveyProgressReport(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Survey,
                    description ?? "Start survey of a progress report.",
                    null);
            });
        }

        public void SurveyedProgressReport(Guid workflowId, ActivityPlanRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Surveyed, body,
                    description ?? "Complete survey of a progress report.",
                    null);
            });
        }

        public void EncodeProgressReport(Guid workflowId, ActivityPlanRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Encode, body,
                    description ?? "Encode data of a progress report.",
                    null);
            });
        }

        public void RejectProgressReport(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Reject,
                    description ?? "Reject data encoding of a progress report.",
                    null);
            });
        }
        
        public void SubmitProgressReport(Guid workflowId, ActivityPlanRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Submit, body,
                    description ?? "Submitted encoded survey data of a progress report from the mobile app.",
                    null);
            });
        }
        
        public Guid SubmitNewProgressReport(ActivityPlanRequest body, string description)
        {
            return Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine();
                
                var workflowId = _progressReportWorkflow.Workflow.Id;
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Submit, body,
                    description ?? "Submitted encoded survey data of a progress report from the portal app.",
                    null);
                return workflowId;
            });
        }

        public void ReportProgressReport(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Report,
                    description ?? "Review and report data encoding of a progress report.",
                    null);
            });
        }

        public void CancelProgressReport(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Cancel,
                    description ?? "Cancel a progress report.",
                    null);
            });
        }

        public void ApproveProgressReport(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_progressReportWorkflow, _context);
                _progressReportWorkflow.ConfigureMachine(workflowId);
                
                _progressReportWorkflow.Fire(workflowId, ProgressReportWorkflow.ParameterizedTriggers.Approve,
                    description ?? "Approve a progress report.",
                    null);
            });
        }

        
        public Guid RequestNewUpdatePlan(ActivityPlanRequest body, string description)
        {
            return Transact(_context, t =>
            {
                PassContext(_updatePlanWorkflow, _context);
                _updatePlanWorkflow.ConfigureMachine();

                var workflowId = _updatePlanWorkflow.Workflow.Id;

                _updatePlanWorkflow.Fire(workflowId, UpdatePlanWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Request a new plan update.",
                    null);

                return workflowId;
            });
        }

        public void CancelUpdatePlan(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_updatePlanWorkflow, _context);
                _updatePlanWorkflow.ConfigureMachine(workflowId);

                _updatePlanWorkflow.Fire(_updatePlanWorkflow.Workflow.Id, UpdatePlanWorkflow.ParameterizedTriggers.Cancel,
                    description ?? "Cancel a plan update.",
                    null);
            });
        }

        public void RequestUpdatePlan(Guid workflowId, ActivityPlanRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_updatePlanWorkflow, _context);
                _updatePlanWorkflow.ConfigureMachine(workflowId);

                _updatePlanWorkflow.Fire(_updatePlanWorkflow.Workflow.Id, UpdatePlanWorkflow.ParameterizedTriggers.Request,
                    body, description ?? "Request a plan update.",
                    null);
            });
        }

        public void RejectUpdatePlan(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_updatePlanWorkflow, _context);
                _updatePlanWorkflow.ConfigureMachine(workflowId);

                _updatePlanWorkflow.Fire(_updatePlanWorkflow.Workflow.Id, UpdatePlanWorkflow.ParameterizedTriggers.Reject,
                    description ?? "Reject a plan update.",
                    null);
            });
        }

        public void ApproveUpdatePlan(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_updatePlanWorkflow, _context);
                _updatePlanWorkflow.ConfigureMachine(workflowId);

                _updatePlanWorkflow.Fire(_updatePlanWorkflow.Workflow.Id, UpdatePlanWorkflow.ParameterizedTriggers.Approve,
                    description ?? "Approve a plan update.",
                    null);
            });
        }


        public ActivityPlanTemplateResponse CreateActivityPlanTemplate(ActivityPlanTemplateRequest body)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.CreateActivityPlanTemplate(body);
            });
        }

        public IList<ActivityPlanTemplateResponse> GetAllActivityPlanTemplates()
        {
            PassContext(_service, _context);
            return _service.GetAllActivityPlanTemplates();
        }

        public ActivityPlanTemplateResponse UpdateActivityPlanTemplate(Guid templateId, ActivityPlanTemplateRequest body)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.UpdateActivityPlanTemplate(templateId, body);
            });
        }

        public ActivityPlanTemplateResponse DeleteActivityPlanTemplate(Guid templateId)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.DeleteActivityPlanTemplate(templateId);
            });
        }
    }
}