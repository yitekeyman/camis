using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Address
    {
        public Address()
        {
            FarmOperator = new HashSet<FarmOperator>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public int UnitId { get; set; }

        public AddressUnit Unit { get; set; }
        public ICollection<FarmOperator> FarmOperator { get; set; }
    }
}
