using intapscamis.camis.domain.Documents;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Projects;
using intapscamis.camis.domain.Workflows;

namespace intapscamis.camis.domain.ContractManagement;

public interface IContractManagementService : ICamisService
{
    void SetSession(UserSession session);
}
public class ContractManagementService:CamisService, IContractManagementService
{
    private readonly IProjectService _projectService;
    private readonly IDocumentService _documentService;
    private readonly IWorkflowService _workflowService;

    private UserSession _session;

    public ContractManagementService(IProjectService projectService, IDocumentService documentService,
        IWorkflowService workflowService)
    {
        _projectService = projectService;
        _documentService = documentService;
        _workflowService = workflowService;
    }
    public void SetSession(UserSession session)
    {
        _session = session;
        _projectService.SetSession(session);
        _documentService.SetSession(session);
        _workflowService.SetSession(session);
    }
}