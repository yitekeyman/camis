using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class InvestorProject
    {
        public Guid InvestorId { get; set; }
        public string ProjectProfile { get; set; }
        public Guid ProjectId { get; set; }
        public long SyncNumber { get; set; }
        public int? Status { get; set; }
    }
}
