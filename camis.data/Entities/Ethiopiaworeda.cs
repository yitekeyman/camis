using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace intapscamis.camis.data.Entities
{
    public partial class Ethiopiaworeda
    {
        public int Gid { get; set; }
        public decimal? Area { get; set; }
        public decimal? Perimeter { get; set; }
        public int? Etwered01c { get; set; }
        public string Wereda { get; set; }
        public string Region { get; set; }
        public string Zones { get; set; }
        public decimal? Popurb { get; set; }
        public decimal? Poprur { get; set; }
        public decimal? Poptot { get; set; }
        public decimal? Areakm2 { get; set; }
        public decimal? Elevation { get; set; }
        public decimal? AreaLak { get; set; }
        public double? Rupopde { get; set; }
        public decimal? CorPdI { get; set; }
        public string PoptoTxt { get; set; }
        public string AreaTxt { get; set; }
        public string PopdeTxt { get; set; }
        public Geometry Geom { get; set; }
    }
}
