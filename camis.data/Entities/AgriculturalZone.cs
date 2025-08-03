using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class AgriculturalZone
    {
        public int Id { get; set; }
        public Guid LandId { get; set; }
        public string IsAgriZone { get; set; }

        public Land Land { get; set; }
    }
}
