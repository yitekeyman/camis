using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class PromotionDoc
    {
        public Guid PromotionUnitId { get; set; }
        public string DocData { get; set; }
        public int Order { get; set; }

        public PromotionUnit PromotionUnit { get; set; }
    }
}
