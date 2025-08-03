using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityPlan
    {
        public ActivityPlan()
        {
            ActivityPlanDetail = new HashSet<ActivityPlanDetail>();
            ActivityPlanDocument = new HashSet<ActivityPlanDocument>();
            ActivitySchedule = new HashSet<ActivitySchedule>();
        }

        public Guid Id { get; set; }
        public string Note { get; set; }
        public Guid? RootActivityId { get; set; }
        public int? StatusId { get; set; }

        public Activity RootActivity { get; set; }
        public ActivityStatusType Status { get; set; }
        public ICollection<ActivityPlanDetail> ActivityPlanDetail { get; set; }
        public ICollection<ActivityPlanDocument> ActivityPlanDocument { get; set; }
        public ICollection<ActivitySchedule> ActivitySchedule { get; set; }
    }
}
