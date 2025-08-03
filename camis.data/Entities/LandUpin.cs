using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class LandUpin
    {
        public string Upin { get; set; }
        public Guid? LandId { get; set; }
        public string Profile { get; set; }
        public Geometry Geometry { get; set; }
        public double? Area { get; set; }
        public double? CentroidX { get; set; }
        public double? CentroidY { get; set; }

        public Land Land { get; set; }
    }
}
