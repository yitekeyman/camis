using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class Regions
    {
        public Regions()
        {
            PortalUser = new HashSet<PortalUser>();
            Promotion = new HashSet<Promotion>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string CamisUrl { get; set; }

        public ICollection<PortalUser> PortalUser { get; set; }
        public ICollection<Promotion> Promotion { get; set; }
    }
}
