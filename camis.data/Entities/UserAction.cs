using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class UserAction
    {
        public UserAction()
        {
            Farm = new HashSet<Farm>();
            UserRole = new HashSet<UserRole>();
            WorkItem = new HashSet<WorkItem>();
            Workflow = new HashSet<Workflow>();
        }

        public long Id { get; set; }
        public long? Timestamp { get; set; }
        public string Username { get; set; }
        public int ActionTypeId { get; set; }
        public string Remark { get; set; }

        public User UsernameNavigation { get; set; }
        public ICollection<Farm> Farm { get; set; }
        public ICollection<UserRole> UserRole { get; set; }
        public ICollection<WorkItem> WorkItem { get; set; }
        public ICollection<Workflow> Workflow { get; set; }
    }
}
