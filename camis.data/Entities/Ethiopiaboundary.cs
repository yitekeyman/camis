using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Ethiopiaboundary
    {
        public int Gid { get; set; }
        public decimal? Area { get; set; }
        public decimal? Perimeter { get; set; }
        public int? Etbound01c { get; set; }
        public int? Etbound01 { get; set; }
        public Geometry Geom { get; set; }
    }
}
