using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class LandClimate
    {
        public Guid LandId { get; set; }
        public int Month { get; set; }
        public float? Precipitation { get; set; }
        public float? TempLow { get; set; }
        public float? TempHigh { get; set; }
        public float? TempAvg { get; set; }

        public Land Land { get; set; }
    }
}
