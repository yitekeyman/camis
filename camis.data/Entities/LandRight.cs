using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace intapscamis.camis.data.Entities
{
    public partial class LandRight
    {
        public Guid LandId { get; set; }

        public long? RightFrom { get; set; }

        public long? RightTo { get; set; }

        public Guid? ContractDocument { get; set; }

        public Guid? CertificateDocument { get; set; }

        public double? YearlyRent { get; set; }

        public int? RightType { get; set; }

        public double? LandSectionArea { get; set; }

        public int SplitIndex { get; set; }

        public Guid? CommonTxtUid { get; set; }

        public int? Status { get; set; }

        public Geometry Geom { get; set; }

        public Guid FarmId { get; set; }

        public virtual Document CertificateDocumentNavigation { get; set; }

        public virtual Document ContractDocumentNavigation { get; set; }

        public virtual Land Land { get; set; }
    }
}
