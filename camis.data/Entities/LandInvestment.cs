using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class LandInvestment
    {
        public int Id { get; set; }
        public Guid LandId { get; set; }
        public int Investment { get; set; }

        public Land Land { get; set; }
    }
}
