using System;
using System.Collections.Generic;

namespace camis.aggregator.data.Entities
{
    public partial class ConnectionParams
    {
        public int Id { get; set; }
        public string RegionCode { get; set; }
        public string Url { get; set; }
    }
}
