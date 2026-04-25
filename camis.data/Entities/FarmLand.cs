using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class FarmLand
    {
        public Guid LandId { get; set; }
        public Guid? CertificateDoc { get; set; }
        public Guid? LeaseContractDoc { get; set; }
        public Guid FarmId { get; set; }
        public int SplitIndex { get; set; }
        public Document CertificateDocNavigation { get; set; }
        public Farm Farm { get; set; }
        public virtual Land Land { get; set; }
        public Document LeaseContractDocNavigation { get; set; }
        //public LandRight LandRight { get; set; }
    }
}
