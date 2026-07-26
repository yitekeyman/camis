using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using DocumentFormat.OpenXml.Office2010.Excel;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Documents;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Projects;
using intapscamis.camis.domain.Projects.Models;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace intapscamis.camis.domain.Farms
{
    public interface IFarmsService : ICamisService
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

        FarmOperator CreateFarmOperator(FarmOperatorRequest data);
        Activity CreateActivity(ActivityPlanRequest data);
        Farm CreateFarm(FarmRequest data);

        FarmOperator UpdateFarmOperator(FarmOperatorRequest data);
        Activity UpdateActivity(ActivityPlanRequest data);
        Farm UpdateFarm(FarmRequest data);
        void AssignFarmLand(Guid farmId, FarmLandRequest farmLandRequest);

        void DeleteFarmOperator(FarmOperatorRequest data);
        void DeleteFarm(FarmRequest data);

        WorkItemResponse GetLastWorkItem(Guid workflowId);
        Document InWorkItemRegistrationFile(Guid workItemId, int regId);
        Document InWorkItemOperatorRegistrationFile(Guid workItemId, int regId);
        Document InWorkItemActivityPlanFile(Guid workItemId, Guid documentId);
        Document InWorkItemActivityPlanFileForPlanUpdate(Guid workItemId, Guid documentId);
        Document InWorkItemOperatorPhoto(Guid workItemId, Guid photoId);
        FarmResponse GetFarmByLandId(Guid id);
        IList<LandBankFacadeModel.UpinWithSplitResponse> GetUPINsWithSplitParts();
        LandBankFacadeModel.UpinWithSplitResponse GetSplitPartsByUpin(string upin);
        LandRightsResponse GetLandRightsByLandIdandFarmId(Guid landId, Guid farmId, int? partId = 0);
        List<LandRightsResponse> GetAllLandRightsByLandId(Guid landId);
        List<LandRightsResponse> GetAllLandRightsByFarmId(Guid farmId);
        void SetFarmStatus(Guid farmId, int status);
        void SetFarmLock(Guid farmId, bool val);
        List<FarmLandResponse2> GetFarmLands(Guid farmId);
        List<Workflow> GetTransferWorkFlows();
        void CancelContract(ContractCancellationRequest request);
        Document InWorkItemContractCancellationDoc(Guid workItemId, Guid documentId);
        object GetAllModificationReasonList();
        void UpdateContract(ContractModificationRequest request);
        Document InWorkItemContractUpdateDoc(Guid workItemId, Guid documentId);
        Document InWorkItemContractRenewalDoc(Guid workItemId, Guid documentId);
        Document InWorkItemContractWarningDoc(Guid workItemId, Guid documentId);
        void RegisterContractWarning(ContractWarningRequest request);
        FarmWarningResponse GetRightWarning(Guid farmId, Guid landId, int splitIndex);
        void RenewContract(ContractRenewalRequest request);
    }

    public class FarmsService : CamisService, IFarmsService
    {
        private readonly IProjectService _projectService;
        private readonly IDocumentService _documentService;
        private readonly IWorkflowService _workflowService;

        private UserSession _session;

        public FarmsService(IProjectService projectService, IDocumentService documentService,
            IWorkflowService workflowService)
        {
            _projectService = projectService;
            _documentService = documentService;
            _workflowService = workflowService;
        }

        public override void SetContext(CamisContext context)
        {
            base.SetContext(context);
            _projectService.SetContext(context);
            _documentService.SetContext(context);
            _workflowService.SetContext(context);
        }

        public void SetSession(UserSession session)
        {
            _session = session;
            _projectService.SetSession(session);
            _documentService.SetSession(session);
            _workflowService.SetSession(session);
        }


        public IList<FarmOperatorTypeResponse> GetFarmOperatorTypes()
        {
            return Context.FarmOperatorType.OrderBy(item => item.Id).Select(operatorType => new FarmOperatorTypeResponse
            {
                Id = operatorType.Id,
                Name = operatorType.Name
            }).ToList();
        }

        public IList<FarmTypeResponse> GetFarmTypes()
        {
            return Context.FarmType.OrderBy(item => item.Id).Select(farmType => new FarmTypeResponse
            {
                Id = farmType.Id,
                Name = farmType.Name
            }).ToList();
        }

        public IList<RegistrationAuthorityResponse> GetRegistrationAuthorities()
        {
            return Context.RegistrationAuthority.OrderBy(item => item.Id).Select(registrationAuthority =>
                new RegistrationAuthorityResponse
                {
                    Id = registrationAuthority.Id,
                    Name = registrationAuthority.Name
                }).ToList();
        }

        public IList<RegistrationTypeResponse> GetRegistrationTypes()
        {
            return Context.RegistrationType.OrderBy(item => item.Id).Select(registrationType =>
                new RegistrationTypeResponse
                {
                    Id = registrationType.Id,
                    Name = registrationType.Name
                }).ToList();
        }

        public IList<FarmOperatorOriginResponse> GetFarmOperatorOrigins()
        {
            return Context.FarmOperatorOrigin.OrderBy(item => item.Id).Select(operatorOrigin =>
                new FarmOperatorOriginResponse
                {
                    Id = operatorOrigin.Id,
                    Name = operatorOrigin.Name
                }).ToList();
        }

        public PaginatorResponse<FarmOperatorResponse> GetFarmOperators(int skip, int take)
        {
            var farmOperators = Context.FarmOperator.Skip(skip).Take(take).AsEnumerable();
            return new PaginatorResponse<FarmOperatorResponse>
            {
                TotalSize = Context.FarmOperator.Count(),
                Items = farmOperators.Select(MapFarmOperatorToResponse).ToList()
            };
        }

        public PaginatorResponse<FarmResponse> GetFarms(int skip, int take)
        {
            var farms = Context.Farm.Skip(skip).Take(take).AsEnumerable();
            return new PaginatorResponse<FarmResponse>
            {
                TotalSize = Context.Farm.Count(),
                Items = farms.Select(MapFarmToResponse).ToList()
            };
        }

        public IList<string> GetUPINs()
        {
            return Context.LandUpin
                .Where(upin => upin.Land.LandType == 2 || upin.Land.LandType == 5 || upin.Land.LandType == 6)
                .Select(upin => upin.Upin).ToList();
        }

        public IList<LandBankFacadeModel.UpinWithSplitResponse> GetUPINsWithSplitParts()
        {
            var land = Context.LandUpin
                .Where(upin => upin.Land.LandType == 2 || upin.Land.LandType == 5 || upin.Land.LandType == 6)
                .Select(upin => upin.Upin).ToList();
            var rets = new List<LandBankFacadeModel.UpinWithSplitResponse>();
            foreach (var upin in land)
            {
                rets.Add(GetSplitPartsByUpin(upin));
            }

            return rets;
        }

        public LandBankFacadeModel.UpinWithSplitResponse GetSplitPartsByUpin(string upin)
        {
            var land = Context.LandUpin.Where(e => e.Upin.Contains(upin)).FirstOrDefault();

            if (land != null)
            {
                var ret = new LandBankFacadeModel.UpinWithSplitResponse();
                ret.Upin = land.Upin;
                // var sql = $"SELECT id, ST_AsText(geom) AS geom, land_id, indexes, status, wid, area FROM lb.land_split WHERE land_id='{land.LandId}' AND status=2;";
                var split = Context.LandSplit.Where(l => l.LandId == land.LandId && l.Status == 2).ToList();
                var writer = new WKTWriter();
                foreach (var ls in split)
                {
                    ret.SplitParts.Add(new LandBankFacadeModel.LandSplitResponse()
                    {
                        Id = ls.Id,
                        Area = ls.Area,
                        Geom = writer.Write(ls.Geom),
                        Indexes = ls.Indexes,
                        Status = ls.Status,
                        LandId = ls.LandId.ToString()
                    });
                }

                return ret;
            }

            return null;
        }

        public PaginatorResponse<FarmOperatorResponse> SearchFarmOperators(string term, int skip, int take)
        {
            var searchQuery = Context.FarmOperator.Where(o => string.Join("\n",
                o.Name,
                o.Nationality,
                o.Address.Name,
                o.Phone,
                o.Email,
                o.Type.Name
            ).ToLower().Contains(term.ToLower()));
            var farmOperators = searchQuery.Skip(skip).Take(take).AsEnumerable();
            return new PaginatorResponse<FarmOperatorResponse>
            {
                TotalSize = searchQuery.Count(),
                Items = farmOperators.Select(MapFarmOperatorToResponse).ToList()
            };
        }

        public PaginatorResponse<FarmResponse> SearchFarms(string term, int ownerType, int farmType, int status,
            int skip, int take)
        {
            // Normalize search term
            var searchTerm = term?.ToLower() ?? string.Empty;

            // Build base query with all necessary includes
            var baseQuery = Context.Farm
                .Include(f => f.Type)
                .Include(f => f.Activity)
                .Include(f => f.Operator)
                .ThenInclude(o => o.Type)
                .Include(f => f.Operator)
                .ThenInclude(o => o.Origin)
                .Include(f => f.Operator)
                .ThenInclude(o => o.FarmOperatorRegistration)
                .ThenInclude(or => or.Authority)
                .Include(f => f.Operator)
                .ThenInclude(o => o.FarmOperatorRegistration)
                .ThenInclude(or => or.Type)
                .Include(f => f.FarmLand)
                .Include(f => f.FarmRegistration)
                .ThenInclude(fr => fr.Authority)
                .Include(f => f.FarmRegistration)
                .ThenInclude(fr => fr.Type)
                .Include(f => f.StatusNavigation)
                .Where(f =>
                    (f.Type != null && f.Type.Name.ToLower().Contains(searchTerm)) ||
                    (f.Activity != null && f.Activity.Name.ToLower().Contains(searchTerm)) ||
                    (f.Activity != null && f.Activity.Description.ToLower().Contains(searchTerm)) ||
                    (f.InvestedCapital.HasValue && f.InvestedCapital.Value.ToString().Contains(searchTerm)) ||
                    (f.Description != null && f.Description.ToLower().Contains(searchTerm)) ||
                    (f.Operator != null && f.Operator.Name != null && f.Operator.Name.ToLower().Contains(searchTerm)) ||
                    (f.Operator != null && f.Operator.Nationality != null &&
                     f.Operator.Nationality.ToLower().Contains(searchTerm)) ||
                    (f.Operator != null && f.Operator.Phone != null &&
                     f.Operator.Phone.ToLower().Contains(searchTerm)) ||
                    (f.Operator != null && f.Operator.Email != null && f.Operator.Email.ToLower().Contains(searchTerm))
                )
                .AsSplitQuery()
                .AsNoTracking();

            if (farmType > 0)
                baseQuery = baseQuery.Where(f => f.TypeId == farmType);

            if (ownerType > 0)
                baseQuery = baseQuery.Where(f => f.Operator != null && f.Operator.TypeId == ownerType);

            if (status > 0)
            {
                if (status == (int)FarmStatusEnum.TransactionLocked)
                {
                    baseQuery = baseQuery.Where(f => f.Locked == true);
                }
                else
                {
                    baseQuery = baseQuery.Where(f => f.Status == status && f.Locked == false);
                }
            }

            // Get total count
            var totalSize = baseQuery.Count();

            // Apply pagination and materialize results
            var farms = baseQuery
                .Skip(skip)
                .Take(take)
                .ToList();

            // Map to response
            return new PaginatorResponse<FarmResponse>
            {
                TotalSize = totalSize,
                Items = farms.Select(MapFarmToResponse).ToList()
            };
        }

        public FarmOperatorResponse GetFarmOperator(Guid id)
        {
            var farmOperator = Context.FarmOperator.Find(id);
            return MapFarmOperatorToResponse(farmOperator);
        }

        public FarmResponse GetFarm(Guid id)
        {
            return MapSingleFarmData(id);
        }

        public FarmResponse GetFarmByActivity(Guid activityId)
        {
            var farm = Context.Farm.First(f => f.ActivityId == activityId);
            return MapFarmToResponse(farm);
        }


        public FarmOperator CreateFarmOperator(FarmOperatorRequest data)
        {
            Document photo = null;
            if (data.Photo != null)
            {
                var workItemId = _documentService.ExtractWorkItemIdFromOverrideFilePath(data.Photo.OverrideFilePath);
                if (workItemId != Guid.Empty)
                    photo = _documentService.CreateDocumentInFolder(workItemId, data.Photo);
            }


            var email = !string.IsNullOrEmpty(data.Email)
                ? new MailAddress(data.Email).Address
                : ""; // validates the email

            var farmOperator = new FarmOperator
            {
                Id = data.Id?.ToGuid() ?? Guid.NewGuid(),
                Name = data.Name,
                Nationality = data.Nationality,
                TypeId = data.TypeId,
                AddressId = data.AddressId.ToGuid(),
                Phone = data.Phone,
                Email = email,
                OriginId = data.OriginId,
                Capital = data.Capital,
                PhotoId = photo?.Id ?? null,
            };

            if (data.TypeId == 1)
            {
                farmOperator.Gender = data.Gender;
                farmOperator.MartialStatus = data.MartialStatus;
                farmOperator.Birthdate = data.Birthdate;
            }
            else
            {
                farmOperator.Gender = null;
                farmOperator.MartialStatus = null;
                farmOperator.Birthdate = null;
            }

            if (data.TypeId == 6)
            {
                farmOperator.Ventures = data.Ventures.ToList().Select(Guid.Parse).ToArray();
            }
            else
            {
                farmOperator.Ventures = new Guid[] { };
            }


            Context.FarmOperator.Add(farmOperator);

            Context.FarmOperatorRegistration.AddRange(data.Registrations.Select(registration =>
            {
                Document document = null;

                if (registration.Document != null)
                {
                    var workItemId =
                        _documentService.ExtractWorkItemIdFromOverrideFilePath(registration.Document.OverrideFilePath);
                    if (workItemId != Guid.Empty)
                        document = _documentService.CreateDocumentInFolder(workItemId, registration.Document);
                }

                return new FarmOperatorRegistration
                {
                    RegistrationNumber = registration.RegistrationNumber,
                    AuthorityId = registration.AuthorityId,
                    OperatorId = farmOperator.Id,
                    TypeId = registration.TypeId,
                    DocumentId = document?.Id
                };
            }));

            Context.SaveChanges(_session.Username, (int)UserActionType.CreateFarmOperator);

            return farmOperator;
        }

        public Activity CreateActivity(ActivityPlanRequest data)
        {
            var plan = _projectService.CreateActivityPlan(data);
            return plan.RootActivity;
        }

        public Farm CreateFarm(FarmRequest data)
        {
            var farm = new Farm
            {
                Id = data.Id?.ToGuid() ?? Guid.NewGuid(),
                OperatorId = data.OperatorId.ToGuid(),
                TypeId = data.TypeId,
                ActivityId = data.ActivityId.ToGuid(),
                InvestedCapital = data.InvestedCapital,
                Description = data.Description,
                OtherTypeIds = data.OtherTypeIds,
                Status = (int)FarmStatusEnum.Ready,
                Locked = false
            };

            Context.Farm.Add(farm);

            Context.FarmRegistration.AddRange(data.Registrations.Select(registration =>
            {
                Document document = null;

                if (registration.Document != null)
                {
                    var workItemId =
                        _documentService.ExtractWorkItemIdFromOverrideFilePath(registration.Document.OverrideFilePath);
                    if (workItemId != Guid.Empty)
                        document = _documentService.CreateDocumentInFolder(workItemId, registration.Document);
                }

                return new FarmRegistration
                {
                    RegistrationNumber = registration.RegistrationNumber,
                    AuthorityId = registration.AuthorityId,
                    FarmId = farm.Id,
                    TypeId = registration.TypeId,
                    DocumentId = document?.Id
                };
            }));

            farm.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.CreateFarm).Id;
            Context.Farm.Update(farm);

            return farm;
        }


        public FarmOperator UpdateFarmOperator(FarmOperatorRequest data)
        {
            var email = !string.IsNullOrEmpty(data.Email)
                ? new MailAddress(data.Email).Address
                : ""; // validates the email

            var farmOperator = Context.FarmOperator.First(fo => fo.Id == data.Id.ToGuid());


            var oldPhoto = Context.Document.FirstOrDefault(p => p.Id == farmOperator.PhotoId);
            if (oldPhoto != null)
            {
                _documentService.DeleteDocumentFromFolder(oldPhoto.Id);
                Context.Remove(oldPhoto);
                Context.SaveChanges();
            }

            Document photo = null;
            if (data.Photo != null)
            {
                var workItemId = _documentService.ExtractWorkItemIdFromOverrideFilePath(data.Photo.OverrideFilePath);
                if (workItemId != Guid.Empty)
                    photo = _documentService.CreateDocumentInFolder(workItemId, data.Photo);
            }


            farmOperator.Name = data.Name;
            farmOperator.Nationality = data.Nationality;
            farmOperator.TypeId = data.TypeId;
            farmOperator.AddressId = data.AddressId.ToGuid();
            farmOperator.Phone = data.Phone;
            farmOperator.Email = email;
            farmOperator.OriginId = data.OriginId;
            farmOperator.Capital = data.Capital;
            farmOperator.PhotoId = photo?.Id ?? null;

            if (data.TypeId == 1)
            {
                farmOperator.Gender = data.Gender;
                farmOperator.MartialStatus = data.MartialStatus;
                farmOperator.Birthdate = data.Birthdate;
            }
            else
            {
                farmOperator.Gender = null;
                farmOperator.MartialStatus = null;
                farmOperator.Birthdate = null;
            }

            if (data.TypeId == 6)
            {
                farmOperator.Ventures = data.Ventures.ToList().Select(Guid.Parse).ToArray();
            }
            else
            {
                farmOperator.Ventures = new Guid[] { };
            }

            Context.FarmOperator.Update(farmOperator);
            Context.SaveChanges();

            var opRegs = Context.FarmOperatorRegistration.Where(or => or.OperatorId == farmOperator.Id).ToList();
            Context.FarmOperatorRegistration.RemoveRange(opRegs);
            Context.SaveChanges();
            var opRegDocumentIds = opRegs.Select(opReg => opReg.DocumentId).ToList();
            var opRegDocuments = Context.Document.Where(d => opRegDocumentIds.Contains(d.Id)).ToList();
            _documentService.DeleteFileFromFolder(opRegDocuments);
            Context.Document.RemoveRange(opRegDocuments);
            Context.SaveChanges();

            Context.FarmOperatorRegistration.AddRange(data.Registrations.Select(registration =>
            {
                Document document = null;
                if (registration.Document != null)
                {
                    var workItemId =
                        _documentService.ExtractWorkItemIdFromOverrideFilePath(registration.Document.OverrideFilePath);
                    if (workItemId != Guid.Empty)
                        document = _documentService.CreateDocumentInFolder(workItemId, registration.Document);
                }

                return new FarmOperatorRegistration
                {
                    RegistrationNumber = registration.RegistrationNumber,
                    AuthorityId = registration.AuthorityId,
                    OperatorId = farmOperator.Id,
                    TypeId = registration.TypeId,
                    DocumentId = document?.Id
                };
            }));

            Context.SaveChanges(_session.Username, (int)UserActionType.UpdateFarmOperator);

            return farmOperator;
        }

        public Activity UpdateActivity(ActivityPlanRequest data)
        {
            var plan = _projectService.UpdateActivityPlan(data);
            return plan.RootActivity;
        }

        public Farm UpdateFarm(FarmRequest data)
        {
            var farm = Context.Farm.First(f => f.Id == data.Id.ToGuid());

            farm.OperatorId = data.OperatorId.ToGuid();
            farm.TypeId = data.TypeId;
            farm.InvestedCapital = data.InvestedCapital;
            farm.Description = data.Description;
            farm.OtherTypeIds = data.OtherTypeIds;
            farm.Locked = false;
            Context.Farm.Update(farm);
            Context.SaveChanges();

            var farmRegistrations = Context.FarmRegistration.Where(fr => fr.FarmId == farm.Id).ToList();
            Context.FarmRegistration.RemoveRange(farmRegistrations);
            Context.SaveChanges();
            var farmRegistrationDocumentIds = farmRegistrations.Select(fr => fr.DocumentId);
            var farmRegistrationDocuments =
                Context.Document.Where(d => farmRegistrationDocumentIds.Contains(d.Id)).ToList();
            _documentService.DeleteFileFromFolder(farmRegistrationDocuments);
            Context.Document.RemoveRange(farmRegistrationDocuments);
            Context.SaveChanges();

            Context.FarmRegistration.AddRange(data.Registrations.Select(registration =>
            {
                Document document = null;
                if (registration.Document != null)
                {
                    var workItemId =
                        _documentService.ExtractWorkItemIdFromOverrideFilePath(registration.Document.OverrideFilePath);
                    if (workItemId != Guid.Empty)
                        document = _documentService.CreateDocumentInFolder(workItemId, registration.Document);
                }

                return new FarmRegistration
                {
                    RegistrationNumber = registration.RegistrationNumber,
                    AuthorityId = registration.AuthorityId,
                    FarmId = farm.Id,
                    TypeId = registration.TypeId,
                    DocumentId = document?.Id
                };
            }));

            farm.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.UpdateFarm).Id;
            Context.Farm.Update(farm);
            Context.SaveChanges();

            return farm;
        }

        public void AssignFarmLand(Guid farmId, FarmLandRequest farmLandRequest)
        {
            var farm = Context.Farm.First(f => f.Id == farmId);
            farm.Locked = false;
            farm.Status = (int)FarmStatusEnum.Active;
            var certificateDoc = _documentService.CreateDocumentInFolder(Guid.Empty, farmLandRequest.CertificateDoc);
            var leaseContractDoc =
                _documentService.CreateDocumentInFolder(Guid.Empty, farmLandRequest.LeaseContractDoc);

            Context.FarmLand.Add(new FarmLand
            {
                LandId = Guid.Parse(farmLandRequest.LandId),
                CertificateDoc = certificateDoc.Id,
                LeaseContractDoc = leaseContractDoc.Id,
                FarmId = Guid.Parse(farmLandRequest.FarmId),
                SplitIndex = farmLandRequest.SplitIndex,
            });

            farm.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.UpdateFarm).Id;
            Context.Farm.Update(farm);
            Context.SaveChanges();
        }


        public void DeleteFarmOperator(FarmOperatorRequest data)
        {
            var opRegs = Context.FarmOperatorRegistration.Where(or => or.OperatorId == data.Id.ToGuid()).ToList();
            Context.FarmOperatorRegistration.RemoveRange(opRegs);
            Context.SaveChanges();
            var opRegDocumentIds = opRegs.Select(opReg => opReg.DocumentId).ToList();
            var opRegDocuments = Context.Document.Where(d => opRegDocumentIds.Contains(d.Id)).ToList();
            Context.Document.RemoveRange(opRegDocuments);
            Context.SaveChanges();


            Context.FarmOperator.Remove(Context.FarmOperator.First(fo => fo.Id == data.Id.ToGuid()));

            Context.SaveChanges(_session.Username, (int)UserActionType.DeleteFarmOperator);
        }

        public void DeleteFarm(FarmRequest data)
        {
            var farmRegistrations = Context.FarmRegistration.Where(fr => fr.FarmId == data.Id.ToGuid()).ToList();
            Context.FarmRegistration.RemoveRange(farmRegistrations);
            Context.SaveChanges();
            var farmRegistrationDocumentIds = farmRegistrations.Select(fr => fr.DocumentId);
            var farmRegistrationDocuments =
                Context.Document.Where(d => farmRegistrationDocumentIds.Contains(d.Id)).ToList();
            Context.Document.RemoveRange(farmRegistrationDocuments);
            Context.SaveChanges();

            Context.Farm.Remove(Context.Farm.First(fo => fo.Id == data.Id.ToGuid()));
            Context.SaveChanges(_session.Username, (int)UserActionType.DeleteFarm);
        }


        public WorkItemResponse GetLastWorkItem(Guid workflowId)
        {
            var workItem = _workflowService.GetLastWorkItem(workflowId);

            if (workItem.Data != null)
            {
                if (workItem.DataType.Equals("intapscamis.camis.domain.LandBank.LandBankFacadeModel+TransferRequest"))
                {
                    var data = JsonConvert.DeserializeObject<LandBankFacadeModel.TransferRequest>(
                        JsonConvert.SerializeObject(workItem.Data));
                    workItem.Data = data;
                }
                else
                {
                    var data = JsonConvert.DeserializeObject<FarmRequest>(
                        JsonConvert.SerializeObject(workItem.Data));

                    if (data.Operator.Photo != null)
                    {
                        data.Operator.Photo.File = Convert.ToBase64String(_documentService
                            .ParseDocumentFromFolder(workItem.Id, data.Operator.Photo).File);
                    }

                    // (FarmRequest data).Registrations[i].Document 
                    if (data?.Registrations != null)
                        foreach (var reg in data.Registrations)
                            if (reg.Document != null)
                                reg.Document.File = null;

                    // (FarmRequest data).Operator.Registrations[i].Document 
                    if (data?.Operator?.Registrations != null)
                        foreach (var reg in data.Operator.Registrations)
                            if (reg.Document != null)
                                reg.Document.File = null;

                    // (FarmRequest data).ActivityPlan.Documents 
                    if (data?.ActivityPlan?.Documents != null)
                        foreach (var doc in data.ActivityPlan.Documents)
                            if (doc != null)
                                doc.File = null;

                    workItem.Data = data;
                }
            }

            return workItem;
        }

        public Document InWorkItemRegistrationFile(Guid workItemId, int regId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<FarmRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";

            var documentRequest = data?.Registrations?.First(d => d.Id == regId)?.Document;
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(data?.Registrations?.First(d => d.Id == regId)?.Document);
        }

        public Document InWorkItemOperatorRegistrationFile(Guid workItemId, int regId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<FarmRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var documentRequest = data?.Operator?.Registrations?.First(d => d.Id == regId)?.Document;
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemActivityPlanFile(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<FarmRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var documentRequest = data?.ActivityPlan?.Documents?.First(d => d.Id == documentId);
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemOperatorPhoto(Guid workItemId, Guid photoId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<FarmRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var documentRequest = data?.Operator?.Photo;
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemActivityPlanFileForPlanUpdate(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<ActivityPlanRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var documentRequest = data?.Documents?.First(d => d.Id == documentId);
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemContractCancellationDoc(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<ContractCancellationRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var documentRequest = data?.CancellationSupDoc?.First(d => d.Id == documentId);
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemContractUpdateDoc(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<ContractRenewalRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var documentRequest = data?.SupportiveDocument?.First(d => d.DocumentId == documentId.ToString()).Document;
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemContractRenewalDoc(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<ContractModificationRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var documentRequest = data?.SupportiveDocument?.First(d => d.Id == documentId);
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemContractWarningDoc(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<ContractWarningRequest>(dataStr);
            var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                                "/usr/bin/CAMIS/data/docs";
            var documentRequest = data?.SupportiveDocument?.First(d => d.Id == documentId);
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                var doc = _documentService.GetDocument(documentRequest.Id);
                if (doc != null && doc.File == null)
                {
                    var filePath = $"{doc.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        doc.File = File.ReadAllBytes(filePath);
                    }

                    return doc;
                }
                else
                {
                    var filePath = $"{documentRequest.Id}";
                    if (!Path.IsPathRooted(filePath))
                    {
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            workItemId.ToString(), Path.GetFileName(filePath));
                    }

                    if (File.Exists(filePath))
                    {
                        var fileBytes = File.ReadAllBytes(filePath);
                        documentRequest.File = Convert.ToBase64String(fileBytes);
                    }

                    return DocumentService.ParseDocument(documentRequest);
                }
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public FarmResponse GetFarmByLandId(Guid id)
        {
            var farmland = Context.FarmLand.FirstOrDefault(l => l.LandId == id);
            if (farmland != null)
            {
                return GetFarm(farmland.FarmId);
            }

            return null;
        }

        public LandRightsResponse GetLandRightsByLandIdandFarmId(Guid landId, Guid farmId, int? partId = 0)
        {
            var lr = Context.LandRight.FirstOrDefault(l =>
                l.LandId == landId && l.FarmId == farmId && l.SplitIndex == partId);
            if (lr != null)
            {
                var s = Context.FarmStatusTypes.First(e => e.Id == lr.Status);
                return new LandRightsResponse
                {
                    LandId = lr.LandId.ToString(),
                    FarmId = lr.FarmId.ToString(),
                    RightFrom = new DateTime(lr.RightFrom ?? 0),
                    RightTo = new DateTime(lr.RightTo ?? 0),
                    RightType = lr.RightType,
                    SplitIndex = lr.SplitIndex,
                    LandSectionArea = lr.LandSectionArea,
                    Geom = GetWktFromGeom(lr.Geom),
                    Status = new FarmStatus { Id = s.Id, Name = s.Name }
                };
            }

            return null;
        }

        public List<LandRightsResponse> GetAllLandRightsByLandId(Guid landId)
        {
            var rights = new List<LandRightsResponse>();
            var rightLists = Context.LandRight.Where(i => i.LandId == landId).ToList();
            foreach (var lr in rightLists)
            {
                var r = GetLandRightsByLandIdandFarmId(lr.LandId, lr.FarmId, lr.SplitIndex);
                if (r != null)
                    rights.Add(r);
            }

            return rights;
        }

        public List<LandRightsResponse> GetAllLandRightsByFarmId(Guid farmId)
        {
            var rights = new List<LandRightsResponse>();
            var rightLists = Context.LandRight.Where(i => i.FarmId == farmId).ToList();
            foreach (var lr in rightLists)
            {
                var r = GetLandRightsByLandIdandFarmId(lr.LandId, lr.FarmId, lr.SplitIndex);
                if (r != null)
                    rights.Add(r);
            }

            return rights;
        }

        public void SetFarmStatus(Guid farmId, int status)
        {
            var l = Context.Farm.Where(x => x.Id == farmId).First();
            l.Status = (int)status;
            Context.Farm.Update(l);
            Context.SaveChanges();
        }

        public void SetFarmLock(Guid farmId, bool val)
        {
            var l = Context.Farm.Where(x => x.Id == farmId).First();
            l.Locked = val;
            Context.Farm.Update(l);
            Context.SaveChanges();
        }

        public List<Workflow> GetTransferWorkFlows()
        {
            return Context.Workflow.Where(x => x.TypeId == 10 && x.CurrentState == 3).ToList();
        }

        public List<FarmLandResponse2> GetFarmLands(Guid farmId)
        {
            var ret = new List<FarmLandResponse2>();
            var landRights = Context.LandRight.Where(l => l.FarmId == farmId).ToList();

            foreach (var fl in landRights)
            {
                var l = new FarmLandResponse2();
                var land = Context.LandUpin.First(x => x.LandId == fl.LandId);
                var farmland = Context.FarmLand.FirstOrDefault(x =>
                    x.LandId == fl.LandId && x.FarmId == farmId && x.SplitIndex == fl.SplitIndex);
                l.Area = land.Area ?? 0;
                l.Geom = GetWktFromGeom(land.Geometry);
                l.CentroidX = land.CentroidX ?? 0;
                l.CentroidY = land.CentroidY ?? 0;
                l.Upin = land.Upin;
                if (farmland != null)
                {
                    fl.CertificateDocument = farmland.CertificateDoc;
                    fl.ContractDocument = farmland.LeaseContractDoc;
                }

                if (fl.SplitIndex > 0)
                {
                    var sp = Context.LandSplit.First(x => x.LandId == fl.LandId && x.Id == fl.SplitIndex);
                    l.Area = sp.Area;
                    l.Upin = land.Upin + "-" + sp.Indexes;
                    l.Geom = GetWktFromGeom(sp.Geom);
                }

                l.SplitIndex = fl.SplitIndex;
                l.LandId = fl.LandId.ToString();
                l.FarmId = fl.FarmId.ToString();
                l.Rights = MapLandRight(fl);

                ret.Add(l);
            }

            return ret;
        }

        public void CancelContract(ContractCancellationRequest request)
        {
            var farm = Context.Farm.First(x => x.Id == Guid.Parse(request.Id));
            var land = new List<Land>();
            var landSplit = new List<LandSplit>();
            var landRight = new List<LandRight>();
            var farmLand = new List<FarmLand>();

            var sourceTxtUid = Guid.NewGuid();
            farm.Locked = false;
            var cancellations = new List<CancelledContract>();
            if (request.CancelledRight.Count == request.FarmLands.Count)
            {
                farm.Status = (int)FarmStatusEnum.Suspended;
            }

            foreach (var cr in request.CancelledRight)
            {
                farmLand.Add(Context.FarmLand.First(x =>
                    x.LandId == Guid.Parse(cr.LandId) && x.FarmId == Guid.Parse(cr.FarmId) &&
                    x.SplitIndex == cr.SplitIndex));
                var r = Context.LandRight.First(x =>
                    x.LandId == Guid.Parse(cr.LandId) && x.FarmId == Guid.Parse(cr.FarmId) &&
                    x.SplitIndex == cr.SplitIndex);
                landRight.Add(r);
                var l = Context.Land.First(x => x.Id == Guid.Parse(cr.LandId));
                if (cr.SplitIndex > 0)
                {
                    var sp = Context.LandSplit.First(x => x.LandId == Guid.Parse(cr.LandId) && x.Id == cr.SplitIndex);
                    sp.Locked = false;
                    sp.Status = (int)LandBankFacadeModel.LandTypeEnum.Prepared;
                    l.LandType = (int)LandBankFacadeModel.LandTypeEnum.PreparedWithSplit;
                    landSplit.Add(sp);
                }
                else
                {
                    l.LandType = (int)LandBankFacadeModel.LandTypeEnum.Prepared;
                    l.Locked = false;
                }

                land.Add(l);
                cancellations.Add(new CancelledContract
                {
                    Id = Guid.NewGuid(),
                    Date = request.Date.Ticks,
                    Reason = request.Reason,
                    ReasonDetails = request.CancellationReason,
                    FarmId = Guid.Parse(request.Id),
                    LandId = Guid.Parse(cr.LandId),
                    SplitIndex = cr.SplitIndex,
                    SourceTxtUid = sourceTxtUid,
                    Wfid = Guid.Parse(request.wfid)
                });
            }

            Context.FarmLand.RemoveRange(farmLand);
            Context.LandRight.RemoveRange(landRight);
            Context.Land.UpdateRange(land);
            Context.LandSplit.UpdateRange(landSplit);
            Context.Farm.Update(farm);
            Context.SaveChanges();
            Context.CancelledContracts.AddRange(cancellations);
            Context.CancelledContractDocs.AddRange(request.CancellationSupDoc.Select(doc =>
            {
                Document document = null;

                if (doc != null)
                {
                    var workItemId =
                        _documentService.ExtractWorkItemIdFromOverrideFilePath(doc.OverrideFilePath);
                    if (workItemId != Guid.Empty)
                        document = _documentService.CreateDocumentInFolder(workItemId, doc);
                    return new CancelledContractDoc()
                    {
                        Id = Guid.NewGuid(),
                        CancellationId = cancellations.First(e => e.FarmId == Guid.Parse(request.Id)).Id,
                        DocId = document.Id
                    };
                }

                return null;
            }));

            var aid = Context.SaveChanges(_session.Username, (int)UserActionType.CancelContract).Id;
            Context.CancelledContracts.UpdateRange(cancellations.Select(c =>
            {
                c.Aid = aid;
                return c;
            }));
        }

        public object GetAllModificationReasonList()
        {
            return Context.ContractUpdateReasons.Select(c => new { id = c.Id, name = c.Name }).ToList();
        }

        public void UpdateContract(ContractModificationRequest request)
        {
            var farm = Context.Farm.First(f => f.Id == Guid.Parse(request.FarmId));
            var modifiedRight = new List<LandRight>();
            foreach (var mr in request.ModifiedRight)
            {
                var lr = Context.LandRight.FirstOrDefault(e =>
                    e.LandId == Guid.Parse(mr.LandId) && e.FarmId == Guid.Parse(mr.FarmId) &&
                    e.SplitIndex == mr.SplitIndex);
                if (lr != null)
                {
                    lr.RightFrom = mr.RightFrom.Ticks;
                    lr.RightTo = mr.RightTo.Ticks;
                    lr.YearlyRent = mr.YearlyRent;
                    lr.Status = (int)FarmStatusEnum.Active;
                    modifiedRight.Add(lr);
                }
            }

            Context.LandRight.UpdateRange(modifiedRight);
            Context.SaveChanges();
            farm.Locked = false;
            farm.Status = (int)FarmStatusEnum.Active;
            Context.Farm.Update(farm);
            Context.SaveChanges(_session.Username, (int)UserActionType.UpdateContract);
        }

        public void RegisterContractWarning(ContractWarningRequest request)
        {
            var right = Context.LandRight.FirstOrDefault(r =>
                r.FarmId == Guid.Parse(request.FarmId) && r.LandId == Guid.Parse(request.LandId) &&
                r.SplitIndex == request.SplitIndex);
            if (right != null)
            {
                right.Status = (int)FarmStatusEnum.UnderWarning;
                Context.LandRight.Update(right);
                Context.SaveChanges();
            }

            var warning = new FarmWarning()
            {
                Id = Guid.NewGuid(),
                FarmId = request.FarmId.ToGuid(),
                LandId = request.LandId.ToGuid(),
                SplitIndex = request.SplitIndex,
                Date = request.Date.Ticks,
                Reason = request.Reason,
                ReasonDetails = request.Description,
                Stage = request.Stage,
                Wfid = request.wfid.ToGuid()
            };
            Context.FarmWarnings.Add(warning);
            Context.WarningDocs.AddRange(request.SupportiveDocument.Select(doc =>
            {
                Document document = null;

                if (doc != null)
                {
                    var workItemId =
                        _documentService.ExtractWorkItemIdFromOverrideFilePath(doc.OverrideFilePath);
                    if (workItemId != Guid.Empty)
                        document = _documentService.CreateDocumentInFolder(workItemId, doc);
                    return new WarningDoc
                    {
                        Id = Guid.NewGuid(),
                        WarningId = warning.Id,
                        DocId = document.Id
                    };
                }

                return null;
            }));

            warning.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.ContractWarning).Id;
            Context.FarmWarnings.Update(warning);
        }

        public FarmWarningResponse GetRightWarning(Guid farmId, Guid landId, int splitIndex)
        {
            var right = Context.LandRight.FirstOrDefault(e =>
                e.FarmId == farmId && e.LandId == landId && e.SplitIndex == splitIndex);
            if (right == null)
                return null;
            var warnings = Context.FarmWarnings
                .Where(e => e.FarmId == farmId && e.LandId == landId && e.SplitIndex == splitIndex)
                .OrderByDescending(e => e.Date).ToList();
            var parseWarnings = new List<WarningResponse>();
            foreach (var wr in warnings)
            {
                parseWarnings.Add(ParseWaringResponse(wr.Id));
            }

            return new FarmWarningResponse
            {
                LandRight = MapLandRight(right),
                Warnings = parseWarnings
            };
        }

        public void RenewContract(ContractRenewalRequest request)
        {
            var right = Context.LandRight.FirstOrDefault(r =>
                r.FarmId == Guid.Parse(request.FarmId) && r.LandId == Guid.Parse(request.LandId) &&
                r.SplitIndex == request.SplitIndex);
            if (right != null)
            {
                right.Status = (int)FarmStatusEnum.Active;
                Context.LandRight.Update(right);
                Context.SaveChanges();
            }

            var renewal = new RenewContract()
            {
                Id = Guid.NewGuid(),
                FarmId = request.FarmId.ToGuid(),
                LandId = request.LandId.ToGuid(),
                SplitIndex = request.SplitIndex,
                Date = request.Date.Ticks,
                BudgetYear = request.BudgetYear,
                Remark = request.Remark,
                Wfid = request.wfid.ToGuid()
            };
            Context.RenewContracts.Add(renewal);
            Context.RenewContractDocs.AddRange(request.SupportiveDocument.Select(doc =>
            {
                Document document = null;

                if (doc?.Document != null)
                {
                    var workItemId =
                        _documentService.ExtractWorkItemIdFromOverrideFilePath(doc.Document.OverrideFilePath);
                    if (workItemId != Guid.Empty)
                        document = _documentService.CreateDocumentInFolder(workItemId, doc.Document);
                    return new RenewContractDoc()
                    {
                        Id = Guid.NewGuid(),
                        RenewId = renewal.Id,
                        AuthorityId = doc.AuthorityId,
                        TypeId = doc.TypeId,
                        DocId = document.Id
                    };
                }

                return null;
            }));

            renewal.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.RenewContract).Id;
            Context.RenewContracts.Update(renewal);
        }

        private FarmResponse ParseFarmResponse(Farm farm)
        {
            var res = new FarmResponse
            {
                Id = farm.Id,
                OperatorId = farm.OperatorId,
                TypeId = farm.TypeId,
                ActivityId = farm.ActivityId,
                InvestedCapital = farm.InvestedCapital,
                Description = farm.Description,
                OtherTypeIds = farm.OtherTypeIds,

                FarmLands = Context.FarmLand.Where(fl => fl.FarmId == farm.Id).AsEnumerable()
                    .Select(MapFarmLandToResponse).ToList()
            };

            var farmRegistrations = Context.FarmRegistration.Where(fr => fr.FarmId == farm.Id);
            res.Registrations = new List<FarmRegistrationResponse>();
            foreach (var farmRegistration in farmRegistrations)
            {
                var registrationAuthority =
                    Context.RegistrationAuthority.First(ra => ra.Id == farmRegistration.AuthorityId);
                var registrationType = Context.RegistrationType.First(rt => rt.Id == farmRegistration.TypeId);
                var document = _documentService.GetDocument(farmRegistration.DocumentId);
                res.Registrations.Add(new FarmRegistrationResponse
                {
                    Id = farmRegistration.Id,
                    RegistrationNumber = farmRegistration.RegistrationNumber,
                    AuthorityId = farmRegistration.AuthorityId,
                    TypeId = farmRegistration.TypeId,
                    DocumentId = farmRegistration.DocumentId?.ToString(),

                    Authority = new RegistrationAuthorityResponse
                    {
                        Id = registrationAuthority.Id,
                        Name = registrationAuthority.Name
                    },
                    Type = new RegistrationTypeResponse
                    {
                        Id = registrationType.Id,
                        Name = registrationType.Name
                    },
                    Document = DocumentService.ParseDocumentResponse(document)
                });
            }

            var farmOperator = Context.FarmOperator.First(op => op.Id == farm.OperatorId);
            res.Operator = ParseFarmOperatorResponse(farmOperator);

            var farmType = Context.FarmType.First(ft => ft.Id == farm.TypeId);
            res.Type = new FarmTypeResponse
            {
                Id = farmType.Id,
                Name = farmType.Name
            };

            return res;
        }

        private FarmLandResponse ParseFarmLandResponse(FarmLand farmLand)
        {
            var certificateDoc = Context.Document.FirstOrDefault(d => d.Id == farmLand.CertificateDoc);
            var leaseContractDoc = Context.Document.FirstOrDefault(d => d.Id == farmLand.LeaseContractDoc);

            return new FarmLandResponse
            {
                LandId = farmLand.LandId,
                CertificateDocId = farmLand.CertificateDoc,
                LeaseContractDocId = farmLand.LeaseContractDoc,
                FarmId = farmLand.FarmId,

                CertificateDoc = DocumentService.ParseDocumentResponse(certificateDoc),
                LeaseContractDoc = DocumentService.ParseDocumentResponse(leaseContractDoc)
            };
        }

        private FarmOperatorResponse ParseFarmOperatorResponse(FarmOperator farmOperator)
        {
            var res = new FarmOperatorResponse
            {
                Id = farmOperator.Id,
                Name = farmOperator.Name,
                Nationality = farmOperator.Nationality,
                TypeId = farmOperator.TypeId,
                AddressId = farmOperator.AddressId,
                Phone = farmOperator.Phone,
                Email = farmOperator.Email,
                OriginId = farmOperator.OriginId,
                Capital = farmOperator.Capital,
                PhotoId = farmOperator.PhotoId,
                Photo = DocumentService.ParseDocumentResponse(_documentService.GetDocument(farmOperator.PhotoId))
            };

            switch (farmOperator.TypeId)
            {
                case 1:
                    res.Gender = farmOperator.Gender;
                    res.MartialStatus = farmOperator.MartialStatus;
                    res.Birthdate = farmOperator.Birthdate;
                    break;
                case 6:
                    res.Ventures = farmOperator.Ventures;
                    break;
            }

            var operatorRegistrations = Context.FarmOperatorRegistration.Where(or => or.OperatorId == farmOperator.Id);
            res.Registrations = new List<FarmOperatorRegistrationResponse>();
            foreach (var operatorRegistration in operatorRegistrations)
            {
                var registrationAuthority =
                    Context.RegistrationAuthority.First(ra => ra.Id == operatorRegistration.AuthorityId);
                var registrationType = Context.RegistrationType.First(rt => rt.Id == operatorRegistration.TypeId);
                var document = _documentService.GetDocument(operatorRegistration.DocumentId);
                res.Registrations.Add(new FarmOperatorRegistrationResponse
                {
                    Id = operatorRegistration.Id,
                    RegistrationNumber = operatorRegistration.RegistrationNumber,
                    AuthorityId = operatorRegistration.AuthorityId,
                    TypeId = operatorRegistration.TypeId,
                    DocumentId = operatorRegistration.DocumentId?.ToString(),

                    Authority = new RegistrationAuthorityResponse
                    {
                        Id = registrationAuthority.Id,
                        Name = registrationAuthority.Name
                    },
                    Type = new RegistrationTypeResponse
                    {
                        Id = registrationType.Id,
                        Name = registrationType.Name
                    },
                    Document = DocumentService.ParseDocumentResponse(document)
                });
            }

            var operatorType = Context.FarmOperatorType.Find(farmOperator.TypeId);
            res.Type = new FarmOperatorTypeResponse
            {
                Id = operatorType.Id,
                Name = operatorType.Name
            };

            var operatorOrigin = Context.FarmOperatorOrigin.Find(farmOperator.OriginId);
            res.Origin = new FarmOperatorOriginResponse
            {
                Id = operatorOrigin.Id,
                Name = operatorOrigin.Name
            };

            return res;
        }

        private FarmResponse MapFarmToResponse(Farm farm)
        {
            var ret = new FarmResponse
            {
                Id = farm.Id,
                OperatorId = farm.OperatorId,
                TypeId = farm.TypeId,
                ActivityId = farm.ActivityId,
                InvestedCapital = farm.InvestedCapital,
                Description = farm.Description,
                OtherTypeIds = farm.OtherTypeIds,
                Locked = farm.Locked,

                FarmLands = farm.FarmLand?
                    .Select(MapFarmLandToResponse)
                    .ToList() ?? new List<FarmLandResponse>(),

                Registrations = farm.FarmRegistration?
                    .Select(MapFarmRegistrationToResponse)
                    .ToList() ?? new List<FarmRegistrationResponse>(),

                Operator = MapFarmOperatorToResponse(farm.Operator),

                Type = farm.Type == null
                    ? null
                    : new FarmTypeResponse
                    {
                        Id = farm.Type.Id,
                        Name = farm.Type.Name
                    },
                Status = farm.StatusNavigation == null
                    ? null
                    : new FarmStatus
                    {
                        Id = farm.StatusNavigation.Id,
                        Name = farm.StatusNavigation.Name,
                    }
            };
            if (ret.Locked)
                ret.Status = new FarmStatus
                {
                    Id = 3,
                    Name = "Transaction Locked"
                };

            return ret;
        }

        private FarmLandResponse MapFarmLandToResponse(FarmLand land)
        {
            return new FarmLandResponse
            {
                LandId = land.LandId,
                CertificateDocId = land.CertificateDoc,
                LeaseContractDocId = land.LeaseContractDoc,
                FarmId = land.FarmId,
                CertificateDoc = MapDocumentResponse(_documentService.GetDocument(land.CertificateDoc)),
                LeaseContractDoc = MapDocumentResponse(_documentService.GetDocument(land.LeaseContractDoc)),
                SplitIndex = land.SplitIndex
            };
        }

        private string GetWktFromGeom(Geometry? geom)
        {
            var writer = new WKTWriter();
            if (geom != null)
                return writer.Write(geom);
            return null;
        }

        private FarmRegistrationResponse MapFarmRegistrationToResponse(FarmRegistration reg)
        {
            var document = MapDocumentResponse(_documentService.GetDocument(reg.DocumentId));
            return new FarmRegistrationResponse
            {
                Id = reg.Id,
                RegistrationNumber = reg.RegistrationNumber,
                AuthorityId = reg.AuthorityId,
                TypeId = reg.TypeId,
                DocumentId = reg.DocumentId?.ToString(),

                Authority = reg.Authority == null
                    ? null
                    : new RegistrationAuthorityResponse
                    {
                        Id = reg.Authority.Id,
                        Name = reg.Authority.Name
                    },

                Type = reg.Type == null
                    ? null
                    : new RegistrationTypeResponse
                    {
                        Id = reg.Type.Id,
                        Name = reg.Type.Name
                    },

                Document = reg.Document == null ? document : MapDocumentResponse(reg.Document)
            };
        }

        private FarmOperatorResponse MapFarmOperatorToResponse(FarmOperator op)
        {
            if (op == null) return null;


            var response = new FarmOperatorResponse
            {
                Id = op.Id,
                Name = op.Name,
                Nationality = op.Nationality,
                TypeId = op.TypeId,
                AddressId = op.AddressId,
                Phone = op.Phone,
                Email = op.Email,
                OriginId = op.OriginId,
                Capital = op.Capital,
                PhotoId = op.PhotoId,

                Type = op.Type == null
                    ? null
                    : new FarmOperatorTypeResponse
                    {
                        Id = op.Type.Id,
                        Name = op.Type.Name
                    },

                Origin = op.Origin == null
                    ? null
                    : new FarmOperatorOriginResponse
                    {
                        Id = op.Origin.Id,
                        Name = op.Origin.Name
                    },

                Registrations = op.FarmOperatorRegistration?
                    .Select(MapOperatorRegistrationToResponse)
                    .ToList() ?? new List<FarmOperatorRegistrationResponse>()
            };

            // Handle type-specific fields
            if (op.TypeId == 1) // Individual
            {
                response.Gender = op.Gender;
                response.MartialStatus = op.MartialStatus;
                response.Birthdate = op.Birthdate;
            }
            else if (op.TypeId == 6) // Corporation
            {
                response.Ventures = op.Ventures;
            }

            if (op.PhotoId != null)
            {
                response.Photo = DocumentService.ParseOperatorPhotoResponse(_documentService.GetDocument(op.PhotoId));
            }

            return response;
        }

        private FarmOperatorRegistrationResponse MapOperatorRegistrationToResponse(FarmOperatorRegistration reg)
        {
            var document = MapDocumentResponse(_documentService.GetDocument(reg.DocumentId));
            return new FarmOperatorRegistrationResponse
            {
                Id = reg.Id,
                RegistrationNumber = reg.RegistrationNumber,
                AuthorityId = reg.AuthorityId,
                TypeId = reg.TypeId,
                DocumentId = reg.DocumentId?.ToString(),

                Authority = reg.Authority == null
                    ? null
                    : new RegistrationAuthorityResponse
                    {
                        Id = reg.Authority.Id,
                        Name = reg.Authority.Name
                    },

                Type = reg.Type == null
                    ? null
                    : new RegistrationTypeResponse
                    {
                        Id = reg.Type.Id,
                        Name = reg.Type.Name
                    },

                Document = reg.Document == null ? document : MapDocumentResponse(reg.Document)
            };
        }

        private DocumentResponse MapDocumentResponse(Document doc)
        {
            if (doc == null) return null;
            return DocumentService.ParseDocumentResponse(doc);
        }

        private FarmResponse MapSingleFarmData(Guid id)
        {
            var baseQuery = Context.Farm
                .Include(f => f.Type)
                .Include(f => f.Activity)
                .Include(f => f.Operator)
                .ThenInclude(o => o.Type)
                .Include(f => f.Operator)
                .ThenInclude(o => o.Origin)
                .Include(f => f.Operator)
                .ThenInclude(o => o.FarmOperatorRegistration)
                .ThenInclude(or => or.Authority)
                .Include(f => f.Operator)
                .ThenInclude(o => o.FarmOperatorRegistration)
                .ThenInclude(or => or.Type)
                .Include(f => f.FarmLand)
                .Include(f => f.FarmRegistration)
                .ThenInclude(fr => fr.Authority)
                .Include(f => f.FarmRegistration)
                .ThenInclude(fr => fr.Type)
                .Include(f => f.StatusNavigation)
                .Where(f => f.Id == id)
                .AsSplitQuery()
                .AsNoTracking();
            var farm = baseQuery.FirstOrDefault(e => e.Id == id);
            return MapFarmToResponse(farm);
        }

        private LandBankFacadeModel.LandRightResponse MapLandRight(LandRight lr)
        {
            var cd = Context.Document.FirstOrDefault(c => c.Id == lr.CertificateDocument);
            var cdDoc = new DocumentResponse();
            cdDoc = null;
            if (cd != null)
            {
                cdDoc = new DocumentResponse()
                {
                    Id = cd.Id,
                    Date = cd.Date,
                    Filename = cd.Filename,
                    Mimetype = cd.Mimetype,
                    OverrideFilePath = cd.OverrideFilePath,
                    Ref = cd.Ref,
                    Type = cd.Type,
                    Note = cd.Note
                };
            }

            var cod = Context.Document.FirstOrDefault(c => c.Id == lr.ContractDocument);
            var codDoc = new DocumentResponse();
            codDoc = null;
            if (cod != null)
            {
                codDoc = new DocumentResponse()
                {
                    Id = cod.Id,
                    Date = cod.Date,
                    Filename = cod.Filename,
                    Mimetype = cod.Mimetype,
                    OverrideFilePath = cod.OverrideFilePath,
                    Ref = cod.Ref,
                    Type = cod.Type,
                    Note = cod.Note
                };
            }

            return new LandBankFacadeModel.LandRightResponse()
            {
                LandId = lr.LandId.ToString(),
                RightFrom = new DateTime(lr.RightFrom ?? 0),
                RightTo = new DateTime(lr.RightTo ?? 0),
                RightType = lr.RightType,
                LandSectionArea = lr.LandSectionArea,
                Status = lr.Status,
                CommonTxtUid = lr.CommonTxtUid.ToString(),
                YearlyRent = lr.YearlyRent,
                Geom = GetWktFromGeom(lr.Geom),
                CertificateDocument = cdDoc,
                ContractDocument = codDoc,
            };
        }

        private WarningResponse ParseWaringResponse(Guid warningId)
        {
            var ret = new WarningResponse();
            var warning = Context.FarmWarnings.FirstOrDefault(w => w.Id == warningId);
            if (warning == null)
                return null;
            var docs = new List<DocumentResponse>();
            var supDoc = Context.WarningDocs.Where(e => e.WarningId == warningId).ToList();
            foreach (var wd in supDoc)
            {
                docs.Add(MapDocumentResponse(_documentService.GetDocument(wd.DocId)));
            }

            return new WarningResponse()
            {
                Id = warning.Id.ToString(),
                FarmId = warning.FarmId.ToString(),
                LandId = warning.LandId.ToString(),
                SplitIndex = warning.SplitIndex,
                Date = new DateTime(warning.Date),
                Stage = warning.Stage,
                Reason = warning.Reason,
                ReasonDetails = warning.ReasonDetails,
                Wfid = warning.Wfid,
                Aid = warning.Aid,
                SupportiveDocuments = docs
            };
        }
    }
}