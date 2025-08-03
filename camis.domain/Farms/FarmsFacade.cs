using System;
using System.Collections.Generic;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Farms.StateMachines;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;

namespace intapscamis.camis.domain.Farms
{
    public interface IFarmsFacade : ICamisFacade
    {
        void SetSession(UserSession session);

        IList<FarmOperatorTypeResponse> GetFarmOperatorTypes();
        IList<FarmTypeResponse> GetFarmTypes();
        IList<RegistrationAuthorityResponse> GetRegistrationAuthorities();
        IList<RegistrationTypeResponse> GetRegistrationTypes();
        IList<FarmOperatorOriginResponse> GetFarmOperatorOrigins();
        PaginatorResponse<FarmOperatorResponse> GetFarmOperators(int skip, int take);
        PaginatorResponse<FarmResponse> GetFarms(int skip, int take);
        IList<string> GetUPINs();

        PaginatorResponse<FarmOperatorResponse> SearchFarmOperators(string term, int skip, int take);
        PaginatorResponse<FarmResponse> SearchFarms(string term, int skip, int take);

        FarmOperatorResponse GetFarmOperator(Guid id);
        FarmResponse GetFarm(Guid id);
        FarmResponse GetFarmByActivity(Guid activityId);

        WorkItemResponse GetLastWorkItem(Guid workflowId);
        Document InWorkItemRegistrationFile(Guid workItemId, int regId);
        Document InWorkItemOperatorRegistrationFile(Guid workItemId, int regId);
        Document InWorkItemActivityPlanFile(Guid workItemId, Guid documentId);
        Document InWorkItemActivityPlanFileForPlanUpdate(Guid workItemId, Guid documentId);
        
        Guid SaveNewFarmRegistration(FarmRequest body, string description);
        void SaveFarmRegistration(Guid workflowId, FarmRequest body, string description);
        Guid RequestNewFarmRegistration(FarmRequest body, string description);
        void CancelFarmRegistration(Guid workflowId, string description);
        void RequestFarmRegistration(Guid workflowId, FarmRequest body, string description);
        void RejectFarmRegistration(Guid workflowId, string description);
        void ApproveFarmRegistration(Guid workflowId, string description);

        Guid RequestNewFarmModification(FarmRequest body, string description);
        void CancelFarmModification(Guid workflowId, string description);
        void RequestFarmModification(Guid workflowId, FarmRequest body, string description);
        void RejectFarmModification(Guid workflowId, string description);
        void ApproveFarmModification(Guid workflowId, string description);

        Guid RequestNewFarmDeletion(FarmRequest body, string description);
        void RejectFarmDeletion(Guid workflowId, string description);
        void ApproveFarmDeletion(Guid workflowId, string description);
        
        void NewWaitLandAssignment(FarmRequest body, string description);
        void WaitLandAssignment(Guid workflowId, FarmRequest body, string description);
        int GetTransferStatus(Guid workflowId);
        void CertifyLandAssignment(Guid workflowId, FarmRequest body, string description);
    }

    public class FarmsFacade : CamisFacade, IFarmsFacade
    {
        private UserSession _session;
        
        private readonly IFarmsService _service;
        private readonly LandAssignmentWorkflow _landAssignmentWorkflow;
        private readonly FarmRegistrationWorkflow _farmRegistrationWorkflow;
        private readonly FarmModificationWorkflow _farmModificationWorkflow;
        private readonly FarmDeletionWorkflow _farmDeletionWorkflow;

        private readonly CamisContext _context;
        
        public FarmsFacade(
            CamisContext context,
            IFarmsService service,
            IWorkflowService workflowService
        )
        {
            _context = context;
            
            _service = service;
            _landAssignmentWorkflow = new LandAssignmentWorkflow(
                _service,
                workflowService,
                new LandBankTransferWorkflow(new LandBankService())
            );
            _farmRegistrationWorkflow = new FarmRegistrationWorkflow(_service, workflowService, _landAssignmentWorkflow);
            _farmModificationWorkflow = new FarmModificationWorkflow(_service, workflowService);
            _farmDeletionWorkflow = new FarmDeletionWorkflow(_service, workflowService);
        }

        public void SetSession(UserSession session)
        {
            _session = session;
            
            _service.SetSession(_session);
            _landAssignmentWorkflow.SetSession(session);
            _farmRegistrationWorkflow.SetSession(session);
            _farmModificationWorkflow.SetSession(session);
            _farmDeletionWorkflow.SetSession(session);
        }


        public IList<FarmOperatorTypeResponse> GetFarmOperatorTypes()
        {
            PassContext(_service, _context);
            return _service.GetFarmOperatorTypes();
        }

        public IList<FarmTypeResponse> GetFarmTypes()
        {
            PassContext(_service, _context);
            return _service.GetFarmTypes();
        }

