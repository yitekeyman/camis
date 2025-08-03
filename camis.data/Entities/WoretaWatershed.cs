using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class WoretaWatershed
    {
        public int Gid { get; set; }
        public string Type { get; set; }
        public string Ident { get; set; }
        public double? Lat { get; set; }
        public double? Long { get; set; }
        public double? YProj { get; set; }
        public double? XProj { get; set; }
        public string NewSeg { get; set; }
        public string Display { get; set; }
        public string Color { get; set; }
        public double? Altitude { get; set; }
        public double? Depth { get; set; }
        public double? Temp { get; set; }
        public string Time { get; set; }
        public string Model { get; set; }
        public string Filename { get; set; }
        public string Ltime { get; set; }
        public Geometry Geom { get; set; }
    }
}
