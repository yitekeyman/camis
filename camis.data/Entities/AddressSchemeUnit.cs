using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class AddressSchemeUnit
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int SchemeId { get; set; }
        public int UnitId { get; set; }

        public AddressScheme Scheme { get; set; }
        public AddressUnit Unit { get; set; }
    }
}
