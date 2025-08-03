using System;

namespace intapscamis.camis.domain.Workflows.Models
{
    public class WorkflowRequest
    {        
        public Guid? Id { get; set; }
        public int CurrentState { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; } // not editable
    }


    public class WorkItemRequest
    {
        public Guid? Id { get; set; }
        public string WorkflowId { get; set; } // encapsulated in workflow definitions
        public int FromState { get; set; } // encapsulated in workflow definitions
        public int ToState { get; set; } // encapsulated in workflow definitions
        public int Trigger { get; set; } // encapsulated in workflow definitions
        public string DataType { get; set; } // encapsulated in workflow definitions
        public string Data { get; set; }
        public string Description { get; set; }
        public long? AssignedRole { get; set; } // encapsulated in workflow definitions
        public long? AssignedUser { get; set; }
    }
}