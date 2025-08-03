using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class WaterSrcParam
    {
        public int Id { get; set; }
        public int SrcType { get; set; }
        public int Result { get; set; }
        public int Irrigation { get; set; }

        public Irrigation IrrigationNavigation { get; set; }
        public WaterSourceType SrcTypeNavigation { get; set; }
    }
}