        public IList<RegistrationAuthorityResponse> GetRegistrationAuthorities()
        {
            PassContext(_service, _context);
            return _service.GetRegistrationAuthorities();
        }

        public IList<RegistrationTypeResponse> GetRegistrationTypes()
        {
            PassContext(_service, _context);
            return _service.GetRegistrationTypes();
        }

        public IList<FarmOperatorOriginResponse> GetFarmOperatorOrigins()
        {
            PassContext(_service, _context);
            return _service.GetFarmOperatorOrigins();
        }

        public PaginatorResponse<FarmOperatorResponse> GetFarmOperators(int skip, int take)
        {
            PassContext(_service, _context);
            return _service.GetFarmOperators(skip, take);
        }

        public PaginatorResponse<FarmResponse> GetFarms(int skip, int take)
        {
            PassContext(_service, _context);
            return _service.GetFarms(skip, take);
        }

        public IList<string> GetUPINs()
        {
            PassContext(_service, _context);
            return _service.GetUPINs();
        }


        public PaginatorResponse<FarmOperatorResponse> SearchFarmOperators(string term, int skip, int take)
        {
            PassContext(_service, _context);
            return _service.SearchFarmOperators(term, skip, take);
        }

        public PaginatorResponse<FarmResponse> SearchFarms(string term, int skip, int take)
        {
            PassContext(_service, _context);
            return _service.SearchFarms(term, skip, take);
        }


        public FarmOperatorResponse GetFarmOperator(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetFarmOperator(id);
        }

        public FarmResponse GetFarm(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetFarm(id);
        }

        public FarmResponse GetFarmByActivity(Guid activityId)
        {
            PassContext(_service, _context);
            return _service.GetFarmByActivity(activityId);
        }
        
         
        public WorkItemResponse GetLastWorkItem(Guid workflowId)
        {
            PassContext(_service, _context);
            return _service.GetLastWorkItem(workflowId);
        }

        public Document InWorkItemRegistrationFile(Guid workItemId, int regId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemRegistrationFile(workItemId, regId);
        }
        
        public Document InWorkItemOperatorRegistrationFile(Guid workItemId, int regId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemOperatorRegistrationFile(workItemId, regId);
        }
        
        public Document InWorkItemActivityPlanFile(Guid workItemId, Guid documentId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemActivityPlanFile(workItemId, documentId);
        }
        
        public Document InWorkItemActivityPlanFileForPlanUpdate(Guid workItemId, Guid documentId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemActivityPlanFileForPlanUpdate(workItemId, documentId);
        }


        public Guid SaveNewFarmRegistration(FarmRequest body, string description)
        {
            return Transact(_context, t =>
            {
                PassContext(_farmRegistrationWorkflow, _context);
                _farmRegistrationWorkflow.ConfigureMachine();

                var workflowId = _farmRegistrationWorkflow.Workflow.Id;

                _farmRegistrationWorkflow.Fire(workflowId, FarmRegistrationWorkflow.ParameterizedTriggers.Save, body,
                    description ?? "Save a new farm registration.",
                    null);

                return workflowId;
            });
        }

