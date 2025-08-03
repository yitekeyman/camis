using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityProgressReport
    {
        public ActivityProgressReport()
        {
            ActivityProgress = new HashSet<ActivityProgress>();
            ActivityProgressReportDocument = new HashSet<ActivityProgressReportDocument>();
            ActivityProgressStatus = new HashSet<ActivityProgressStatus>();
        }

        public Guid Id { get; set; }
        public string Note { get; set; }
        public long ReportTime { get; set; }
        public int StatusId { get; set; }
        public Guid RootActivityId { get; set; }

        public Activity RootActivity { get; set; }
        public ActivityStatusType Status { get; set; }
        public ICollection<ActivityProgress> ActivityProgress { get; set; }
        public ICollection<ActivityProgressReportDocument> ActivityProgressReportDocument { get; set; }
        public ICollection<ActivityProgressStatus> ActivityProgressStatus { get; set; }
    }
}
