using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class WaterSourceType
    {
        public WaterSourceType()
        {
            WaterSrcParam = new HashSet<WaterSrcParam>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<WaterSrcParam> WaterSrcParam { get; set; }
    }
}
