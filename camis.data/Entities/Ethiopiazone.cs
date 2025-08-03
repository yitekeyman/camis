using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Ethiopiazone
    {
        public int Gid { get; set; }
        public decimal? Area { get; set; }
        public decimal? Perimeter { get; set; }
        public int? Etzones01c { get; set; }
        public int? Etzones01 { get; set; }
        public string Zones { get; set; }
        public Geometry Geom { get; set; }
    }
}
