using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class GroundWater
    {
        public GroundWater()
        {
            GroundData = new HashSet<GroundData>();
            Irrigation = new HashSet<Irrigation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<GroundData> GroundData { get; set; }
        public ICollection<Irrigation> Irrigation { get; set; }
    }
}
