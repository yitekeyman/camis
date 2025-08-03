using System;
using System.Collections.Generic;

namespace camis.aggregator.data.Entities
{
    public partial class AuditLog
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string TableName { get; set; }
        public long? UserAction { get; set; }
        public long TimeStamp { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}