        public void SaveFarmRegistration(Guid workflowId, FarmRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmRegistrationWorkflow, _context);
                _farmRegistrationWorkflow.ConfigureMachine(workflowId);

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id, FarmRegistrationWorkflow.ParameterizedTriggers.Save, body,
                    description ?? "Save a farm registration.",
                    null);
            });
        }

        public Guid RequestNewFarmRegistration(FarmRequest body, string description)
        {
                return Transact(_context, t =>
            {
                PassContext(_farmRegistrationWorkflow, _context);
                _farmRegistrationWorkflow.ConfigureMachine();

                var workflowId = _farmRegistrationWorkflow.Workflow.Id;

                _farmRegistrationWorkflow.Fire(workflowId, FarmRegistrationWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Request a new farm registration.",
                    null);

                return workflowId;
            });
        }

        public void CancelFarmRegistration(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmRegistrationWorkflow, _context);
                _farmRegistrationWorkflow.ConfigureMachine(workflowId);

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id, FarmRegistrationWorkflow.ParameterizedTriggers.Cancel,
                    description ?? "Cancel a farm registration.",
                    null);
            });
        }

        public void RequestFarmRegistration(Guid workflowId, FarmRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmRegistrationWorkflow, _context);
                _farmRegistrationWorkflow.ConfigureMachine(workflowId);

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id, FarmRegistrationWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Request a farm registration.",
                    null);
            });
        }

        public void RejectFarmRegistration(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmRegistrationWorkflow, _context);
                _farmRegistrationWorkflow.ConfigureMachine(workflowId);

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id, FarmRegistrationWorkflow.ParameterizedTriggers.Reject,
                    description ?? "Reject a farm registration request.",
                    null);
            });
        }

        public void ApproveFarmRegistration(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmRegistrationWorkflow, _context);
                _farmRegistrationWorkflow.ConfigureMachine(workflowId);

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id, FarmRegistrationWorkflow.ParameterizedTriggers.Approve,
                    description ?? "Approve a farm registration request.",
                    null);
            });
        }


        public Guid RequestNewFarmModification(FarmRequest body, string description)
        {
            return Transact(_context, t =>
            {
                PassContext(_farmModificationWorkflow, _context);
                _farmModificationWorkflow.ConfigureMachine();

                var workflowId = _farmModificationWorkflow.Workflow.Id;

                _farmModificationWorkflow.Fire(workflowId, FarmModificationWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Request a new farm modification.",
                    null);

                return workflowId;
            });
        }

        public void CancelFarmModification(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmModificationWorkflow, _context);
                _farmModificationWorkflow.ConfigureMachine(workflowId);

                _farmModificationWorkflow.Fire(_farmModificationWorkflow.Workflow.Id, FarmModificationWorkflow.ParameterizedTriggers.Cancel,
                    description ?? "Cancel a farm modification.",
                    null);
            });
        }

        public void RequestFarmModification(Guid workflowId, FarmRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmModificationWorkflow, _context);
                _farmModificationWorkflow.ConfigureMachine(workflowId);

                _farmModificationWorkflow.Fire(_farmModificationWorkflow.Workflow.Id, FarmModificationWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Request a farm modification.",
                    null);
            });
        }

        public void RejectFarmModification(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmModificationWorkflow, _context);
                _farmModificationWorkflow.ConfigureMachine(workflowId);

                _farmModificationWorkflow.Fire(_farmModificationWorkflow.Workflow.Id, FarmModificationWorkflow.ParameterizedTriggers.Reject,
                    description ?? "Reject a farm modification request.",
                    null);
            });
        }

        public void ApproveFarmModification(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmModificationWorkflow, _context);
                _farmModificationWorkflow.ConfigureMachine(workflowId);

                _farmModificationWorkflow.Fire(_farmModificationWorkflow.Workflow.Id, FarmModificationWorkflow.ParameterizedTriggers.Approve,
                    description ?? "Approve a farm modification request.",
                    null);
            });
        }


        public Guid RequestNewFarmDeletion(FarmRequest body, string description)
        {
            return Transact(_context, t =>
            {
                PassContext(_farmDeletionWorkflow, _context);
                _farmDeletionWorkflow.ConfigureMachine();

                var workflowId = _farmDeletionWorkflow.Workflow.Id;

                _farmDeletionWorkflow.Fire(workflowId, FarmDeletionWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Request a farm deletion.",
                    null);

                return workflowId;
            });
        }

        public void RejectFarmDeletion(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmDeletionWorkflow, _context);
                _farmDeletionWorkflow.ConfigureMachine(workflowId);

                _farmDeletionWorkflow.Fire(_farmDeletionWorkflow.Workflow.Id, FarmDeletionWorkflow.ParameterizedTriggers.Reject,
                    description ?? "Reject a farm deletion request.",
                    null);
            });
        }

        public void ApproveFarmDeletion(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_farmDeletionWorkflow, _context);
                _farmDeletionWorkflow.ConfigureMachine(workflowId);

                _farmDeletionWorkflow.Fire(_farmDeletionWorkflow.Workflow.Id, FarmDeletionWorkflow.ParameterizedTriggers.Approve,
                    description ?? "Approve a farm deletion request.",
                    null);
            });
        }
        
        
        public void NewWaitLandAssignment(FarmRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_landAssignmentWorkflow, _context);
                _landAssignmentWorkflow.ConfigureMachine();

                _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id, LandAssignmentWorkflow.ParameterizedTriggers.Wait, body,
                    description ?? "Wait for NRLAIS to assign new land to this farm.",
                    null);
            });
        }
        
        public void WaitLandAssignment(Guid workflowId, FarmRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_landAssignmentWorkflow, _context);
                _landAssignmentWorkflow.ConfigureMachine(workflowId);

                _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id, LandAssignmentWorkflow.ParameterizedTriggers.Wait, body,
                    description ?? "Wait for NRLAIS to assign land to this farm.",
                    null);
            });
        }
        
        public int GetTransferStatus(Guid workflowId)
        {
            return Transact(_context, t =>
            {
                PassContext(_landAssignmentWorkflow, _context);
                _landAssignmentWorkflow.ConfigureMachine(workflowId);

                return _landAssignmentWorkflow.GetTransferStatus();
            });
        }

        public void CertifyLandAssignment(Guid workflowId, FarmRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_landAssignmentWorkflow, _context);
                _landAssignmentWorkflow.ConfigureMachine(workflowId);

                _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id,
                    LandAssignmentWorkflow.ParameterizedTriggers.Certify, body,
                    description ?? "Certify a farm registration's land.", null);
            });
        }
    }
}