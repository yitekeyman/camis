using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class UserRole
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public long? Aid { get; set; }

        public UserAction A { get; set; }
        public Role Role { get; set; }
        public User User { get; set; }
    }
}
