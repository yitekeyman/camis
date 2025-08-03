using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class PercipitationPurgatory
    {
        public int Id { get; set; }
        public double? Amount { get; set; }
        public Geometry Geom { get; set; }
    }
}
