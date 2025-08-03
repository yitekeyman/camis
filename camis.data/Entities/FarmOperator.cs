using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class FarmOperator
    {
        public FarmOperator()
        {
            Farm = new HashSet<Farm>();
            FarmOperatorRegistration = new HashSet<FarmOperatorRegistration>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public int TypeId { get; set; }
        public Guid AddressId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int OriginId { get; set; }
        public double? Capital { get; set; }
        public string Gender { get; set; }
        public int? MartialStatus { get; set; }
        public long? Birthdate { get; set; }
        public Guid[] Ventures { get; set; }

        public Address Address { get; set; }
        public FarmOperatorOrigin Origin { get; set; }
        public FarmOperatorType Type { get; set; }
        public ICollection<Farm> Farm { get; set; }
        public ICollection<FarmOperatorRegistration> FarmOperatorRegistration { get; set; }
    }
}
