using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class Promotion
    {
        public Promotion()
        {
            PromotionStatusChange = new HashSet<PromotionStatusChange>();
            PromotionUnit = new HashSet<PromotionUnit>();
        }

        public Guid Id { get; set; }
        public long ApplyDateFrom { get; set; }
        public long ApplyDateTo { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public long? PostedOn { get; set; }
        public string EvaluationCriterion { get; set; } 
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Status { get; set; }
        public string PromotionRef { get; set; }
        public string PhysicalAddress { get; set; }

        public Regions RegionNavigation { get; set; }
        public ICollection<PromotionStatusChange> PromotionStatusChange { get; set; }
        public ICollection<PromotionUnit> PromotionUnit { get; set; }
    }
}
