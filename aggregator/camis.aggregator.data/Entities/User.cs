using System;
using System.Collections.Generic;

namespace camis.aggregator.data.Entities
{
    public partial class User
    {
        public User()
        {
            UserAction = new HashSet<UserAction>();
            UserRole = new HashSet<UserRole>();
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int Status { get; set; }
        public long RegOn { get; set; }
        public string CamisUsername { get; set; }
        public string CamisPassword { get; set; }

        public ICollection<UserAction> UserAction { get; set; }
        public ICollection<UserRole> UserRole { get; set; }
    }
}
