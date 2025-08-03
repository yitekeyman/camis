using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class FarmRegistration
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public int AuthorityId { get; set; }
        public Guid FarmId { get; set; }
        public int TypeId { get; set; }
        public Guid? DocumentId { get; set; }

        public RegistrationAuthority Authority { get; set; }
        public Document Document { get; set; }
        public Farm Farm { get; set; }
        public RegistrationType Type { get; set; }
    }
}
