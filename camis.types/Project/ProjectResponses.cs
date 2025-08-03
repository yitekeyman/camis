using System;
using System.Collections.Generic;
using intapscamis.camis.domain.Documents.Models;

namespace intapscamis.camis.domain.Projects.Models
{
    public class ActivityStatusTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class ActivityProgressMeasuringUnitResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ConvertFrom { get; set; }
        public double? ConversionFactor { get; set; }
        public double? ConversionOffset { get; set; }
    }
    
    public class ActivityProgressVariableResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? DefaultUnitId { get; set; }
        public int? TypeId { get; set; }

        public ActivityProgressMeasuringUnitResponse DefaultUnit { get; set; }
        public ActivityProgressVariableTypeResponse Type { get; set; }
    }

    public class ActivityProgressVariableTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class ActivityVariableValueListResponse
    {
        public int VariableId { get; set; }
        public int Order { get; set; }
        public double Value { get; set; }
        public string Name { get; set; }
    }
    
    
    public class ActivityPlanResponse
    {
        public Guid? Id { get; set; }
        public string Note { get; set; }
        public int? StatusId { get; set; }
        public Guid? RootActivityId { get; set; }
        
        public ActivityStatusTypeResponse Status { get; set; }
        public ActivityResponse RootActivity { get; set; }
        public ICollection<DocumentResponse> Documents { get; set; }

        public double CalculatedProgress { get; set; }
    }
    
    public class ActivityResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public Guid? ParentActivityId { get; set; }
        public string Tag { get; set; }

        public ICollection<ActivityScheduleResponse> Schedules { get; set; }
        public ICollection<ActivityPlanDetailResponse> ActivityPlanDetails { get; set; }

        public ICollection<ActivityResponse> Children { get; set; }
        
        // only for generating all projects' summary report
        public Guid? TemplateId { get; set; }
    }

    public class ActivityScheduleResponse
    {
        public Guid Id { get; set; }
        public long From { get; set; }
        public long To { get; set; }
    }

    public class ActivityPlanDetailResponse
    {
        public Guid Id { get; set; }
        public double Target { get; set; }
        public double? Weight { get; set; }
        public int VariableId { get; set; }
        public string CustomVariableName { get; set; }
        public string Tag { get; set; }


        public ActivityProgressVariableResponse Variable { get; set; }
    }


    public class ActivityProgressReportResponse
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public long ReportTime { get; set; }
        public int StatusId { get; set; }
        public Guid RootActivityId { get; set; }

        public ActivityStatusTypeResponse ReportStatus { get; set; }
        public ActivityResponse RootActivity { get; set; }
        public ICollection<DocumentResponse> ReportDocuments { get; set; }
        public ICollection<ActivityProgressStatusResponse> ActivityStatuses { get; set; }
        public ICollection<ActivityProgressResponse> VariableProgresses { get; set; }
        
        public double CalculatedProgress { get; set; }
    }

    public class ActivityProgressStatusResponse
    {
        public Guid Id { get; set; }
        public Guid? ActivityId { get; set; }
        public long Time { get; set; }
        public int StatusId { get; set; }

        public ActivityResponse Activity { get; set; }
        public ActivityStatusTypeResponse Status { get; set; }
    }

    public class ActivityProgressResponse
    {
        public Guid Id { get; set; }
        public Guid? ActivityId { get; set; }
        public long Time { get; set; }
        public int? VariableId { get; set; }
        public double Progress { get; set; }

        public ActivityResponse Activity { get; set; }
        public ActivityProgressVariableResponse Variable { get; set; }
    }


    public class CalculatedVariableProgressResponse
    {
        public int VariableId { get; set; }
        public double TotalProgress { get; set; }
        public double TotalTarget { get; set; }
        
        public ActivityProgressVariableResponse Variable { get; set; }
    }
    
    
    public class ActivityPlanTemplateResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ActivityRequest Data { get; set; }
    }
}