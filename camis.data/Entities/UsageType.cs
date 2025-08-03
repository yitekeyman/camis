using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class UsageType
    {
        public UsageType()
        {
            LandUsage = new HashSet<LandUsage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<LandUsage> LandUsage { get; set; }
    }
}
