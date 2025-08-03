using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class LandSplit
    {
        public int Id { get; set; }
        public Geometry Geom { get; set; }
    }
}
