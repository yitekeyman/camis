using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class AccessibiltyType
    {
        public AccessibiltyType()
        {
            LandAccessibility = new HashSet<LandAccessibility>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<LandAccessibility> LandAccessibility { get; set; }
    }
}
