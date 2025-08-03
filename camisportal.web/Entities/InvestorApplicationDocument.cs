using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class InvestorApplicationDocument
    {
        public Guid Id { get; set; }
        public Guid PromotionUnitId { get; set; }
        public Guid? InvestorId { get; set; }
        public string Data { get; set; }
        public int Order { get; set; }

        public Investor Investor { get; set; }
        public PromotionUnit PromotionUnit { get; set; }
    }
}
