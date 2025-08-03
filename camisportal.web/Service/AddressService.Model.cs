using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace intaps.camisPortal.Service
{
    public class AddressServiceModel
    {
        public class AddressSchemeResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ICollection<AddressResponse> Addresses { get; set; }
        }
        public class AddressUnitResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Custom { get; set; }
        }

        public class AddressResponse
        {
            public Guid Id { get; set; }     // address Id
            public string Unit { get; set; } // unit Name

            public string Name { get; set; } // address Name
            public int UnitId { get; set; }  // unit Id
            public bool Custom { get; set; } // unit Custom
        }


        public class AddressPairResponse
        {
            public string Unit { get; set; }
            public string Value { get; set; }
        }
        public class CustomAddressRequest
        {
            public Guid? ParentId { get; set; }
            public int UnitId { get; set; }
            public string CustomAddressName { get; set; }
        }
    }
}
