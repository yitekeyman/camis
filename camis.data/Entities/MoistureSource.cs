using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class MoistureSource
    {
        public MoistureSource()
        {
            LandMoisture = new HashSet<LandMoisture>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<LandMoisture> LandMoisture { get; set; }
    }
}
