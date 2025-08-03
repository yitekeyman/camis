using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityPlanTemplate
    {
        public ActivityPlanTemplate()
        {
            Activity = new HashSet<Activity>();
        }

        public Guid Id { get; set; }
        public string Data { get; set; }
        public string Name { get; set; }

        public ICollection<Activity> Activity { get; set; }
    }
}
