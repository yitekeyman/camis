using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActionType
    {
        public ActionType()
        {
            UserAction = new HashSet<UserAction>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<UserAction> UserAction { get; set; }
    }
}
