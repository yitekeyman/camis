using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace camis.aggregator.data.Entities
{
    public partial class UserRole
    {
        [Key, Column(Order = 0)]
        public long UserId { get; set; }
        [Key, Column(Order = 1)]
        public long RoleId { get; set; }
        public long? Aid { get; set; }

        public UserAction A { get; set; }
        public Role Role { get; set; }
        public User User { get; set; }
    }
}
