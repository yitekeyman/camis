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

        PaginatorResponse<FarmResponse> SearchFarms(string term, int ownerType, int farmType, int status, int skip,
            int take);

        FarmOperatorResponse GetFarmOperator(Guid id);
        FarmResponse GetFarm(Guid id);
        FarmResponse GetFarmByActivity(Guid activityId);

        WorkItemResponse GetLastWorkItem(Guid workflowId);
        Document InWorkItemRegistrationFile(Guid workItemId, int regId);
        Document InWorkItemOperatorRegistrationFile(Guid workItemId, int regId);
        Document InWorkItemActivityPlanFile(Guid workItemId, Guid documentId);
        Document InWorkItemActivityPlanFileForPlanUpdate(Guid workItemId, Guid documentId);
        Document InWorkItemOperatorPhoto(Guid workItemId, Guid photoId);

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
        void ApproveLandAssignment(Guid workflowId, string description);
        void RejectLandAssignment(Guid workflowId, string description);
        int GetTransferStatus(Guid workflowId);
        void CertifyLandAssignment(Guid workflowId, FarmRequest body, string description);
        FarmResponse GetFarmByLandId(Guid id);
        IList<LandBankFacadeModel.UpinWithSplitResponse> GetUPINsWithSplitParts();
        LandBankFacadeModel.UpinWithSplitResponse GetSplitPartsByUpin(string upin);
        LandRightsResponse GetLandRightsByLandIdandFarmId(Guid landId, Guid farmId, int? partId = 0);
        List<LandRightsResponse> GetAllLandRightsByLandId(Guid landId);
        List<LandRightsResponse> GetAllLandRightsByFarmId(Guid farmId);
        List<FarmLandResponse2> GetFarmLands(Guid farmId);
        void GetTransferWorkFlows();
        Document InWorkItemContractCancellationDoc(Guid workItemId, Guid documentId);
        Document InWorkItemContractRenewalDoc(Guid workItemId, Guid documentId);
        Document InWorkItemContractUpdateDoc(Guid workItemId, Guid documentId);
        void RequestContractCancellation(Guid workflowId, ContractCancellationRequest body, string description);
        void ApproveContractCancellation(Guid workflowId, string description);
        void RejectContractCancellation(Guid workflowId, string description);
        void CancelContractCancellation(Guid workflowId, string description);
        void RequestContractModification(Guid workflowId, ContractModificationRequest body, string description);
        void ApproveContractModification(Guid workflowId, string description);
        void RejectContractModification(Guid workflowId, string description);
        void CancelContractModification(Guid workflowId, string description);
        object GetAllModificationReasonList();
        
        Document InWorkItemContractWarningDoc(Guid workItemId, Guid documentId);
        void RequestContractWarning(Guid workflowId, ContractWarningRequest body, string description);
        void ApproveContractWarning(Guid workflowId, string description);
        void RejectContractWarning(Guid workflowId, string description);
        void CancelContractWarning(Guid workflowId, string description);
        FarmWarningResponse GetRightWarning(Guid farmId, Guid landId, int splitIndex);
        
        void RequestContractRenewal(Guid workflowId, ContractRenewalRequest body, string description);
        void ApproveContractRenewal(Guid workflowId, string description);
        void RejectContractRenewal(Guid workflowId, string description);
        void CancelContractRenewal(Guid workflowId, string description);
    }

    public class FarmsFacade : CamisFacade, IFarmsFacade
    {
        private UserSession _session;

        private readonly IFarmsService _service;
        private readonly LandAssignmentWorkflow _landAssignmentWorkflow;
        private readonly LandBankTransferWorkflow _landBankTransferWorkflow;
        private readonly FarmRegistrationWorkflow _farmRegistrationWorkflow;
        private readonly FarmModificationWorkflow _farmModificationWorkflow;
        private readonly FarmDeletionWorkflow _farmDeletionWorkflow;
        private readonly ContractCancellationWorkflow _contractCancellationWorkflow;
        private readonly ContractModificationWorkflow _contractModificationWorkflow;
        private readonly FarmWarningWorkflow _contractWarningWorkflow;
        private readonly ContractRenewWorkflow _contractRenewWorkflow;

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
            _farmRegistrationWorkflow =
                new FarmRegistrationWorkflow(_service, workflowService, _landAssignmentWorkflow);
            _farmModificationWorkflow = new FarmModificationWorkflow(_service, workflowService, new LandBankService());
            _farmDeletionWorkflow = new FarmDeletionWorkflow(_service, workflowService);
            _landBankTransferWorkflow = new LandBankTransferWorkflow(new LandBankService());
            _contractCancellationWorkflow =
                new ContractCancellationWorkflow(_service, workflowService, new LandBankService());
            _contractModificationWorkflow =
                new ContractModificationWorkflow(_service, workflowService, new LandBankService());
            _contractWarningWorkflow = new FarmWarningWorkflow(_service, workflowService, new LandBankService());
            _contractRenewWorkflow = new ContractRenewWorkflow(_service, workflowService, new LandBankService());
        }

        public void SetSession(UserSession session)
        {
            _session = session;

            _service.SetSession(_session);
            _landAssignmentWorkflow.SetSession(session);
            _farmRegistrationWorkflow.SetSession(session);
            _farmModificationWorkflow.SetSession(session);
            _farmDeletionWorkflow.SetSession(session);
            _landBankTransferWorkflow.SetSession(session);
            _contractCancellationWorkflow.SetSession(session);
            _contractModificationWorkflow.SetSession(session);
            _contractWarningWorkflow.SetSession(session);
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

        public PaginatorResponse<FarmResponse> SearchFarms(string term, int ownerType, int farmType, int status,
            int skip, int take)
        {
            PassContext(_service, _context);
            return _service.SearchFarms(term, ownerType, farmType, status, skip, take);
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

        public Document InWorkItemOperatorPhoto(Guid workItemId, Guid photoId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemOperatorPhoto(workItemId, photoId);
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

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id,
                    FarmRegistrationWorkflow.ParameterizedTriggers.Save, body,
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

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id,
                    FarmRegistrationWorkflow.ParameterizedTriggers.Cancel,
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

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id,
                    FarmRegistrationWorkflow.ParameterizedTriggers.Request, body,
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

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id,
                    FarmRegistrationWorkflow.ParameterizedTriggers.Reject,
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

                _farmRegistrationWorkflow.Fire(_farmRegistrationWorkflow.Workflow.Id,
                    FarmRegistrationWorkflow.ParameterizedTriggers.Approve,
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

                _farmModificationWorkflow.Fire(_farmModificationWorkflow.Workflow.Id,
                    FarmModificationWorkflow.ParameterizedTriggers.Cancel,
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

                _farmModificationWorkflow.Fire(_farmModificationWorkflow.Workflow.Id,
                    FarmModificationWorkflow.ParameterizedTriggers.Request, body,
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

                _farmModificationWorkflow.Fire(_farmModificationWorkflow.Workflow.Id,
                    FarmModificationWorkflow.ParameterizedTriggers.Reject,
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

                _farmModificationWorkflow.Fire(_farmModificationWorkflow.Workflow.Id,
                    FarmModificationWorkflow.ParameterizedTriggers.Approve,
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

                _farmDeletionWorkflow.Fire(_farmDeletionWorkflow.Workflow.Id,
                    FarmDeletionWorkflow.ParameterizedTriggers.Reject,
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

                _farmDeletionWorkflow.Fire(_farmDeletionWorkflow.Workflow.Id,
                    FarmDeletionWorkflow.ParameterizedTriggers.Approve,
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


                _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id,
                    LandAssignmentWorkflow.ParameterizedTriggers.Wait, body,
                    description ?? "Parcel locating request sent for farm suppervisor",
                    null);

                // _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id,
                //     LandAssignmentWorkflow.ParameterizedTriggers.Wait, body,
                //     description ?? "Wait for NRLAIS to assign land to this farm.",
                //     null);
            });
        }

        public void WaitLandAssignment(Guid workflowId, FarmRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_landAssignmentWorkflow, _context);
                _landAssignmentWorkflow.ConfigureMachine(workflowId);

                _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id,
                    LandAssignmentWorkflow.ParameterizedTriggers.Wait, body,
                    description ?? "Parcel locating request sent for farm suppervisor",
                    null);

                // _landAssignmentWorkflow.Fire(_landAssignmentWorkflow.Workflow.Id,
                //     LandAssignmentWorkflow.ParameterizedTriggers.Wait, body,
                //     description ?? "Wait for NRLAIS to assign land to this farm.",
                //     null);
            });
        }

        public void ApproveLandAssignment(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_landBankTransferWorkflow, _context);
                _landBankTransferWorkflow.ApproveLandTransfer(workflowId, description);
            });
            GetTransferWorkFlows();
        }

        public void RejectLandAssignment(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_landBankTransferWorkflow, _context);
                _landBankTransferWorkflow.RejectLandTransfer(workflowId, description);
            });
        }

        public void RerequestLandAssignment(Guid workflowId, FarmRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_landBankTransferWorkflow, _context);
                _landBankTransferWorkflow.RerequestApproval(workflowId, body, description);
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

        public FarmResponse GetFarmByLandId(Guid id)
        {
            PassContext(_service, _context);
            return _service.GetFarmByLandId(id);
        }

        public IList<LandBankFacadeModel.UpinWithSplitResponse> GetUPINsWithSplitParts()
        {
            PassContext(_service, _context);
            return _service.GetUPINsWithSplitParts();
        }

        public LandBankFacadeModel.UpinWithSplitResponse GetSplitPartsByUpin(string upin)
        {
            PassContext(_service, _context);
            return _service.GetSplitPartsByUpin(upin);
        }

        public LandRightsResponse GetLandRightsByLandIdandFarmId(Guid landId, Guid farmId, int? partId = 0)
        {
            PassContext(_service, _context);
            return _service.GetLandRightsByLandIdandFarmId(landId, farmId, partId);
        }

        public List<LandRightsResponse> GetAllLandRightsByLandId(Guid landId)
        {
            PassContext(_service, _context);
            return _service.GetAllLandRightsByLandId(landId);
        }

        public List<LandRightsResponse> GetAllLandRightsByFarmId(Guid farmId)
        {
            PassContext(_service, _context);
            return _service.GetAllLandRightsByFarmId(farmId);
        }

        public List<FarmLandResponse2> GetFarmLands(Guid farmId)
        {
            PassContext(_service, _context);
            return _service.GetFarmLands(farmId);
        }

        public void GetTransferWorkFlows()
        {
            PassContext(_service, _context);
            var workflows = _service.GetTransferWorkFlows();
            foreach (var wf in workflows)
            {
                GetTransferStatus(wf.Id);
            }
        }

        public Document InWorkItemContractCancellationDoc(Guid workItemId, Guid documentId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemContractCancellationDoc(workItemId, documentId);
        }

       public Document InWorkItemContractUpdateDoc(Guid workItemId, Guid documentId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemContractUpdateDoc(workItemId, documentId);
        }
        public Document InWorkItemContractRenewalDoc(Guid workItemId, Guid documentId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemContractRenewalDoc(workItemId, documentId);
        }
        public void RequestContractCancellation(Guid workflowId, ContractCancellationRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractCancellationWorkflow, _context);
                if (workflowId == Guid.Empty)
                    _contractCancellationWorkflow.ConfigureMachine();
                else
                    _contractCancellationWorkflow.ConfigureMachine(workflowId);

                _contractCancellationWorkflow.Fire(_contractCancellationWorkflow.Workflow.Id,
                    ContractCancellationWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Contract Cancellation Request for FS.",
                    null);
            });
        }

        public void ApproveContractCancellation(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractCancellationWorkflow, _context);
                _contractCancellationWorkflow.ConfigureMachine(workflowId);

                _contractCancellationWorkflow.Fire(_contractCancellationWorkflow.Workflow.Id,
                    ContractCancellationWorkflow.ParameterizedTriggers.Approve,
                    description ?? "FS Approved Contract Cancellation.",
                    null);
            });
        }

        public void RejectContractCancellation(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractCancellationWorkflow, _context);
                _contractCancellationWorkflow.ConfigureMachine(workflowId);

                _contractCancellationWorkflow.Fire(_contractCancellationWorkflow.Workflow.Id,
                    ContractCancellationWorkflow.ParameterizedTriggers.Reject,
                    description ?? "FS Rejected Contract Cancellation.",
                    null);
            });
        }

        public void CancelContractCancellation(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractCancellationWorkflow, _context);
                _contractCancellationWorkflow.ConfigureMachine(workflowId);

                _contractCancellationWorkflow.Fire(_contractCancellationWorkflow.Workflow.Id,
                    ContractCancellationWorkflow.ParameterizedTriggers.Cancel,
                    description ?? "FS/FC Cancel Contract Cancellation.",
                    null);
            });
        }
        
          public void RequestContractModification(Guid workflowId, ContractModificationRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractModificationWorkflow, _context);
                if (workflowId == Guid.Empty)
                    _contractModificationWorkflow.ConfigureMachine();
                else
                    _contractModificationWorkflow.ConfigureMachine(workflowId);

                _contractModificationWorkflow.Fire(_contractModificationWorkflow.Workflow.Id,
                    ContractModificationWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Contract Renewal/Modification Request for FS.",
                    null);
            });
        }

        public void ApproveContractModification(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractModificationWorkflow, _context);
                _contractModificationWorkflow.ConfigureMachine(workflowId);

                _contractModificationWorkflow.Fire(_contractModificationWorkflow.Workflow.Id,
                    ContractModificationWorkflow.ParameterizedTriggers.Approve,
                    description ?? "FS Approved Contract Renewal/Modification.",
                    null);
            });
        }

        public void RejectContractModification(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractModificationWorkflow, _context);
                _contractModificationWorkflow.ConfigureMachine(workflowId);

                _contractModificationWorkflow.Fire(_contractModificationWorkflow.Workflow.Id,
                    ContractModificationWorkflow.ParameterizedTriggers.Reject,
                    description ?? "FS Rejected Contract Renewal/Modification.",
                    null);
            });
        }

        public void CancelContractModification(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractModificationWorkflow, _context);
                _contractModificationWorkflow.ConfigureMachine(workflowId);

                _contractModificationWorkflow.Fire(_contractModificationWorkflow.Workflow.Id,
                    ContractModificationWorkflow.ParameterizedTriggers.Cancel,
                    description ?? "FS/FC Cancel Contract Renewal/Modification.",
                    null);
            });
        }

        public object GetAllModificationReasonList()
        {
            PassContext(_service, _context);
            return _service.GetAllModificationReasonList();
        }

        public Document InWorkItemContractWarningDoc(Guid workItemId, Guid documentId)
        {
            PassContext(_service, _context);
            return _service.InWorkItemContractWarningDoc(workItemId, documentId);
        }
        
         public void RequestContractWarning(Guid workflowId, ContractWarningRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractWarningWorkflow, _context);
                if (workflowId == Guid.Empty)
                    _contractWarningWorkflow.ConfigureMachine();
                else
                    _contractWarningWorkflow.ConfigureMachine(workflowId);

                _contractWarningWorkflow.Fire(_contractWarningWorkflow.Workflow.Id,
                    FarmWarningWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Farm Contract Warning Registration Request for FS.",
                    null);
            });
        }

        public void ApproveContractWarning(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractWarningWorkflow, _context);
                _contractWarningWorkflow.ConfigureMachine(workflowId);

                _contractWarningWorkflow.Fire(_contractWarningWorkflow.Workflow.Id, FarmWarningWorkflow.ParameterizedTriggers.Approve,
                    description ?? "FS Approved Contract Warning Registration.",
                    null);
            });
        }

        public void RejectContractWarning(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractWarningWorkflow, _context);
                _contractWarningWorkflow.ConfigureMachine(workflowId);

                _contractWarningWorkflow.Fire(_contractWarningWorkflow.Workflow.Id,
                    FarmWarningWorkflow.ParameterizedTriggers.Reject,
                    description ?? "FS Rejected Contract Warning Registration.",
                    null);
            });
        }

        public void CancelContractWarning(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractWarningWorkflow, _context);
                _contractWarningWorkflow.ConfigureMachine(workflowId);

                _contractWarningWorkflow.Fire(_contractWarningWorkflow.Workflow.Id,
                    FarmWarningWorkflow.ParameterizedTriggers.Cancel,
                    description ?? "FS/FC Cancel Contract Warning Registration.",
                    null);
            });
        }

        public FarmWarningResponse GetRightWarning(Guid farmId, Guid landId, int splitIndex)
        {
            PassContext(_service, _context);
            return _service.GetRightWarning(farmId, landId, splitIndex);
        }
        
       public void RequestContractRenewal(Guid workflowId, ContractRenewalRequest body, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractRenewWorkflow, _context);
                if (workflowId == Guid.Empty)
                    _contractRenewWorkflow.ConfigureMachine();
                else
                    _contractRenewWorkflow.ConfigureMachine(workflowId);

                _contractRenewWorkflow.Fire(_contractRenewWorkflow.Workflow.Id,
                    ContractRenewWorkflow.ParameterizedTriggers.Request, body,
                    description ?? "Farm Contract Renewal Request for FS.",
                    null);
            });
        }

        public void ApproveContractRenewal(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractRenewWorkflow, _context);
                _contractRenewWorkflow.ConfigureMachine(workflowId);

                _contractRenewWorkflow.Fire(_contractRenewWorkflow.Workflow.Id, ContractRenewWorkflow.ParameterizedTriggers.Approve,
                    description ?? "FS Approved Contract Renewal request.",
                    null);
            });
        }

        public void RejectContractRenewal(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractRenewWorkflow, _context);
                _contractRenewWorkflow.ConfigureMachine(workflowId);

                _contractRenewWorkflow.Fire(_contractRenewWorkflow.Workflow.Id,
                    ContractRenewWorkflow.ParameterizedTriggers.Reject,
                    description ?? "FS Rejected Contract Renewal request.",
                    null);
            });
        }

        public void CancelContractRenewal(Guid workflowId, string description)
        {
            Transact(_context, t =>
            {
                PassContext(_contractRenewWorkflow, _context);
                _contractRenewWorkflow.ConfigureMachine(workflowId);

                _contractRenewWorkflow.Fire(_contractRenewWorkflow.Workflow.Id,
                    ContractRenewWorkflow.ParameterizedTriggers.Cancel,
                    description ?? "FS/FC Cancel Contract Renewal request.",
                    null);
            });
        }
    }
}