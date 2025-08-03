using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Role
    {
        public Role()
        {
            UserRole = new HashSet<UserRole>();
            WorkItem = new HashSet<WorkItem>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserRole> UserRole { get; set; }
        public ICollection<WorkItem> WorkItem { get; set; }
    }
}
