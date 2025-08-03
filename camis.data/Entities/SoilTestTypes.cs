using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class SoilTestTypes
    {
        public SoilTestTypes()
        {
            SoilTest = new HashSet<SoilTest>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<SoilTest> SoilTest { get; set; }
    }
}
