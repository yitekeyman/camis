using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class InvestorApplication
    {
        public Guid PromotionUnitId { get; set; }
        public Guid InvestorId { get; set; }
        public long ApplyTime { get; set; }
        public double? InvestmentCapital { get; set; }
        public string ProposalAbstract { get; set; }
        public int Status { get; set; }
        public string InvestmentType { get; set; }
        public string ContactAddress { get; set; }
        public string Investment { get; set; }
        public string ActivityPlan { get; set; }
        public bool? IsApproved { get; set; } = false;

        public Investor Investor { get; set; }
        public PromotionUnit PromotionUnit { get; set; }
    }
}
