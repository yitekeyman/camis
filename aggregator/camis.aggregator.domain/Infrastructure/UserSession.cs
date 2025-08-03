using System;
using System.Collections.Generic;

namespace camis.aggregator.domain.Infrastructure
{
    public class UserSession
    {
        public String id;
        public Dictionary<string, object> Content;
        public string Username { get; set; }
        public long Role { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastSeen { get; set; }
    }
}