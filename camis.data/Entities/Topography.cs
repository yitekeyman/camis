using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Topography
    {
        public Guid LandId { get; set; }
        public int Type { get; set; }
        public string Result { get; set; }
        public Guid Id { get; set; }

        public Land Land { get; set; }
        public TopographyType TypeNavigation { get; set; }
    }
}
