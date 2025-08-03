using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class LandDoc
    {
        public Guid LandId { get; set; }
        public Guid DocId { get; set; }
        public Guid Id { get; set; }

        public Document Doc { get; set; }
        public Land Land { get; set; }
    }
}
