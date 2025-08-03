using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Precipitation
    {
        public int Id { get; set; }
        public double? Amount { get; set; }
        public Geometry Geom { get; set; }
        public Guid? LandId { get; set; }
        public int? Month { get; set; }

        public Land Land { get; set; }
        public Months MonthNavigation { get; set; }
    }
}
