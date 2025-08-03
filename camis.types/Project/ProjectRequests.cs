using System;
using System.Collections.Generic;
using intapscamis.camis.domain.Documents.Models;

namespace intapscamis.camis.domain.Projects.Models
{
    public class ActivityPlanRequest
    {
        public string Note { get; set; }
        public int StatusId { get; set; }
        public ActivityRequest RootActivity { get; set; }

        public ICollection<DocumentRequest> Documents { get; set; }

        // only for progress reporting...
        public string RootActivityId { get; set; }
        public int ReportStatusId { get; set; }
        public string ReportNote { get; set; }
        public long ReportDate { get; set; }
        public ICollection<DocumentRequest> ReportDocuments { get; set; }
        public bool IsAdditional { get; set; } = false;
        
        // only for editing
        public Guid? Id { get; set; }
    }

    public class ActivityRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public string ParentActivityId { get; set; } // overridden by null for root activity
        public string Tag { get; set; }

        public ICollection<ActivityScheduleRequest> Schedules { get; set; }
        public ICollection<ActivityPlanDetailRequest> ActivityPlanDetails { get; set; }

        public ICollection<ActivityRequest> Children { get; set; }
        
        // only for progress reporting...
        public Guid? Id { get; set; } // also for editing
        public int? ProgressStatusId { get; set; }
        
        // only for generating all projects' summary report
        public Guid? TemplateId { get; set; }
    }

    public class ActivityScheduleRequest
    {
        public long From { get; set; }
        public long To { get; set; }
        
        // only for editing
        public Guid? Id { get; set; }
    }

    public class ActivityPlanDetailRequest
    {
        public double Target { get; set; }
        public double Weight { get; set; }
        public int VariableId { get; set; }
        public string CustomVariableName { get; set; }
        public string Tag { get; set; }

        // only for progress reporting...
        public double? Progress { get; set; }
        
        // only for editing
        public Guid? Id { get; set; }
    }


    public class ActivityPlanTemplateRequest
    {
        public string Name { get; set; }
        public ActivityRequest Data { get; set; }
    }
}