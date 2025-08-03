using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityProgressStatus
    {
        public Guid? ActivityId { get; set; }
        public Guid? ReportId { get; set; }
        public long Time { get; set; }
        public int StatusId { get; set; }
        public Guid Id { get; set; }

        public Activity Activity { get; set; }
        public ActivityProgressReport Report { get; set; }
        public ActivityStatusType Status { get; set; }
    }
}
