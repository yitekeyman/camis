using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Activity
    {
        public Activity()
        {
            ActivityPlan = new HashSet<ActivityPlan>();
            ActivityPlanDetail = new HashSet<ActivityPlanDetail>();
            ActivityProgress = new HashSet<ActivityProgress>();
            ActivityProgressReport = new HashSet<ActivityProgressReport>();
            ActivityProgressStatus = new HashSet<ActivityProgressStatus>();
            ActivitySchedule = new HashSet<ActivitySchedule>();
            Farm = new HashSet<Farm>();
            InverseParentActivity = new HashSet<Activity>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public Guid? ParentActivityId { get; set; }
        public Guid? TemplateId { get; set; }
        public string Tag { get; set; }

        public Activity ParentActivity { get; set; }
        public ActivityPlanTemplate Template { get; set; }
        public ICollection<ActivityPlan> ActivityPlan { get; set; }
        public ICollection<ActivityPlanDetail> ActivityPlanDetail { get; set; }
        public ICollection<ActivityProgress> ActivityProgress { get; set; }
        public ICollection<ActivityProgressReport> ActivityProgressReport { get; set; }
        public ICollection<ActivityProgressStatus> ActivityProgressStatus { get; set; }
        public ICollection<ActivitySchedule> ActivitySchedule { get; set; }
        public ICollection<Farm> Farm { get; set; }
        public ICollection<Activity> InverseParentActivity { get; set; }
    }
}
