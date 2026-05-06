using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class LandSplit
    {
        public int Id { get; set; }
        public double Area { get; set; }

        public Geometry Geom { get; set; }

        public Guid LandId { get; set; }

        public int Indexes { get; set; }

        public long Status { get; set; }

        public Guid Wid { get; set; }
        public bool Locked { get; set; }

        public virtual Land Land { get; set; }
        
    }
}
