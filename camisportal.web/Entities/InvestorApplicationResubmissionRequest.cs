using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class InvestorApplicationResubmissionRequest
    {
        public Guid PromotionUnitId { get; set; }
        public Guid InvestorId { get; set; }
        public long? RequestTime { get; set; }
        public long? WaitUntil { get; set; }
        public int Status { get; set; }

        public Investor Investor { get; set; }
        public PromotionUnit PromotionUnit { get; set; }
    }
}
