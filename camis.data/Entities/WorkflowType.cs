using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class WorkflowType
    {
        public WorkflowType()
        {
            Workflow = new HashSet<Workflow>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Workflow> Workflow { get; set; }
    }
}
