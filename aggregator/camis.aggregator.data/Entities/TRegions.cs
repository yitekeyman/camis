using System;
using System.Collections.Generic;

namespace camis.aggregator.data.Entities
{
    public partial class TRegions
    {
        public int Id { get; set; }
        public string Csaregionid { get; set; }
        public string Csaregionnameeng { get; set; }
        public string Csaregionnameamharic { get; set; }
        public string Csaregionnametigrinya { get; set; }
        public string Csaregionnameoromifya { get; set; }
        public string Geometry { get; set; }
        public string Regioncodeam { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
