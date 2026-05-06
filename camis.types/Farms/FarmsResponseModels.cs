using System;
using System.Collections.Generic;
using intapscamis.camis.domain.Documents.Models;
using intapscamis.camis.domain.LandBank;

namespace intapscamis.camis.domain.Farms.Models
{
    public class FarmResponse
    {
        public Guid Id { get; set; }
        public Guid OperatorId { get; set; }
        public int TypeId { get; set; }
        public Guid ActivityId { get; set; } // only Id
        public double? InvestedCapital { get; set; }
        public string Description { get; set; }
        public int[] OtherTypeIds { get; set; } = { };

        public ICollection<FarmRegistrationResponse> Registrations { get; set; }

        public FarmOperatorResponse Operator { get; set; }
        public FarmTypeResponse Type { get; set; }

        public ICollection<FarmLandResponse> FarmLands { get; set; }
        public FarmStatus Status { get; set; }=new  FarmStatus();
        public bool Locked { get; set; }
    }

    public class FarmLandResponse
    {
        public Guid LandId { get; set; }
        public Guid? CertificateDocId { get; set; }
        public Guid? LeaseContractDocId { get; set; }
        public Guid FarmId { get; set; }

        public DocumentResponse CertificateDoc { get; set; }
        public DocumentResponse LeaseContractDoc { get; set; }
        public int SplitIndex { get; set; }
    }

    public class FarmLandResponse2
    {
        public string LandId { get; set; }
        public string Upin { get; set; }
        public string CertificateDocId { get; set; }
        public string LeaseContractDocId { get; set; }
        public string FarmId { get; set; }
        public int SplitIndex { get; set; }
        public double Area { get; set; }
        public string Geom { get; set; }
        public double CentroidX{get;set;}
        public double CentroidY{get;set;}
        public LandBankFacadeModel.LandRightResponse Rights { get; set; } = new LandBankFacadeModel.LandRightResponse();
    }
    public class LandRightsResponse
    {
        public string LandId { get; set; }
        public string FarmId { get; set; }
        public DateTime? RightFrom { get; set; }
        public DateTime? RightTo { get; set; }
        public int? RightType { get; set; }
        public double? YearlyRent { get; set; }
        public double? LandSectionArea { get; set; }
        public int SplitIndex { get; set; }
        public string CommonTxtUid { get; set; }
        public string Geom { get; set; }
        public FarmStatus Status { get; set; } = new FarmStatus();
    }

    public class FarmRegistrationResponse
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public int AuthorityId { get; set; }
        public int TypeId { get; set; }
        public string DocumentId { get; set; }

        public RegistrationAuthorityResponse Authority { get; set; }
        public RegistrationTypeResponse Type { get; set; }
        public DocumentResponse Document { get; set; }
    }

    public class FarmOperatorResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public int TypeId { get; set; }
        public Guid AddressId { get; set; } // only Id
        public string Phone { get; set; }
        public string Email { get; set; }
        public int OriginId { get; set; }
        public double? Capital { get; set; }

        // if TypeId == 1:
        public string Gender { get; set; } // 'F' = Female, 'M' = Male
        public int? MartialStatus { get; set; } // 1 = Not Married, 2 = Married, 3 = Divorced, 4 = Widowed
        public long? Birthdate { get; set; } // javascript date format (in millis)

        // if TypeId == 6:
        public Guid[] Ventures { get; set; } = { };
        public Guid? PhotoId { get; set; }

        public DocumentResponse Photo { get; set; }
        public ICollection<FarmOperatorRegistrationResponse> Registrations { get; set; }

        public FarmOperatorTypeResponse Type { get; set; }
        public FarmOperatorOriginResponse Origin { get; set; }
    }

    public class FarmOperatorRegistrationResponse
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public int AuthorityId { get; set; }
        public int TypeId { get; set; }
        public string DocumentId { get; set; }

        public RegistrationAuthorityResponse Authority { get; set; }
        public RegistrationTypeResponse Type { get; set; }
        public DocumentResponse Document { get; set; }
    }


    public class FarmOperatorTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class FarmTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RegistrationAuthorityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RegistrationTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class FarmOperatorOriginResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class FarmStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum FarmStatusEnum
    {
        OnEditing=1,
        Ready=2,
        TransactionLocked=3,
        Active=4,
        Expired=5,
        Suspended=6,
        Deleted=7,
        UnderWarning=8
    }
}