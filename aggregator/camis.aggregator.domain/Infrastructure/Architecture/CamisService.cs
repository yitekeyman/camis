using camis.aggregator.data.Entities;

using Stateless;
using System;

namespace camis.aggregator.domain.Infrastructure.Architecture
{
    public interface ICamisService
    {
        aggregatorContext GetContext();
        void SetContext(aggregatorContext value);
    }

    public class CamisService : ICamisService
    {
        protected aggregatorContext Context;

        public virtual aggregatorContext GetContext()
        {
            return Context;
        }

        public virtual void SetContext(aggregatorContext value)
        {
            Context = value;
        }
    }
    
}