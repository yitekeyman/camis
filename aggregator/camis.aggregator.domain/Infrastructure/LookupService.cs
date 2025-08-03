using camis.aggregator.data.Entities;
using System.Linq;


namespace camis.aggregator.domain.Infrastructure
{
    public interface ILookupService
    {
        object GetRoles();
    }

    public class LookupService : ILookupService
    {
        private readonly aggregatorContext _aggregatorContext;

        public LookupService(aggregatorContext context)
        {
            _aggregatorContext = context;
        }

        public object GetRoles()
        {
            return _aggregatorContext.Role.Select(role => new {id = role.Id, name = role.Name}).ToList();
        }
    }
}