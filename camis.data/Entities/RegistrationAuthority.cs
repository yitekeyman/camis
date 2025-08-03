using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class RegistrationAuthority
    {
        public RegistrationAuthority()
        {
            FarmOperatorRegistration = new HashSet<FarmOperatorRegistration>();
            FarmRegistration = new HashSet<FarmRegistration>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<FarmOperatorRegistration> FarmOperatorRegistration { get; set; }
        public ICollection<FarmRegistration> FarmRegistration { get; set; }
    }
}
