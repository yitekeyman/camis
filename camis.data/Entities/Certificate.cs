using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Certificate
    {
        public Guid LandId { get; set; }
        public string Label { get; set; }
        public Guid? WId { get; set; }
        public Geometry Geom { get; set; }
    }
}
