using System;
using System.Collections.Generic;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.System.Addresses.Models;

namespace intapscamis.camis.domain.System.Addresses
{
    public interface IAddressFacade : ICamisFacade
    {
        IList<AddressSchemeResponse> GetAllSchemes();
        IList<AddressUnitResponse> GetAddressUnits(int schemeId);
        IList<AddressResponse> GetAddresses(int addressSchemeId, Guid? parentId);

        IList<AddressPairResponse> GetAddressPairs(Guid leafId);
        
        AddressResponse SaveAddress(CustomAddressRequest data);
    }

    public class AddressFacade : CamisFacade, IAddressFacade
    {
        private readonly IAddressService _service;

        private readonly CamisContext _context;

        public AddressFacade(CamisContext context, IAddressService service)
        {
            _context = context;
            _service = service;
        }


        public IList<AddressSchemeResponse> GetAllSchemes()
        {
            PassContext(_service, _context);
            return _service.GetAllSchemes();
        }

        public IList<AddressUnitResponse> GetAddressUnits(int schemeId)
        {
            PassContext(_service, _context);
            return _service.GetAddressUnits(schemeId);
        }

        public IList<AddressResponse> GetAddresses(int addressSchemeId, Guid? parentId)
        {
            PassContext(_service, _context);
            return _service.GetAddresses(addressSchemeId, parentId);
        }


        public IList<AddressPairResponse> GetAddressPairs(Guid leafId)
        {
            PassContext(_service, _context);
            return _service.GetAddressPairs(leafId);
        }


        public AddressResponse SaveAddress(CustomAddressRequest data)
        {
            return Transact(_context, t =>
            {
                PassContext(_service, _context);
                return _service.SaveAddress(data);
            });
        }
    }
}