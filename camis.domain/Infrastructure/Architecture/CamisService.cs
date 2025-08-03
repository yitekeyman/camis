using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Stateless;
using System;

namespace intapscamis.camis.domain.Infrastructure.Architecture
{
    public interface ICamisService
    {
        CamisContext GetContext();
        void SetContext(CamisContext value);
    }

    public class CamisService : ICamisService
    {
        protected CamisContext Context;

        public virtual CamisContext GetContext()
        {
            return Context;
        }

        public virtual void SetContext(CamisContext value)
        {
            Context = value;
        }
    }
    public class StatlessWorkFlowServiceBase<TrigerType,StateType>:CamisService
    {
        protected StateMachine<StateType, TrigerType> _machine;
        protected IWorkflowService _workflowService;

        protected virtual int ConverToInt(TrigerType triger)
        {
            return Convert.ToInt32(triger);
        }
        protected virtual int ConverToInt(StateType state)
        {
            return Convert.ToInt32(state);
        }
        protected WorkItem fireAction(Guid workFlowID, TrigerType triger, string note, long? role)
        {
            return fireAction(workFlowID, triger, note, role, null);
        }
        protected WorkItem fireAction(Guid workFlowID, TrigerType triger, string note, long? role, object data)
        {
            var prevState = _machine.State;

            _machine.Fire(triger);

            var wi = _workflowService.CreateWorkItemChangeState(new WorkItemRequest
            {
                AssignedRole = role,
                Data = data == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(data),
                DataType = data == null ? null : data.GetType().ToString(),
                Description = note,
                AssignedUser = null,
                FromState = ConverToInt(prevState),
                ToState = ConverToInt(_machine.State),
                Trigger = ConverToInt(triger),
                WorkflowId = workFlowID.ToString(),
            });
            return wi;
        }

    }
}