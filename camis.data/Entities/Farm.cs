using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Farm
    {
        public Farm()
        {
            FarmLand = new HashSet<FarmLand>();
            FarmRegistration = new HashSet<FarmRegistration>();
        }

        public Guid Id { get; set; }
        public Guid OperatorId { get; set; }
        public int TypeId { get; set; }
        public Guid ActivityId { get; set; }
        public double? InvestedCapital { get; set; }
        public string Description { get; set; }
        public long? Aid { get; set; }
        public int[] OtherTypeIds { get; set; }

        public UserAction A { get; set; }
        public Activity Activity { get; set; }
        public FarmOperator Operator { get; set; }
        public FarmType Type { get; set; }
        public ICollection<FarmLand> FarmLand { get; set; }
        public ICollection<FarmRegistration> FarmRegistration { get; set; }
    }
}
