using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Accessibility
    {
        public int Id { get; set; }
        public Guid? LandId { get; set; }
        public int? Accessibility1 { get; set; }

        public AccessibiltyType Accessibility1Navigation { get; set; }
        public Land Land { get; set; }
    }
}
