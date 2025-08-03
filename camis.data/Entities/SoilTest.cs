using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class SoilTest
    {
        public int Id { get; set; }
        public int? TestType { get; set; }
        public Guid? LandId { get; set; }
        public string Result { get; set; }

        public Land Land { get; set; }
        public SoilTestTypes TestTypeNavigation { get; set; }
    }
}
