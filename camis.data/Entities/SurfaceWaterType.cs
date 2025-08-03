using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class SurfaceWaterType
    {
        public SurfaceWaterType()
        {
            SurfaceWater = new HashSet<SurfaceWater>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<SurfaceWater> SurfaceWater { get; set; }
    }
}
