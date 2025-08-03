using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Ethrivers
    {
        public int Gid { get; set; }
        public int? Fnode { get; set; }
        public int? Tnode { get; set; }
        public int? Lpoly { get; set; }
        public int? Rpoly { get; set; }
        public decimal? Length { get; set; }
        public int? Etriver01c { get; set; }
        public int? Etriver01 { get; set; }
        public string RivName { get; set; }
        public string Type { get; set; }
        public short? Class { get; set; }
        public Geometry Geom { get; set; }
    }
}
