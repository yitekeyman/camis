using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class SurfaceWater
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Result { get; set; }
        public int Irrigation { get; set; }

        public Irrigation IrrigationNavigation { get; set; }
        public SurfaceWaterType TypeNavigation { get; set; }
    }
}
