using System;
using System.Collections.Generic;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Projects.Models;

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

        public ICollection<FarmRegistrationRequest> Registrations { get; set; }

        public FarmOperatorRequest Operator { get; set; } // if no OperatorId
        public ActivityPlanRequest ActivityPlan { get; set; } // only for registration

        // only for land transfer
        public LandBankFacadeModel.TransferRequest LandTransferRequest { get; set; }
        public Guid LandTransferWorkflowId { get; set; }

        public ICollection<FarmLandRequest> FarmLands { get; set; }
    }

    public class FarmLandRequest
    {
        public Guid LandId { get; set; }
        public Guid FarmId { get; set; }

        public DocumentRequest CertificateDoc { get; set; }
        public DocumentRequest LeaseContractDoc { get; set; }
    }

    public class FarmRegistrationRequest
    {
        public int Id { get; set; } // read-only
        public string RegistrationNumber { get; set; }
        public int AuthorityId { get; set; }
        public int TypeId { get; set; }
        public string DocumentId { get; set; } // only for modification and deletion

        public DocumentRequest Document { get; set; }
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
}