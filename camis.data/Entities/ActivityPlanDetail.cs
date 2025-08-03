using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityPlanDetail
    {
        public Guid? PlanId { get; set; }
        public Guid ActivityId { get; set; }
        public double Target { get; set; }
        public int VariableId { get; set; }
        public double? Weight { get; set; }
        public Guid Id { get; set; }
        public string CustomVariableName { get; set; }
        public string Tag { get; set; }

        public Activity Activity { get; set; }
        public ActivityPlan Plan { get; set; }
        public ActivityProgressVariable Variable { get; set; }
    }
}
