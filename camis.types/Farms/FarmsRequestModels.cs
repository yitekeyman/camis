using System;
using System.Collections.Generic;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Projects.Models;
using NetTopologySuite.Geometries;

namespace intapscamis.camis.domain.Farms.Models
{
    public class FarmRequest
    {
        public string Id { get; set; } // read-only
        public string OperatorId { get; set; } // if it is an existing operator
        public int TypeId { get; set; }
        public string ActivityId { get; set; } // not needed from client, read-only
        public double? InvestedCapital { get; set; }
        public string Description { get; set; }
        public int[] OtherTypeIds { get; set; } = { };

        public ICollection<FarmRegistrationRequest> Registrations { get; set; }=new List<FarmRegistrationRequest>();

        public FarmOperatorRequest Operator { get; set; } // if no OperatorId
        public ActivityPlanRequest ActivityPlan { get; set; } // only for registration

        // only for land transfer
        public LandBankFacadeModel.TransferRequest LandTransferRequest { get; set; }=new LandBankFacadeModel.TransferRequest();
        public Guid LandTransferWorkflowId { get; set; }

        public ICollection<FarmLandRequest> FarmLands { get; set; }=new List<FarmLandRequest>();
        public FarmStatus Status { get; set; } = new FarmStatus();
        public bool Locked { get; set; }
    }

    public class ContractCancellationRequest
    {
        public string Id { get; set; } // read-only
        public string OperatorId { get; set; } // if it is an existing operator
        public int TypeId { get; set; }
        public string ActivityId { get; set; } // not needed from client, read-only
        public double? InvestedCapital { get; set; }
        public string Description { get; set; }
        public int[] OtherTypeIds { get; set; } = { };
        public IList<CancelledRightRequest> CancelledRight { get; set; }=new List<CancelledRightRequest>();
        public IList<DocumentRequest> CancellationSupDoc { get; set; }=new List<DocumentRequest>();
        public ICollection<FarmLandRequest> FarmLands { get; set; }=new List<FarmLandRequest>();
        public string CancellationReason { get; set; }
        public string Reason { get; set; }
        public string wfid { get; set; }
        public DateTime Date { get; set; }
        public FarmStatus Status { get; set; } = new FarmStatus();
        public bool Locked { get; set; }
    }

    public class ContractModificationRequest
    {
        public string FarmId { get; set; }
        public FarmStatus ModificationReason { get; set; } = new FarmStatus();
        public string Description { get; set; }
        public ICollection<FarmLandRequest> FarmLands { get; set; }=new List<FarmLandRequest>();
        public IList<DocumentRequest> SupportiveDocument { get; set; }=new List<DocumentRequest>();
        public IList<CancelledRightRequest> ModifiedRight { get; set; }=new List<CancelledRightRequest>();
        public FarmStatus Status { get; set; } = new FarmStatus();
        public bool Locked { get; set; }
    }

    public class ContractWarningRequest
    {
        public string Id { get; set; }
        public string FarmId { get; set; }
        public string LandId { get; set; }
        public int SplitIndex {get; set;}
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int Stage { get; set; }
        public string Reason { get; set; }
        public string wfid { get; set; }
        public IList<DocumentRequest> SupportiveDocument { get; set; }=new List<DocumentRequest>();
        public CancelledRightRequest WarnedRight { get; set; }=new CancelledRightRequest();
        
    }
    public class FarmLandRequest
    {
        public string LandId { get; set; }
        public string FarmId { get; set; }

        public DocumentRequest CertificateDoc { get; set; }=new DocumentRequest();
        public DocumentRequest LeaseContractDoc { get; set; }=new DocumentRequest();
        public int SplitIndex { get; set; }
    }

    public class FarmRegistrationRequest
    {
        public int Id { get; set; } // read-only
        public string RegistrationNumber { get; set; }
        public int AuthorityId { get; set; }
        public int TypeId { get; set; }
        public string DocumentId { get; set; } // only for modification and deletion

        public DocumentRequest Document { get; set; }=new DocumentRequest();
    }

    public class FarmOperatorRequest
    {
        public string Id { get; set; } // read-only
        public string Name { get; set; }
        public string Nationality { get; set; }
        public int TypeId { get; set; }
        public string AddressId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int OriginId { get; set; }
        public double? Capital { get; set; }
        
        // if TypeId == 1:
        public string Gender { get; set; } // 'F' = Female, 'M' = Male
        public int? MartialStatus { get; set; } // 1 = Not Married, 2 = Married, 3 = Divorced, 4 = Widowed
        public long? Birthdate { get; set; } // javascript date format (in millis)

        
        // if TypeId == 6:
        public string[] Ventures { get; set; } = { };

        
        public ICollection<FarmOperatorRegistrationRequest> Registrations { get; set; }
        public string PhotoId { get; set; }
        public DocumentRequest Photo { get; set; }
    }

    public class FarmOperatorRegistrationRequest
    {
        public int Id { get; set; } // read-only
        public string RegistrationNumber { get; set; }
        public int AuthorityId { get; set; }
        public int TypeId { get; set; }
        public string DocumentId { get; set; } // only for modification and deletion

        public DocumentRequest Document { get; set; }
    }

    public class CancelledRightRequest
    {
        public string Id { get; set; }
        public string LandId { get; set; }
        public string FarmId { get; set; }
        public DateTime RightFrom { get; set; }
        public DateTime RightTo { get; set; }
        public int RightType { get; set; }
        public double YearlyRent { get; set; }
        public double LandSectionArea { get; set; }
        public int SplitIndex { get; set; }
        public string CommonTxtUid { get; set; }
        public string Geom { get; set; }
        public FarmStatus Status { get; set; } = new FarmStatus();
    }
    
    public class ContractRenewalRequest
    {
        public string Id { get; set; }
        public string FarmId { get; set; }
        public string LandId { get; set; }
        public int SplitIndex {get; set;}
        public DateTime Date { get; set; }
        public string Remark { get; set; }
        public int BudgetYear { get; set; }
        public string wfid { get; set; }
        public IList<ContractRenewalDocumentRequest> SupportiveDocument { get; set; }=new List<ContractRenewalDocumentRequest>();
        public CancelledRightRequest RenewRight { get; set; }=new CancelledRightRequest();
        
    }

    public class ContractRenewalDocumentRequest
    {
        public string RegistrationNumber { get; set; }
        public int AuthorityId { get; set; }
        public int TypeId { get; set; }
        public string DocumentId { get; set; } // only for modification and deletion
        public DocumentRequest Document { get; set; }=new DocumentRequest();
    }
}