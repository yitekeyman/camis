using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class LandMoisture
    {
        public int Id { get; set; }
        public Guid LandId { get; set; }
        public int Moisture { get; set; }

        public Land Land { get; set; }
        public MoistureSource MoistureNavigation { get; set; }
    }
}
