using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;

namespace intapscamis.camis.domain.ContractManagement;


public interface IContractManagementFacade : ICamisFacade
{
    void SetSession(UserSession session);
}
public class ContractManagementFacade: CamisFacade, IContractManagementFacade
{
    private UserSession _session;
    private readonly CamisContext _context;
    public readonly IContractManagementService _service;

    public ContractManagementFacade(CamisContext context, IContractManagementService service)
    {
        _context = context;

        _service = service;
    }

    public void SetSession(UserSession session)
    {
        _session = session;

        _service.SetSession(_session);
    }
}