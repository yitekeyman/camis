using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class CamisTransferRequest
    {
        public Guid PromotionUnitId { get; set; }
        public Guid InvestorId { get; set; }
        public Guid RequestWfid { get; set; }
        public Guid? FarmId { get; set; }
    }
}
