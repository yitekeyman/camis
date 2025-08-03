using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Lake
    {
        public int Gid { get; set; }
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public Geometry Geom { get; set; }
    }
}
