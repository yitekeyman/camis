using System;
using System.Collections.Generic;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Documents;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Projects.Models;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace intapscamis.camis.domain.Projects
{
    public interface IProjectService : ICamisService
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

        ActivityPlan CreateActivityPlan(ActivityPlanRequest data);

        ActivityPlan UpdateActivityPlan(ActivityPlanRequest data);

        ActivityProgressReport CreateActivityProgressReport(ActivityPlanRequest body, Guid rootActivityId);

        WorkItemResponse GetLastWorkItem(Guid workflowId);
        Document InWorkItemReportFile(Guid workItemId, Guid documentId);

        ActivityPlanTemplateResponse CreateActivityPlanTemplate(ActivityPlanTemplateRequest body);
        IList<ActivityPlanTemplateResponse> GetAllActivityPlanTemplates();
        ActivityPlanTemplateResponse UpdateActivityPlanTemplate(Guid templateId, ActivityPlanTemplateRequest body);
        ActivityPlanTemplateResponse DeleteActivityPlanTemplate(Guid templateId);
    }

    public class ProjectService : CamisService, IProjectService
    {
        private readonly IDocumentService _documentService;
        private readonly IWorkflowService _workflowService;

        private UserSession _session;

        public ProjectService(IDocumentService documentService, IWorkflowService workflowService)
        {
            _documentService = documentService;
            _workflowService = workflowService;
        }

        public override void SetContext(CamisContext context)
        {
            base.SetContext(context);
            _documentService.SetContext(Context);
            _workflowService.SetContext(Context);
        }

        public void SetSession(UserSession session)
        {
            _session = session;
            _documentService.SetSession(session);
            _workflowService.SetSession(session);
        }


        public IList<ActivityStatusTypeResponse> GetActivityStatusTypes()
        {
            return Context.ActivityStatusType.OrderBy(item => item.Id).Select(item => new ActivityStatusTypeResponse
            {
                Id = item.Id,
                Name = item.Name
            }).ToList();
        }

        public IList<ActivityProgressMeasuringUnitResponse> GetActivityProgressMeasuringUnits()
        {
            return Context.ActivityProgressMeasuringUnit.OrderBy(item => item.Id).Select(item =>
                new ActivityProgressMeasuringUnitResponse
                {
                    Id = item.Id,
                    Name = item.Name
                }).ToList();
        }

        public IList<ActivityProgressVariableTypeResponse> GetActivityProgressVariableTypes()
        {
            return Context.ActivityProgressVariableType.OrderBy(item => item.Id).Select(item =>
                new ActivityProgressVariableTypeResponse
                {
                    Id = item.Id,
                    Name = item.Name
                }).ToList();
        }

        public IList<ActivityVariableValueListResponse> GetActivityVariableValueLists()
        {
            return Context.ActivityVariableValueList.OrderBy(item => item.Order).Select(item =>
                new ActivityVariableValueListResponse
                {
                    VariableId = item.VariableId,
                    Order = item.Order,
                    Value = item.Value,
                    Name = item.Name
                }).ToList();
        }

        public IList<string> GetActivityTags()
        {
            var tags = new List<string>();

            Context.Activity.ToList().ForEach(a =>
            {
                if (!string.IsNullOrEmpty(a.Tag) && tags.Contains(a.Tag) != true) tags.Add(a.Tag);
            });

            return tags;
        }

        public IList<string> GetActivityPlanDetailTags()
        {
            var tags = new List<string>();

            Context.ActivityPlanDetail.ToList().ForEach(a =>
            {
                if (!string.IsNullOrEmpty(a.Tag) && tags.Contains(a.Tag) != true) tags.Add(a.Tag);
            });

            return tags;
        }

        public IList<ActivityProgressVariableResponse> GetActivityProgressVariables()
        {
            return Context.ActivityProgressVariable.Select(item => new ActivityProgressVariableResponse
            {
                Id = item.Id,
                Name = item.Name,
                DefaultUnitId = item.DefaultUnitId,
                TypeId = item.TypeId,

                DefaultUnit = new ActivityProgressMeasuringUnitResponse
                {
                    Id = item.DefaultUnit.Id,
                    Name = item.DefaultUnit.Name,
                    ConvertFrom = item.DefaultUnit.ConvertFrom,
                    ConversionFactor = item.DefaultUnit.ConversionFactor,
                    ConversionOffset = item.DefaultUnit.ConversionOffset
                },
                Type = new ActivityProgressVariableTypeResponse
                {
                    Id = item.Type.Id,
                    Name = item.Type.Name
                }
            }).ToList();
        }


        public ActivityPlanResponse GetActivityPlan(Guid id)
        {
            var plan = Context.ActivityPlan.Find(id);
            return ParseActivityPlanResponse(plan);
        }

        public ActivityPlanResponse GetPlanFromRootActivity(Guid id)
        {
            var plan = Context.ActivityPlan.First(ap => ap.RootActivityId == id);
            return ParseActivityPlanResponse(plan);
        }

        public ActivityResponse GetActivity(Guid id)
        {
            var activity = Context.Activity.Find(id);
            return ParseActivityResponse(activity);
        }

        public ActivityProgressReportResponse GetProgressReport(Guid id)
        {
            var report = Context.ActivityProgressReport.Find(id);
            return ParseActivityProgressReportResponse(report);
        }


        public PaginatorResponse<ActivityProgressReportResponse> SearchReports(Guid planId, string term, int skip,
            int take)
        {
            var plan = Context.ActivityPlan.Find(planId);

            var searchQuery = Context.ActivityProgressReport.Where(r =>
                r.RootActivityId == plan.RootActivityId && string.Join("\n",
                    r.Note,
                    r.ReportTime.ToString(), // todo: convert to date string
                    r.RootActivity.Name,
                    r.RootActivity.Description,
                    r.Status.Name
                ).ToLower().Contains(term.ToLower())
            ).OrderByDescending(r => r.ReportTime);
            var farms = searchQuery.Skip(skip).Take(take).AsEnumerable();
            return new PaginatorResponse<ActivityProgressReportResponse>
            {
                TotalSize = searchQuery.Count(),
                Items = farms.Select(ParseActivityProgressReportResponse).ToList()
            };
        }


        public double CalculateProgress(Guid activityId, long? reportTime)
        {
            var activity = GetActivity(activityId);

            var reports = Context.ActivityProgressReport.Where(r => r.RootActivityId == activity.Id);
            if (!reports.Any()) return 0;

            var lastReportTime = reportTime ?? reports.Max(r => r.ReportTime);
            var lastReport = reports.First(r => r.ReportTime == lastReportTime);

            if (lastReport.StatusId == (int) ActivityStatusTypes.Completed) return 1;
            return GetWeighedActivityProgress(activity, reportTime) / activity.Weight;
        }

        private double GetWeighedActivityProgress(ActivityResponse activity, long? reportTime)
        {
            var statuses = Context.ActivityProgressStatus.Where(s => s.ActivityId == activity.Id);
            if (statuses.Any())
            {
                var lastStatusTime = reportTime ?? statuses.Max(s => s.Time);
                var lastStatus = statuses.FirstOrDefault(s => s.Time == lastStatusTime);

                if (lastStatus != null)
                {
                    Console.WriteLine($"activity ({activity.Name}): +{activity.Weight} (complete)\n");
                    if (lastStatus.StatusId == (int) ActivityStatusTypes.Completed)
                        return activity.Weight;
                }
            }

            if (activity.ActivityPlanDetails.Count > 0)
            {
                double totalProgress = 0;
                double totalWeight = 0;
                foreach (var detail in activity.ActivityPlanDetails)
                {
                    if (detail.Weight == null) detail.Weight = 0;
                    if (detail.Variable.TypeId != (int) ActivityProgressVariableTypes.Measuring) continue;

                    var progresses = Context.ActivityProgress.Where(p =>
                        p.ActivityId == activity.Id && p.VariableId == detail.VariableId);
                    if (!progresses.Any()) continue;
                    var lastProgressTime = reportTime ?? progresses.Max(p => p.Time);
                    var lastProgress = progresses.FirstOrDefault(p => p.Time == lastProgressTime);

                    if (lastProgress == null) continue;

                    Console.WriteLine(
                        $"    variable ({detail.CustomVariableName}): totalProgress += {lastProgress.Progress} / {detail.Target} * {detail.Weight.Value} ...");
                    totalProgress += lastProgress.Progress / detail.Target * detail.Weight.Value;

                    totalWeight += detail.Weight.Value;
                }

                if (totalWeight != 0)
                {
                    Console.WriteLine(
                        $"activity ({activity.Name}): +{activity.Weight * totalProgress / totalWeight} (equals {activity.Weight} * {totalProgress} / {totalWeight})\n");
                    return activity.Weight * totalProgress / totalWeight;
                }
            }

            // nanny
            var childrenActivities = Context.Activity.Where(a => a.ParentActivityId.Equals(activity.Id));
            double totalChildrenProgress = 0;
            double totalChildrenWeight = 0;
            foreach (var childActivity in childrenActivities)
            {
                totalChildrenProgress += GetWeighedActivityProgress(GetActivity(childActivity.Id), reportTime);
                totalChildrenWeight += childActivity.Weight;
            }

            if (totalChildrenWeight != 0) return totalChildrenProgress / totalChildrenWeight;

            return 0;
        }

        public IList<CalculatedVariableProgressResponse> CalculateResourceProgress(Guid activityId, long? reportTime)
        {
            var activity = GetActivity(activityId);
            var resourceVariables = Context.ActivityProgressVariable
                .Where(v => v.TypeId == (int) ActivityProgressVariableTypes.Resource).AsEnumerable();

            var reports = Context.ActivityProgressReport.Where(r => r.RootActivityId == activity.Id);
            if (!reports.Any()) return new List<CalculatedVariableProgressResponse>();
            var lastReportTime = reportTime ?? reports.Max(r => r.ReportTime);
            var lastReport = reports.First(r => r.ReportTime == lastReportTime);

            return (from variable in resourceVariables
                let p = GetAddedVariableProgress(activity, variable.Id, lastReportTime)
                let t = GetAddedVariableTarget(activity, variable.Id)
                where t != null
                select new CalculatedVariableProgressResponse
                {
                    VariableId = variable.Id,
                    TotalProgress = (lastReport.StatusId == (int) ActivityStatusTypes.Completed ? t : p) ?? 0,
                    TotalTarget = t ?? 0,
                    Variable = ParseActivityProgressVariableResponse(variable)
                }).ToList();
        }

        private double? GetAddedVariableProgress(ActivityResponse activity, int variableId, long? reportTime)
        {
            var detail = activity.ActivityPlanDetails.FirstOrDefault(d => d.VariableId == variableId);
            if (detail != null)
            {
                var statuses = Context.ActivityProgressStatus.Where(s => s.ActivityId == activity.Id);
                if (statuses.Any())
                {
                    var lastStatusTime = reportTime ?? statuses.Max(s => s.Time);
                    var lastStatus = statuses.FirstOrDefault(s => s.Time == lastStatusTime);

                    if (lastStatus != null && lastStatus.StatusId == (int) ActivityStatusTypes.Completed)
                        return detail.Target;
                }

                var progresses = Context.ActivityProgress.Where(p =>
                    p.ActivityId == activity.Id && p.VariableId == detail.VariableId);
                if (progresses.Any())
                {
                    var lastProgressTime = reportTime ?? progresses.Max(p => p.Time);
                    var lastProgress = progresses.FirstOrDefault(p => p.Time == lastProgressTime);

                    if (lastProgress != null) return lastProgress.Progress;
                }
            }

            if (activity.Children.Count > 0)
            {
                var sum = activity.Children.Sum(child => GetAddedVariableProgress(child, variableId, reportTime) ?? 0);
                if (sum > 0) return sum;
            }

            return null;
        }

        public IList<CalculatedVariableProgressResponse> CalculateOutcomeProgress(Guid activityId, long? reportTime)
        {
            var activity = GetActivity(activityId);
            var outcomeVariables = Context.ActivityProgressVariable
                .Where(v => v.TypeId == (int) ActivityProgressVariableTypes.Outcome).AsEnumerable();

            var reports = Context.ActivityProgressReport.Where(r => r.RootActivityId == activity.Id);
            if (!reports.Any()) return new List<CalculatedVariableProgressResponse>();
            var lastReportTime = reportTime ?? reports.Max(r => r.ReportTime);
            var lastReport = reports.First(r => r.ReportTime == lastReportTime);

            return (from variable in outcomeVariables
                let p = GetAddedVariableProgress(activity, variable.Id, lastReportTime)
                let t = GetAddedVariableTarget(activity, variable.Id)
                where t != null
                select new CalculatedVariableProgressResponse
                {
                    VariableId = variable.Id,
                    TotalProgress = (lastReport.StatusId == (int) ActivityStatusTypes.Completed ? t : p) ?? 0,
                    TotalTarget = t ?? 0,
                    Variable = ParseActivityProgressVariableResponse(variable)
                }).ToList();
        }

        private double? GetAddedVariableTarget(ActivityResponse activity, int variableId)
        {
            var detail = activity.ActivityPlanDetails.FirstOrDefault(d => d.VariableId == variableId);
            if (detail != null) return detail.Target;

            if (activity.Children.Count > 0)
            {
                var sum = activity.Children.Sum(child => GetAddedVariableTarget(child, variableId) ?? 0);
                if (sum > 0) return sum;
            }

            return null;
        }


        public ActivityPlan CreateActivityPlan(ActivityPlanRequest data)
        {
            var plan = new ActivityPlan
            {
                Id = Guid.NewGuid(),
                Note = data.Note,
                StatusId = data.StatusId
            };

            var rootActivity = plan.RootActivity = new Activity
            {
                Id = Guid.NewGuid(),
                Name = data.RootActivity.Name,
                Description = data.RootActivity.Description,
                Weight = data.RootActivity.Weight,
                ParentActivityId = null,
                Tag = data.RootActivity.Tag?.Replace(" ", ""),
                TemplateId = data.RootActivity.TemplateId
            };

            rootActivity.ActivitySchedule = data.RootActivity.Schedules.Select(scheduleData => new ActivitySchedule
            {
                Id = Guid.NewGuid(),
                ActivityId = rootActivity.Id,
                PlanId = plan.Id,
                From = scheduleData.From,
                To = scheduleData.To
            }).ToList();

            rootActivity.ActivityPlanDetail = data.RootActivity.ActivityPlanDetails.Select(planDetailData =>
                new ActivityPlanDetail
                {
                    Id = Guid.NewGuid(),
                    ActivityId = rootActivity.Id,
                    PlanId = plan.Id,
                    Target = planDetailData.Target,
                    VariableId = planDetailData.VariableId,
                    Weight = planDetailData.Weight,
                    CustomVariableName = planDetailData.CustomVariableName,
                    Tag = planDetailData.Tag?.Replace(" ", "")
                }).ToList();

            Context.ActivityPlan.Add(plan);

            for (var i = 0; i < data.Documents.Count; i++)
            {
                var docReq = data.Documents.ElementAt(i);
                var doc = _documentService.CreateDocument(docReq);
                Context.ActivityPlanDocument.Add(new ActivityPlanDocument
                {
                    Id = Guid.NewGuid(),
                    Order = i,
                    DocumentId = doc.Id,
                    PlanId = plan.Id,
                    Aid = doc.Aid
                });
            }

            Context.SaveChanges(_session.Username, (int) UserActionType.CreateActivityPlan);

            foreach (var childActivityData in data.RootActivity.Children)
                CreateChildActivity(childActivityData, rootActivity.Id, plan.Id);

            return plan;
        }

        private Activity CreateChildActivity(ActivityRequest data, Guid parentActivityId, Guid planId)
        {
            var parentActivity = Context.Activity.First(a => a.Id == parentActivityId);

            var childActivity = new Activity
            {
                Id = Guid.NewGuid(),
                Name = data.Name,
                Description = data.Description,
                Weight = data.Weight,
                ParentActivityId = parentActivity.Id,
                Tag = parentActivity.Tag?.Replace(" ", ""),
                TemplateId = data.TemplateId
            };

            childActivity.ActivitySchedule = data.Schedules.Select(scheduleData => new ActivitySchedule
            {
                Id = Guid.NewGuid(),
                ActivityId = childActivity.Id,
                PlanId = planId,
                From = scheduleData.From,
                To = scheduleData.To
            }).ToList();

            childActivity.ActivityPlanDetail = data.ActivityPlanDetails.Select(planDetailData =>
                new ActivityPlanDetail
                {
                    Id = Guid.NewGuid(),
                    ActivityId = childActivity.Id,
                    PlanId = planId,
                    Target = planDetailData.Target,
                    VariableId = planDetailData.VariableId,
                    Weight = planDetailData.Weight,
                    CustomVariableName = planDetailData.CustomVariableName,
                    Tag = planDetailData.Tag?.Replace(" ", "")
                }).ToList();

            parentActivity.InverseParentActivity.Add(childActivity);
            Context.SaveChanges(_session.Username, (int) UserActionType.CreateChildActivity);

            foreach (var grandChildActivityData in data.Children)
                CreateChildActivity(grandChildActivityData, childActivity.Id, planId);

            return childActivity;
        }


        public ActivityPlan UpdateActivityPlan(ActivityPlanRequest data)
        {
            var plan = Context.ActivityPlan.Find(data.Id);
            plan.Note = data.Note;
            plan.StatusId = data.StatusId;
            data.RootActivity.Id = plan.RootActivityId; // override
            plan.RootActivityId = RecursivelyAddOrUpdateActivity(data.RootActivity, null, plan.Id).Id;
            Context.ActivityPlan.Update(plan);
            Context.SaveChanges();

            // add or update plan documents
            IList<ActivityPlanDocument> validPlanDocuments = data.Documents
                .Select((documentData, i) => AddOrUpdatePlanDocument(documentData, plan.Id, i)).ToList();
            // ...remove old plan documents
            IList<Guid> validPlanDocumentIds = validPlanDocuments.Select(planDocument => planDocument.Id).ToList();
            var oldPlanDocuments = Context.ActivityPlanDocument.Where(planDocument =>
                planDocument.PlanId == plan.Id && !validPlanDocumentIds.Contains(planDocument.Id));
            Context.ActivityPlanDocument.RemoveRange(oldPlanDocuments);
            Context.SaveChanges();
            // ...remove old documents
            IList<Guid> oldDocumentIds = oldPlanDocuments.Select(planDocument => planDocument.DocumentId).ToList();
            var oldDocuments = Context.Document.Where(document => oldDocumentIds.Contains(document.Id));
            Context.Document.RemoveRange(oldDocuments);
            Context.SaveChanges();

            Context.SaveChanges(_session.Username, (int) UserActionType.UpdateActivityPlan);
            return plan;
        }

        private ActivityPlanDocument AddOrUpdatePlanDocument(DocumentRequest documentData, Guid planId, int order)
        {
            var oldPlanDocument = Context.ActivityPlanDocument.FirstOrDefault(planDoc =>
                planDoc.PlanId == planId && documentData.Id != null &&
                planDoc.DocumentId == documentData.Id);
            var isNew = oldPlanDocument == null;

            var document = isNew
                ? _documentService.CreateDocument(documentData)
                : _documentService.UpdateDocument(documentData.Id.Value, documentData);
            Context.SaveChanges();
            var planDocument = isNew
                ? new ActivityPlanDocument()
                : Context.ActivityPlanDocument.First(pd => pd.DocumentId == document.Id);
            planDocument.Id = isNew ? Guid.NewGuid() : planDocument.Id;
            planDocument.Order = order;
            planDocument.DocumentId = document.Id;
            planDocument.PlanId = planId;
            planDocument.Aid = document.Aid;

            if (isNew) Context.ActivityPlanDocument.Add(planDocument);
            else Context.ActivityPlanDocument.Update(planDocument);
            Context.SaveChanges();

            return planDocument;
        }

        private Activity RecursivelyAddOrUpdateActivity(ActivityRequest activityData, Guid? parentActivityId,
            Guid planId)
        {
            var oldActivity = Context.Activity.Find(activityData.Id);
            var isNew = oldActivity == null;

            var activity = isNew ? new Activity() : oldActivity;
            activity.Id = isNew ? Guid.NewGuid() : activity.Id;
            activity.Name = activityData.Name;
            activity.Description = activityData.Description;
            activity.Weight = activityData.Weight;
            activity.ParentActivityId = parentActivityId;
            activity.Tag = activityData.Tag?.Replace(" ", "");
            activity.TemplateId = activityData.TemplateId;
            
            if (isNew) Context.Activity.Add(activity);
            else Context.Activity.Update(activity);
            Context.SaveChanges();
            
            // add or update plan details
            IList<ActivityPlanDetail> validPlanDetails = activityData.ActivityPlanDetails.Select(planDetailData =>
                AddOrUpdateActivityPlanDetail(planDetailData, activity.Id, planId)).ToList();
            // ...remove old plan details
            IList<Guid> validPlanDetailIds = validPlanDetails.Select(planDetail => planDetail.Id).ToList();
            var oldPlanDetails = Context.ActivityPlanDetail.Where(planDetail =>
                planDetail.PlanId == planId && planDetail.ActivityId == activity.Id &&
                !validPlanDetailIds.Contains(planDetail.Id)).ToList();
            Context.ActivityPlanDetail.RemoveRange(oldPlanDetails);
            Context.SaveChanges();
            // ...remove old progresses
            IList<int> validPlanDetailVariableIds =
                validPlanDetails.Select(planDetail => planDetail.VariableId).ToList();
            var oldProgressesForPlanDetail = Context.ActivityProgress.Where(progress =>
                progress.ActivityId == activity.Id && progress.VariableId.HasValue &&
                !validPlanDetailVariableIds.Contains(progress.VariableId.Value)).ToList();
            Context.ActivityProgress.RemoveRange(oldProgressesForPlanDetail);
            Context.SaveChanges();
            
            // add or update schedules
            IList<ActivitySchedule> validSchedules = activityData.Schedules.Select(scheduleData =>
                AddOrUpdateActivitySchedule(scheduleData, activity.Id, planId)).ToList();
            // ...remove old schedules
            IList<Guid> validScheduleIds = validSchedules.Select(schedule => schedule.Id).ToList();
            var oldSchedules = Context.ActivitySchedule.Where(schedule =>
                schedule.PlanId == planId && schedule.ActivityId == activity.Id &&
                !validScheduleIds.Contains(schedule.Id)).ToList();
            Context.ActivitySchedule.RemoveRange(oldSchedules);
            Context.SaveChanges();
            
            // recursively add or update child activities
            IList<Activity> validChildActivities = activityData.Children.Select(childActivityData =>
                RecursivelyAddOrUpdateActivity(childActivityData, activity.Id, planId)).ToList();
            // ...remove old child activities
            IList<Guid> validChildActivityIds = validChildActivities.Select(childActivity => childActivity.Id).ToList();
            var oldChildActivities = Context.Activity.Where(childActivity =>
                childActivity.ParentActivityId == activity.Id && !validChildActivityIds.Contains(childActivity.Id));            
            foreach (var oldChildActivity in oldChildActivities)
                PerformActivityTreeGenocideOn(oldChildActivity, planId);
            
            return activity;
        }

        private void PerformActivityTreeGenocideOn(Activity activity, Guid planId)
        {
            var children = Context.Activity.Where(childActivity => childActivity.ParentActivityId == activity.Id)
                .AsEnumerable();
            foreach (var child in children) PerformActivityTreeGenocideOn(child, planId);
            
            var progresses = Context.ActivityProgress.Where(progress => progress.ActivityId == activity.Id);
            Context.ActivityProgress.RemoveRange(progresses);
            Context.SaveChanges();

            var progressStatuses =
                Context.ActivityProgressStatus.Where(progressStatus => progressStatus.ActivityId == activity.Id);
            Context.ActivityProgressStatus.RemoveRange(progressStatuses);
            Context.SaveChanges();
            
            var planDetails = Context.ActivityPlanDetail.Where(planDetail =>
                planDetail.PlanId == planId && planDetail.ActivityId == activity.Id);
            Context.ActivityPlanDetail.RemoveRange(planDetails);
            Context.SaveChanges();
            
            var schedules = Context.ActivitySchedule.Where(schedule =>
                schedule.PlanId == planId && schedule.ActivityId == activity.Id);
            Context.ActivitySchedule.RemoveRange(schedules);
            Context.SaveChanges();
            
            Context.Activity.Remove(activity);
            Context.SaveChanges();
        }

        private ActivityPlanDetail AddOrUpdateActivityPlanDetail(ActivityPlanDetailRequest planDetailData,
            Guid activityId, Guid planId)
        {
            var oldPlanDetail = Context.ActivityPlanDetail.Find(planDetailData.Id);
            var isNew = oldPlanDetail == null;

            var planDetail = isNew ? new ActivityPlanDetail() : oldPlanDetail;
            planDetail.Id = isNew ? Guid.NewGuid() : planDetail.Id;
            planDetail.ActivityId = activityId;
            planDetail.PlanId = planId;
            planDetail.Target = planDetailData.Target;
            planDetail.VariableId = planDetailData.VariableId;
            planDetail.Weight = planDetailData.Weight;
            planDetail.CustomVariableName = planDetailData.CustomVariableName;
            planDetail.Tag = planDetailData.Tag?.Replace(" ", "");

            if (isNew) Context.ActivityPlanDetail.Add(planDetail);
            else Context.ActivityPlanDetail.Update(planDetail);
            Context.SaveChanges();

            return planDetail;
        }

        private ActivitySchedule AddOrUpdateActivitySchedule(ActivityScheduleRequest scheduleData, Guid activityId,
            Guid planId)
        {
            var oldSchedule = Context.ActivitySchedule.Find(scheduleData.Id);
            var isNew = oldSchedule == null;

            var schedule = isNew ? new ActivitySchedule() : oldSchedule;
            schedule.Id = isNew ? Guid.NewGuid() : schedule.Id;
            schedule.ActivityId = activityId;
            schedule.PlanId = planId;
            schedule.From = scheduleData.From;
            schedule.To = scheduleData.To;

            if (isNew) Context.ActivitySchedule.Add(schedule);
            else Context.ActivitySchedule.Update(schedule);
            Context.SaveChanges();

            return schedule;
        }


        public ActivityProgressReport CreateActivityProgressReport(ActivityPlanRequest body, Guid rootActivityId)
        {
            var report = new ActivityProgressReport
            {
                Id = Guid.NewGuid(),
                Note = body.ReportNote,
                ReportTime = body.ReportDate,
                StatusId = body.ReportStatusId,
                RootActivityId = rootActivityId
            };

            AddProgress(body.RootActivity, report, body.IsAdditional);

            Context.ActivityProgressReport.Add(report);

            for (var i = 0; i < body.ReportDocuments.Count; i++)
            {
                var docReq = body.ReportDocuments.ElementAt(i);
                var doc = _documentService.CreateDocument(docReq);
                Context.ActivityProgressReportDocument.Add(new ActivityProgressReportDocument
                {
                    Id = Guid.NewGuid(),
                    Order = i,
                    ReportId = report.Id,
                    DocumentId = doc.Id
                });
            }

            Context.SaveChanges(_session.Username, (int) UserActionType.CreateActivityProgressReport);

            return report;
        }

        private void AddProgress(ActivityRequest activity, ActivityProgressReport report, bool isAdditional)
        {
            if (activity.ProgressStatusId != null)
                Context.ActivityProgressStatus.Add(new ActivityProgressStatus
                {
                    Id = Guid.NewGuid(),
                    ActivityId = activity.Id,
                    ReportId = report.Id,
                    Time = report.ReportTime,
                    StatusId = activity.ProgressStatusId ??
                               0 // it will never reach the else b/c there's a type check, above
                });

            if (activity.ProgressStatusId != (int) ActivityStatusTypes.Completed)
                foreach (var detail in activity.ActivityPlanDetails)
                    if (detail.Progress != null)
                    {
                        if (isAdditional)
                        {
                            var lastProgress = Context.ActivityProgress.FirstOrDefault(p =>
                                p.ActivityId == activity.Id && p.VariableId == detail.VariableId);
                            if (lastProgress != null) detail.Progress += lastProgress.Progress;
                        }

                        Context.ActivityProgress.Add(new ActivityProgress
                        {
                            Id = Guid.NewGuid(),
                            ReportId = report.Id,
                            ActivityId = activity.Id,
                            Time = report.ReportTime,
                            VariableId = detail.VariableId,
                            Progress = detail.Progress ??
                                       0 // it will never reach the else b/c there's a type check, above
                        });
                    }

            foreach (var child in activity.Children) AddProgress(child, report, isAdditional);
        }


        public WorkItemResponse GetLastWorkItem(Guid workflowId)
        {
            var workItem = _workflowService.GetLastWorkItem(workflowId);

            if (workItem.Data != null)
            {
                var data = JsonConvert.DeserializeObject<ActivityPlanRequest>(
                    JsonConvert.SerializeObject(workItem.Data));

                // (ActivityPlanRequest data).ReportDocuments 
                if (data.ReportDocuments != null)
                    foreach (var doc in data.ReportDocuments)
                        if (doc != null)
                            doc.File = null;

                workItem.Data = data;
            }

            return workItem;
        }

        public Document InWorkItemReportFile(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<ActivityPlanRequest>(dataStr);

            return DocumentService.ParseDocument(data?.ReportDocuments?.First(d => d.Id == documentId));
        }


        public ActivityPlanTemplateResponse CreateActivityPlanTemplate(ActivityPlanTemplateRequest body)
        {
            var template = new ActivityPlanTemplate
            {
                Id = Guid.NewGuid(),
                Name = body.Name,
                Data = JsonConvert.SerializeObject(body.Data)
            };
            template = Context.ActivityPlanTemplate.Add(template).Entity;

            Context.SaveChanges(_session.Username, (int) UserActionType.CreateActivityPlanTemplate);
            return ParseActivityPlanTemplateResponse(template);
        }

        public IList<ActivityPlanTemplateResponse> GetAllActivityPlanTemplates()
        {
            return Context.ActivityPlanTemplate.Select(ParseActivityPlanTemplateResponse).ToList();
        }

        public ActivityPlanTemplateResponse UpdateActivityPlanTemplate(Guid templateId,
            ActivityPlanTemplateRequest body)
        {
            var template = Context.ActivityPlanTemplate.Find(templateId);
            template.Name = body.Name;
            template.Data = JsonConvert.SerializeObject(body.Data);
            template = Context.ActivityPlanTemplate.Update(template).Entity;

            Context.SaveChanges(_session.Username, (int) UserActionType.UpdateActivityPlanTemplate);
            return ParseActivityPlanTemplateResponse(template);
        }

        public ActivityPlanTemplateResponse DeleteActivityPlanTemplate(Guid templateId)
        {
            var template = Context.ActivityPlanTemplate.Find(templateId);
            Context.ActivityPlanTemplate.Remove(template);

            Context.SaveChanges(_session.Username, (int) UserActionType.DeleteActivityPlanTemplate);
            return ParseActivityPlanTemplateResponse(template);
        }


        private ActivityPlanResponse ParseActivityPlanResponse(ActivityPlan plan)
        {
            var rootActivity = Context.Activity.First(a => a.Id == plan.RootActivityId);
            var status = Context.ActivityStatusType.First(s => s.Id == plan.StatusId);
            var documents = Context.ActivityPlanDocument.Where(pd => pd.PlanId == plan.Id).Select(p => p.Document)
                .AsEnumerable();
            var calculatedProgress = CalculateProgress(rootActivity.Id, null);

            return new ActivityPlanResponse
            {
                Id = plan.Id,
                StatusId = plan.StatusId,
                Note = plan.Note,
                RootActivityId = rootActivity.Id,

                Status = ParseActivityStatusTypeResponse(status),
                RootActivity = ParseActivityResponse(rootActivity),
                Documents = documents.Select(DocumentService.ParseDocumentResponse).ToList(),

                CalculatedProgress = calculatedProgress
            };
        }

        private ActivityResponse ParseActivityResponse(Activity activity)
        {
            var schedules = Context.ActivitySchedule.Where(s => s.ActivityId == activity.Id).AsEnumerable();
            var planDetails = Context.ActivityPlanDetail.Where(d => d.ActivityId == activity.Id).AsEnumerable();
            var children = Context.Activity.Where(c => c.ParentActivityId == activity.Id).AsEnumerable();

            return new ActivityResponse
            {
                Id = activity.Id,
                Name = activity.Name,
                Description = activity.Description,
                Weight = activity.Weight,
                Schedules = schedules.Select(ParseActivityScheduleResponse).ToList(),
                ActivityPlanDetails = planDetails.Select(ParseActivityPlanDetailResponse).ToList(),
                ParentActivityId = activity.ParentActivityId,
                Tag = activity.Tag,

                Children = children.Select(ParseActivityResponse).ToList(),
                
                TemplateId = activity.TemplateId
            };
        }

        private ActivityScheduleResponse ParseActivityScheduleResponse(ActivitySchedule schedule)
        {
            return new ActivityScheduleResponse
            {
                Id = schedule.Id,
                From = schedule.From,
                To = schedule.To
            };
        }

        private ActivityPlanDetailResponse ParseActivityPlanDetailResponse(ActivityPlanDetail detail)
        {
            var variable = Context.ActivityProgressVariable.First(v => v.Id == detail.VariableId);

            return new ActivityPlanDetailResponse
            {
                Id = detail.Id,
                Target = detail.Target,
                Weight = detail.Weight,
                VariableId = detail.VariableId,
                CustomVariableName = detail.CustomVariableName,
                Tag = detail.Tag,

                Variable = ParseActivityProgressVariableResponse(variable)
            };
        }

        private ActivityProgressVariableResponse ParseActivityProgressVariableResponse(
            ActivityProgressVariable variable)
        {
            var defaultUnit = Context.ActivityProgressMeasuringUnit.First(u => u.Id == variable.DefaultUnitId);
            var type = Context.ActivityProgressVariableType.First(t => t.Id == variable.TypeId);

            return new ActivityProgressVariableResponse
            {
                Id = variable.Id,
                Name = variable.Name,
                DefaultUnitId = variable.DefaultUnitId,
                TypeId = variable.TypeId,

                DefaultUnit = ParseActivityProgressMeasuringUnitResponse(defaultUnit),
                Type = ParseActivityProgressVariableTypeResponse(type)
            };
        }

        private ActivityProgressMeasuringUnitResponse ParseActivityProgressMeasuringUnitResponse(
            ActivityProgressMeasuringUnit unit)
        {
            return new ActivityProgressMeasuringUnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                ConversionFactor = unit.ConversionFactor,
                ConversionOffset = unit.ConversionOffset,
                ConvertFrom = unit.ConvertFrom // todo: turn this to ConvertFromId and add ConvertFrom in the response
            };
        }

        private ActivityProgressVariableTypeResponse ParseActivityProgressVariableTypeResponse(
            ActivityProgressVariableType type)
        {
            return new ActivityProgressVariableTypeResponse
            {
                Id = type.Id,
                Name = type.Name
            };
        }

        private ActivityStatusTypeResponse ParseActivityStatusTypeResponse(ActivityStatusType type)
        {
            return new ActivityStatusTypeResponse
            {
                Id = type.Id,
                Name = type.Name
            };
        }

        private ActivityProgressReportResponse ParseActivityProgressReportResponse(ActivityProgressReport report)
        {
            var reportStatus = Context.ActivityStatusType.Find(report.StatusId);
            var rootActivity = Context.Activity.Find(report.RootActivityId);
            var documents = Context.ActivityProgressReportDocument.Where(d => d.ReportId == report.Id)
                .Select(d => d.Document).AsEnumerable();
            var progresses = Context.ActivityProgress.Where(p => p.ReportId == report.Id).AsEnumerable();
            var statuses = Context.ActivityProgressStatus.Where(s => s.ReportId == report.Id).AsEnumerable();
            var calculatedProgress = CalculateProgress(rootActivity.Id, report.ReportTime);

            return new ActivityProgressReportResponse
            {
                Id = report.Id,
                Note = report.Note,
                ReportTime = report.ReportTime,
                StatusId = report.StatusId,
                RootActivityId = report.RootActivityId,

                ReportStatus = ParseActivityStatusTypeResponse(reportStatus),
                RootActivity = ParseActivityResponse(rootActivity),
                ReportDocuments = documents.Select(DocumentService.ParseDocumentResponse).ToList(),
                ActivityStatuses = statuses.Select(ParseActivityProgressStatusResponse).ToList(),
                VariableProgresses = progresses.Select(ParseActivityProgressResponse).ToList(),

                CalculatedProgress = calculatedProgress
            };
        }

        private ActivityProgressStatusResponse ParseActivityProgressStatusResponse(
            ActivityProgressStatus progressStatus)
        {
            var activity = Context.Activity.Find(progressStatus.ActivityId);
            var statusType = Context.ActivityStatusType.Find(progressStatus.StatusId);

            return new ActivityProgressStatusResponse
            {
                Id = progressStatus.Id,
                ActivityId = progressStatus.ActivityId,
                Time = progressStatus.Time,
                StatusId = progressStatus.StatusId,

                Activity = ParseActivityResponse(activity),
                Status = ParseActivityStatusTypeResponse(statusType)
            };
        }

        private ActivityProgressResponse ParseActivityProgressResponse(ActivityProgress progress)
        {
            var activity = Context.Activity.Find(progress.ActivityId);
            var variable = Context.ActivityProgressVariable.Find(progress.VariableId);

            return new ActivityProgressResponse
            {
                Id = progress.Id,
                ActivityId = progress.ActivityId,
                Time = progress.Time,
                VariableId = progress.VariableId,
                Progress = progress.Progress,

                Activity = ParseActivityResponse(activity),
                Variable = ParseActivityProgressVariableResponse(variable)
            };
        }

        private ActivityPlanTemplateResponse ParseActivityPlanTemplateResponse(ActivityPlanTemplate template)
        {
            return new ActivityPlanTemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Data = JsonConvert.DeserializeObject<ActivityRequest>(template.Data)
            };
        }
    }
}