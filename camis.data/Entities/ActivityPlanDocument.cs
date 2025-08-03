using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityPlanDocument
    {
        public int Order { get; set; }
        public Guid DocumentId { get; set; }
        public Guid? PlanId { get; set; }
        public long? Aid { get; set; }
        public Guid Id { get; set; }

        public Document Document { get; set; }
        public ActivityPlan Plan { get; set; }
    }
}
