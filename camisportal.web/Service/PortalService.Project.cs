using System;
using System.Collections.Generic;
using intaps.camisPortal.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Projects.Models;

namespace intaps.camisPortal.Service
{
    public partial class PortalService
    {
        public IList<ActivityStatusTypeResponse> ActivityStatusTypes(PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).ActivityStatusTypes();
        }
        
        public IList<ActivityProgressMeasuringUnitResponse> ActivityProgressMeasuringUnits(PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).ActivityProgressMeasuringUnits();
        }
        
        public IList<ActivityProgressVariableResponse> ActivityProgressVariables(PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).ActivityProgressVariables();
        }
        
        public IList<ActivityProgressVariableTypeResponse> ActivityProgressVariableTypes(PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).ActivityProgressVariableTypes();
        }
        
        public IList<ActivityVariableValueListResponse> ActivityVariableValueLists(PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).ActivityVariableValueLists();
        }
        
        
        public ActivityPlanResponse ActivityPlan(Guid id, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).ActivityPlan(id);
        }
        
        public ActivityPlanResponse PlanFromRootActivity(Guid id, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).PlanFromRootActivity(id);
        }
        
        public ActivityResponse Activity(Guid id, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).Activity(id);
        }
        
        public ActivityProgressReportResponse ProgressReport(Guid id, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).ProgressReport(id);
        }
        
        
        public PaginatorResponse<ActivityProgressReportResponse> SearchReports(Guid planId, string term, int skip, int take, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).SearchReports(planId, term, skip, take);
        }
        
        
        public double CalculateProgress(Guid planId, long? reportTime, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).CalculateProgress(planId, reportTime);
        }
        
        public IList<CalculatedVariableProgressResponse> CalculateResourceProgress(Guid activityId, long? reportTime, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).CalculateResourceProgress(activityId, reportTime);
        }
        
        public IList<CalculatedVariableProgressResponse> CalculateOutcomeProgress(Guid activityId, long? reportTime, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).CalculateOutcomeProgress(activityId, reportTime);
        }
        
        
        public Guid SubmitNewProgressReport(ActivityPlanRequest body, string description, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).SubmitNewProgressReport(body, description);
        }
        
        
        public Guid RequestNewUpdatePlan(ActivityPlanRequest body, string description, PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).RequestNewUpdatePlan(body, description);
        }
        
        
        public IList<ActivityPlanTemplateResponse> ActivityPlanTemplates(PortalUser user, string regionCode)
        {
            ConnectDB();
            return new CamisInterface(user, regionCode).ActivityPlanTemplates();
        }
    }
}
