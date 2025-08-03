using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityProgressVariableType
    {
        public ActivityProgressVariableType()
        {
            ActivityProgressVariable = new HashSet<ActivityProgressVariable>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ActivityProgressVariable> ActivityProgressVariable { get; set; }
    }
}
