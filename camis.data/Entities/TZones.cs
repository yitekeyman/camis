using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class TZones
    {
        public int Id { get; set; }
        public string Csaregionid { get; set; }
        public string Csazoneid { get; set; }
        public string Csazonenameeng { get; set; }
        public string Csazonenameamharic { get; set; }
        public string Csazonenametigrinya { get; set; }
        public string Csazonenameoromifya { get; set; }
        public string NrlaisZoneid { get; set; }
        public string Geometry { get; set; }
    }
}
