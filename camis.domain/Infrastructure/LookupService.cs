using System.Linq;
using intapscamis.camis.data.Entities;

namespace intapscamis.camis.domain.Infrastructure
{
    public interface ILookupService
    {
        object GetRoles();
        object GetLandType();
        object GetFarmStatusType();
    }

    public class LookupService : ILookupService
    {
        private readonly CamisContext _camisContext;

        public LookupService(CamisContext context)
        {
            _camisContext = context;
        }

        public object GetRoles()
        {
            return _camisContext.Role.Select(role => new {id = role.Id, name = role.Name}).ToList();
        }

        public object GetLandType()
        {
            return _camisContext.LandType.Select(lt => new {id = lt.Id, name = lt.Name}).ToList();
        }

        public object GetFarmStatusType()
        {
            return _camisContext.FarmStatusTypes.Select(lt => new {id = lt.Id, name = lt.Name}).ToList();
        }
    }
}