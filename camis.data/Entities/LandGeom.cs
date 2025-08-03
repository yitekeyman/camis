using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class LandGeom
    {
        public Guid? LandId { get; set; }
        public string Upid { get; set; }
        public Geometry Geom { get; set; }
        public int? Column4 { get; set; }

        public Land Land { get; set; }
    }
}
