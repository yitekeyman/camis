using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Workflow
    {
        public Workflow()
        {
            WorkItem = new HashSet<WorkItem>();
        }

        public Guid Id { get; set; }
        public int CurrentState { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public long? Aid { get; set; }

        public UserAction A { get; set; }
        public WorkflowType Type { get; set; }
        public ICollection<WorkItem> WorkItem { get; set; }
    }
}
