using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityStatusType
    {
        public ActivityStatusType()
        {
            ActivityPlan = new HashSet<ActivityPlan>();
            ActivityProgressReport = new HashSet<ActivityProgressReport>();
            ActivityProgressStatus = new HashSet<ActivityProgressStatus>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ActivityPlan> ActivityPlan { get; set; }
        public ICollection<ActivityProgressReport> ActivityProgressReport { get; set; }
        public ICollection<ActivityProgressStatus> ActivityProgressStatus { get; set; }
    }
}
