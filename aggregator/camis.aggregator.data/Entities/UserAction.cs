using System;
using System.Collections.Generic;

namespace camis.aggregator.data.Entities
{
    public partial class UserAction
    {
        public UserAction()
        {
            UserRole = new HashSet<UserRole>();
        }

        public long Id { get; set; }
        public long? Timestamp { get; set; }
        public string Username { get; set; }
        public int ActionTypeId { get; set; }
        public string Remark { get; set; }

        public User UsernameNavigation { get; set; }
        public ICollection<UserRole> UserRole { get; set; }
    }
}
