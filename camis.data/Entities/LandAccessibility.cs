using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class LandAccessibility
    {
        public int Id { get; set; }
        public Guid? LandId { get; set; }
        public int? Accessibility { get; set; }

        public AccessibiltyType AccessibilityNavigation { get; set; }
        public Land Land { get; set; }
    }
}
