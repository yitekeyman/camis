using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class AddressScheme
    {
        public AddressScheme()
        {
            AddressSchemeUnit = new HashSet<AddressSchemeUnit>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<AddressSchemeUnit> AddressSchemeUnit { get; set; }
    }
}
