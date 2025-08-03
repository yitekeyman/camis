using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class Address
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int UnitId { get; set; }

        public AddressUnit Unit { get; set; }
    }
}
