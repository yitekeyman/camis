using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class Document
    {
        public Document()
        {
            ActivityPlanDocument = new HashSet<ActivityPlanDocument>();
            ActivityProgressReportDocument = new HashSet<ActivityProgressReportDocument>();
            FarmLandCertificateDocNavigation = new HashSet<FarmLand>();
            FarmLandLeaseContractDocNavigation = new HashSet<FarmLand>();
            FarmOperatorRegistration = new HashSet<FarmOperatorRegistration>();
            FarmRegistration = new HashSet<FarmRegistration>();
            LandDoc = new HashSet<LandDoc>();
            LandRightCertificateDocumentNavigation = new HashSet<LandRight>();
            LandRightContractDocumentNavigation = new HashSet<LandRight>();
        }

        public Guid Id { get; set; }
        public long Date { get; set; }
        public string Ref { get; set; }
        public string Note { get; set; }
        public string Mimetype { get; set; }
        public int? Type { get; set; }
        public long? Aid { get; set; }
        public string Filename { get; set; }
        public byte[] File { get; set; }
        public string OverrideFilePath { get; set; }

        public DocumentType TypeNavigation { get; set; }
        public ICollection<ActivityPlanDocument> ActivityPlanDocument { get; set; }
        public ICollection<ActivityProgressReportDocument> ActivityProgressReportDocument { get; set; }
        public ICollection<FarmLand> FarmLandCertificateDocNavigation { get; set; }
        public ICollection<FarmLand> FarmLandLeaseContractDocNavigation { get; set; }
        public ICollection<FarmOperatorRegistration> FarmOperatorRegistration { get; set; }
        public ICollection<FarmRegistration> FarmRegistration { get; set; }
        public ICollection<LandDoc> LandDoc { get; set; }
        public ICollection<LandRight> LandRightCertificateDocumentNavigation { get; set; }
        public ICollection<LandRight> LandRightContractDocumentNavigation { get; set; }
    }
}
