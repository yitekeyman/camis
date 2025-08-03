using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class PromotionPicture
    {
        public Guid PromotionUnitId { get; set; }
        public string Picture { get; set; }
        public int Order { get; set; }

        public PromotionUnit PromotionUnit { get; set; }
    }
}
