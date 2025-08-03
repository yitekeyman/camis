using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Irrigation
    {
        public Irrigation()
        {
            GroundData = new HashSet<GroundData>();
            SurfaceWater = new HashSet<SurfaceWater>();
            WaterSrcParam = new HashSet<WaterSrcParam>();
        }

        public Guid LandId { get; set; }
        public int? GrndWater { get; set; }
        public int Id { get; set; }

        public GroundWater GrndWaterNavigation { get; set; }
        public Land Land { get; set; }
        public ICollection<GroundData> GroundData { get; set; }
        public ICollection<SurfaceWater> SurfaceWater { get; set; }
        public ICollection<WaterSrcParam> WaterSrcParam { get; set; }
    }
}
