using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class GroundData
    {
        public int GrndType { get; set; }
        public int Id { get; set; }
        public int Irrigation { get; set; }

        public GroundWater GrndTypeNavigation { get; set; }
        public Irrigation IrrigationNavigation { get; set; }
    }
}
