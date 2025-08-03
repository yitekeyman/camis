using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class InvestorRegistrationNumber
    {
        public Guid InvestorId { get; set; }
        public string RegisrationNumber { get; set; }
        public int RegistrationTypeId { get; set; }
    }
}
