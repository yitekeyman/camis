using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Documents;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Projects;
using intapscamis.camis.domain.Projects.Models;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Microsoft.EntityFrameworkCore;
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
        PaginatorResponse<FarmResponse> SearchFarms(string term, int skip, int take);

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
            return Context.LandUpin.Where(upin => upin.Land.LandType == 2).Select(upin => upin.Upin).ToList();
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

        public PaginatorResponse<FarmResponse> SearchFarms(string term, int skip, int take)
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
                Capital = data.Capital
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
                if (registration.Document != null) document = _documentService.CreateDocument(registration.Document);

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
                OtherTypeIds = data.OtherTypeIds
            };

            Context.Farm.Add(farm);

            Context.FarmRegistration.AddRange(data.Registrations.Select(registration =>
            {
                Document document = null;
                if (registration.Document != null) document = _documentService.CreateDocument(registration.Document);

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

            farmOperator.Name = data.Name;
            farmOperator.Nationality = data.Nationality;
            farmOperator.TypeId = data.TypeId;
            farmOperator.AddressId = data.AddressId.ToGuid();
            farmOperator.Phone = data.Phone;
            farmOperator.Email = email;
            farmOperator.OriginId = data.OriginId;
            farmOperator.Capital = data.Capital;

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
            Context.Document.RemoveRange(opRegDocuments);
            Context.SaveChanges();

            Context.FarmOperatorRegistration.AddRange(data.Registrations.Select(registration =>
            {
                Document document = null;
                if (registration.Document != null) document = _documentService.CreateDocument(registration.Document);

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

            Context.Farm.Update(farm);
            Context.SaveChanges();

            var farmRegistrations = Context.FarmRegistration.Where(fr => fr.FarmId == farm.Id).ToList();
            Context.FarmRegistration.RemoveRange(farmRegistrations);
            Context.SaveChanges();
            var farmRegistrationDocumentIds = farmRegistrations.Select(fr => fr.DocumentId);
            var farmRegistrationDocuments =
                Context.Document.Where(d => farmRegistrationDocumentIds.Contains(d.Id)).ToList();
            Context.Document.RemoveRange(farmRegistrationDocuments);
            Context.SaveChanges();

            Context.FarmRegistration.AddRange(data.Registrations.Select(registration =>
            {
                Document document = null;
                if (registration.Document != null) document = _documentService.CreateDocument(registration.Document);

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

            var certificateDoc = _documentService.CreateDocument(farmLandRequest.CertificateDoc);
            var leaseContractDoc = _documentService.CreateDocument(farmLandRequest.LeaseContractDoc);

            Context.FarmLand.Add(new FarmLand
            {
                LandId = farmLandRequest.LandId,
                CertificateDoc = certificateDoc.Id,
                LeaseContractDoc = leaseContractDoc.Id,
                FarmId = farmLandRequest.FarmId
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
                var data = JsonConvert.DeserializeObject<FarmRequest>(
                    JsonConvert.SerializeObject(workItem.Data));

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

            return workItem;
        }

        public Document InWorkItemRegistrationFile(Guid workItemId, int regId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<FarmRequest>(dataStr);

            var documentRequest = data?.Registrations?.First(d => d.Id == regId)?.Document;
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                return _documentService.GetDocument(documentRequest.Id);
            }

            return DocumentService.ParseDocument(data?.Registrations?.First(d => d.Id == regId)?.Document);
        }

        public Document InWorkItemOperatorRegistrationFile(Guid workItemId, int regId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<FarmRequest>(dataStr);

            var documentRequest = data?.Operator?.Registrations?.First(d => d.Id == regId)?.Document;
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                return _documentService.GetDocument(documentRequest.Id);
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemActivityPlanFile(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<FarmRequest>(dataStr);

            var documentRequest = data?.ActivityPlan?.Documents?.First(d => d.Id == documentId);
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                return _documentService.GetDocument(documentRequest.Id);
            }

            return DocumentService.ParseDocument(documentRequest);
        }

        public Document InWorkItemActivityPlanFileForPlanUpdate(Guid workItemId, Guid documentId)
        {
            var dataStr = Context.WorkItem.Find(workItemId).Data;
            if (dataStr == null) return null;
            var data = JsonConvert.DeserializeObject<ActivityPlanRequest>(dataStr);

            var documentRequest = data?.Documents?.First(d => d.Id == documentId);
            if (documentRequest?.Id != null && documentRequest.File == null)
            {
                return _documentService.GetDocument(documentRequest.Id);
            }

            return DocumentService.ParseDocument(documentRequest);
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
                Capital = farmOperator.Capital
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
                    }
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
            };
        }

        private FarmRegistrationResponse MapFarmRegistrationToResponse(FarmRegistration reg)
        {
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

                Document = MapDocumentResponse(reg.Document)
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

            return response;
        }

        private FarmOperatorRegistrationResponse MapOperatorRegistrationToResponse(FarmOperatorRegistration reg)
        {
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

                Document = MapDocumentResponse(reg.Document)
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
                .Where(f => f.Id==id)
                .AsSplitQuery()
                .AsNoTracking();
            var farm = baseQuery.FirstOrDefault(e=>e.Id==id);
            return MapFarmToResponse(farm);
        }
    }
}