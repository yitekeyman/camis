using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class AddressUnit
    {
        public AddressUnit()
        {
            Address = new HashSet<Address>();
            AddressSchemeUnit = new HashSet<AddressSchemeUnit>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Custom { get; set; }

        public ICollection<Address> Address { get; set; }
        public ICollection<AddressSchemeUnit> AddressSchemeUnit { get; set; }
    }
}
