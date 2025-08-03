using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class LandUsage
    {
        public int Id { get; set; }
        public Guid LandId { get; set; }
        public int Use { get; set; }

        public Land Land { get; set; }
        public UsageType UseNavigation { get; set; }
    }
}
