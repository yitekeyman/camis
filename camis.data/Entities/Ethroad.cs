using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Ethroad
    {
        public int Gid { get; set; }
        public int? Fnode { get; set; }
        public int? Tnode { get; set; }
        public int? Lpoly { get; set; }
        public int? Rpoly { get; set; }
        public double? Length { get; set; }
        public int? Etroad01co { get; set; }
        public int? Etroad011 { get; set; }
        public string Type { get; set; }
        public double? Class { get; set; }
        public Geometry Geom { get; set; }
    }
}
