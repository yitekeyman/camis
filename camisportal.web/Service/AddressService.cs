using intaps.camisPortal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace intaps.camisPortal.Service
{
    public class AddressService 
    {
        CamisPortalContext _context = null;
        public AddressService(CamisPortalContext context)
        {
            _context = context;
        }

        public IList<AddressServiceModel.AddressSchemeResponse> GetAllSchemes()
        {
            var addressSchemes = _context.AddressScheme;
            return addressSchemes.AsEnumerable().Select(addressScheme => new AddressServiceModel.AddressSchemeResponse
            {
                Id = addressScheme.Id,
                Name = addressScheme.Name,
                Addresses = GetAddresses(addressScheme.Id, null)
            })
                .ToList();
        }
        public IList<AddressServiceModel.AddressUnitResponse> GetAddressUnits(int schemeId)
        {
            return _context.AddressSchemeUnit.Where(schemeUnit => schemeUnit.SchemeId == schemeId)
                .OrderBy(schemeUnit => schemeUnit.Order).Select(schemeUnit => ParseAddressUnitResponse(schemeUnit.Unit))
                .ToList();
        }

        public IList<AddressServiceModel.AddressResponse> GetAddresses(int addressSchemeId, Guid? parentId)
        {
            var addressScheme = _context.AddressScheme.First(s => s.Id == addressSchemeId);

            IList<AddressServiceModel.AddressResponse> viewModels = new List<AddressServiceModel.AddressResponse>();

            var schemeUnits = _context.AddressSchemeUnit.Where(asu => asu.Scheme.Id == addressScheme.Id);
            foreach (var schemeUnit in schemeUnits)
            {
                var addresses = _context.Address
                    .Where(a => (a.ParentId == null && parentId == null ||
                                 a.ParentId == parentId.ToString()) && a.UnitId == schemeUnit.UnitId);
                foreach (var address in addresses)
                {
                    var unit = _context.AddressUnit.First(au => au.Id == address.UnitId);
                    viewModels.Add(new AddressServiceModel.AddressResponse
                    {
                        Id = address.Id,
                        Unit = unit?.Name,
                        Name = address.Name
                    });
                }
            }

            return viewModels;
        }


        public IList<AddressServiceModel.AddressPairResponse> GetAddressPairs(Guid leafId)
        {
            var ret = new List<AddressServiceModel.AddressPairResponse>();

            var address = _context.Address.First(a => a.Id == leafId);
            while (address != null)
            {
                ret.Add(new AddressServiceModel.AddressPairResponse
                {
                    Unit = _context.AddressUnit.First(u => u.Id == address.UnitId).Name,
                    Value = address.Name
                });
                address = address.ParentId == null
                    ? null
                    : _context.Address.First(a => a.Id == Guid.Parse(address.ParentId));
            }

            ret.Reverse();
            return ret;
        }
        
        public AddressServiceModel.AddressResponse SaveAddress(AddressServiceModel.CustomAddressRequest data)
        {
            var existing = _context.Address.FirstOrDefault(old =>
                old.ParentId == data.ParentId.ToString() && old.UnitId == data.UnitId &&
                old.Name.Trim().ToLower() == data.CustomAddressName.Trim().ToLower());
            if (existing != null)
            {
                return ParseAddressResponse(existing, _context.AddressUnit.Find(existing.UnitId));
            }

            if (data.ParentId != null)
            {
                _context.Address.Find(data.ParentId); // throws an exception if parent address Id is incorrect
            }

            var address = new Address
            {
                Id = Guid.NewGuid(),
                ParentId = data.ParentId.ToString(),
                UnitId = data.UnitId,
                Name = data.CustomAddressName
            };
            _context.Address.Add(address);
            _context.SaveChanges();

            return ParseAddressResponse(address, _context.AddressUnit.Find(address.UnitId));
        }
        
        
        private static AddressServiceModel.AddressUnitResponse ParseAddressUnitResponse(AddressUnit unit)
        {
            return new AddressServiceModel.AddressUnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Custom = unit.Custom?? false
            };
        }

        private static AddressServiceModel.AddressResponse ParseAddressResponse(Address address, AddressUnit unit)
        {
            return new AddressServiceModel.AddressResponse
            {
                Id = address.Id,
                Unit = unit.Name,
                Name = address.Name,
                UnitId = unit.Id,
                Custom = unit.Custom ?? false
            };
        }
    }
}
