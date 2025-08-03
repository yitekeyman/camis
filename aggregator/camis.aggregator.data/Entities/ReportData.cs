using System;
using System.Collections.Generic;

namespace camis.aggregator.data.Entities
{
    public partial class ReportData
    {
        public int? SummerizedBy { get; set; }
        public int? FilteredBy { get; set; }
        public string Region { get; set; }
        public string Zone { get; set; }
        public string Woreda { get; set; }
        public string ReportResponse { get; set; }
        public int ReportType { get; set; }
        public long Timestamp { get; set; }
        public string ReportRequest { get; set; }
        public int Id { get; set; }
    }
}
