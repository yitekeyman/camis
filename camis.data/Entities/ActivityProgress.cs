using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityProgress
    {
        public Guid? ReportId { get; set; }
        public Guid? ActivityId { get; set; }
        public long Time { get; set; }
        public int? VariableId { get; set; }
        public double Progress { get; set; }
        public Guid Id { get; set; }

        public Activity Activity { get; set; }
        public ActivityProgressReport Report { get; set; }
        public ActivityProgressVariable Variable { get; set; }
    }
}
