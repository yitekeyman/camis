using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivitySchedule
    {
        public long From { get; set; }
        public long To { get; set; }
        public Guid ActivityId { get; set; }
        public Guid? PlanId { get; set; }
        public Guid Id { get; set; }

        public Activity Activity { get; set; }
        public ActivityPlan Plan { get; set; }
    }
}
