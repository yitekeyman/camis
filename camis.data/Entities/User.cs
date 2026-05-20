using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class User
    {
        public User()
        {
            UserAction = new HashSet<UserAction>();
            UserRole = new HashSet<UserRole>();
            WorkItem = new HashSet<WorkItem>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int Status { get; set; }
        public long RegOn { get; set; }
        public string Email { get; set; }
        public virtual ICollection<CHArchive> CHArchiveApprovedByNavigations { get; set; } = new List<CHArchive>();

        public virtual ICollection<CHArchive> CHArchiveRequestedByNavigations { get; set; } = new List<CHArchive>();
        public ICollection<UserAction> UserAction { get; set; }
        public ICollection<UserRole> UserRole { get; set; }
        public ICollection<WorkItem> WorkItem { get; set; }
    }
}
