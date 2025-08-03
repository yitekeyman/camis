using System;

namespace intapscamis.camis.domain.Infrastructure.Architecture
{
    public interface ICamisWorkflow : ICamisService
    {
        void ConfigureMachine();                   // for a new workflow
        void ConfigureMachine(Guid workflowId);    // for an existing workflow
    }
    
    public abstract class CamisWorkflow : CamisService, ICamisWorkflow
    {
        public abstract void ConfigureMachine();
        public abstract void ConfigureMachine(Guid workflowId);
    }
}