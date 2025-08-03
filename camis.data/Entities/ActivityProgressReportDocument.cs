using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityProgressReportDocument
    {
        public int Order { get; set; }
        public Guid? ReportId { get; set; }
        public Guid? DocumentId { get; set; }
        public Guid Id { get; set; }

        public Document Document { get; set; }
        public ActivityProgressReport Report { get; set; }
    }
}
