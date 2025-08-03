using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class PromotionStatusChange
    {
        public Guid Id { get; set; }
        public long ChangeTime { get; set; }
        public string Data { get; set; }
        public int NewStatus { get; set; }
        public int OldStatus { get; set; }
        public Guid PromotionId { get; set; }

        public Promotion Promotion { get; set; }
    }
}
