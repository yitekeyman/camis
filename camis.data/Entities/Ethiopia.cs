using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Ethiopia
    {
        public int Gid { get; set; }
        public int? Id { get; set; }
        public string Regionname { get; set; }
        public int? Regionno { get; set; }
        public decimal? Area { get; set; }
        public Geometry Geom { get; set; }
    }
}
