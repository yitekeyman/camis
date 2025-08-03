using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class AgroEchology
    {
        public int Id { get; set; }
        public Guid LandId { get; set; }
        public int Type { get; set; }
        public string Result { get; set; }

        public Land Land { get; set; }
        public AgroType TypeNavigation { get; set; }
    }
}
