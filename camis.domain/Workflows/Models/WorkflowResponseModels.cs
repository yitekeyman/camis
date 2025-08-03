using System;

namespace intapscamis.camis.domain.Workflows.Models
{
    
    public class WorkflowResponse
    {
        public Guid Id { get; set; }
        public int CurrentState { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public long? Aid { get; set; }

        public WorkflowTypeResponse Type { get; set; }
        public WorkflowActionResponse Action { get; set; }
        public WorkItemResponse WorkItem { get; set; }
    }

    public class WorkflowTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class WorkflowActionResponse
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string TimeStr { get; set; }
    }

    public class WorkItemResponse
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public int SeqNo { get; set; }
        public int FromState { get; set; }
        public int ToState { get; set; }
        public int Trigger { get; set; }
        public string DataType { get; set; }
        public object Data { get; set; }
        public string Description { get; set; }
        public long? AssignedRole { get; set; }
        public long? AssignedUser { get; set; }

        public long Aid { get; internal set; }
    }
}