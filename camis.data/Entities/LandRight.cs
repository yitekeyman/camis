using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class LandRight
    {
        public Guid LandId { get; set; }
        public long? RightFrom { get; set; }
        public long? RightTo { get; set; }
        public Guid? ContractDocument { get; set; }
        public Guid? CertificateDocument { get; set; }
        public int? RightType { get; set; }
        public double? YearlyRent { get; set; }
        public double? LandSectionArea { get; set; }

        public Document CertificateDocumentNavigation { get; set; }
        public Document ContractDocumentNavigation { get; set; }
        public Land Land { get; set; }
    }
}
