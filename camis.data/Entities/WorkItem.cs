using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class WorkItem
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public int SeqNo { get; set; }
        public int FromState { get; set; }
        public int Trigger { get; set; }
        public string DataType { get; set; }
        public string Data { get; set; }
        public string Description { get; set; }
        public long? AssignedRole { get; set; }
        public long? AssignedUser { get; set; }
        public long? Aid { get; set; }
        public int ToState { get; set; }

        public UserAction A { get; set; }
        public Role AssignedRoleNavigation { get; set; }
        public User AssignedUserNavigation { get; set; }
        public Workflow Workflow { get; set; }
    }
}
