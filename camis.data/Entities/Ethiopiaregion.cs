using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Ethiopiaregion
    {
        public int Gid { get; set; }
        public decimal? Area { get; set; }
        public decimal? Perimeter { get; set; }
        public int? Etregio01c { get; set; }
        public int? Etregio01 { get; set; }
        public string Region { get; set; }
        public Geometry Geom { get; set; }
    }
}
