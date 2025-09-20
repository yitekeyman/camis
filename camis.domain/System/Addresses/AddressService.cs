using System;
using System.Collections.Generic;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.System.Addresses.Models;

namespace intapscamis.camis.domain.System.Addresses
{
    public interface IAddressService : ICamisService
    {
        IList<AddressSchemeResponse> GetAllSchemes();
        IList<AddressUnitResponse> GetAddressUnits(int schemeId);
        IList<AddressResponse> GetAddresses(int addressSchemeId, Guid? parentId);

        IList<AddressPairResponse> GetAddressPairs(Guid leafId);

        AddressResponse SaveAddress(CustomAddressRequest data);
    }

    public class AddressService : CamisService, IAddressService
    {
        public IList<AddressSchemeResponse> GetAllSchemes()
        {
            var addressSchemes = Context.AddressScheme.ToList();
    
            return addressSchemes.Select(addressScheme => new AddressSchemeResponse
            {
                Id = addressScheme.Id,
                Name = addressScheme.Name,
                Addresses = GetAddresses(addressScheme.Id, null)
            }).ToList();
        }

        public IList<AddressUnitResponse> GetAddressUnits(int schemeId)
        {
            return Context.AddressSchemeUnit.Where(schemeUnit => schemeUnit.SchemeId == schemeId)
                .OrderBy(schemeUnit => schemeUnit.Order).Select(schemeUnit => ParseAddressUnitResponse(schemeUnit.Unit))
                .ToList();
        }

        public IList<AddressResponse> GetAddresses(int addressSchemeId, Guid? parentId)
        {
            var addressScheme = Context.AddressScheme.First(s => s.Id == addressSchemeId);

            IList<AddressResponse> viewModels = new List<AddressResponse>();

            var schemeUnits = Context.AddressSchemeUnit.Where(asu => asu.Scheme.Id == addressScheme.Id).ToList();
            foreach (var schemeUnit in schemeUnits)
            {
                var addresses = Context.Address.Where(a =>
                        (a.ParentId == null && parentId == null || a.ParentId == parentId) &&
                        a.UnitId == schemeUnit.UnitId)
                    .OrderBy(a => a.Name).ToList();
                foreach (var address in addresses)
                {
                    viewModels.Add(ParseAddressResponse(address, Context.AddressUnit.Find(address.UnitId)));
                }
            }

            return viewModels;
        }


        public IList<AddressPairResponse> GetAddressPairs(Guid leafId)
        {
            var ret = new List<AddressPairResponse>();

            var address = Context.Address.First(a => a.Id == leafId);
            while (address != null)
            {
                ret.Add(new AddressPairResponse
                {
                    Unit = Context.AddressUnit.First(u => u.Id == address.UnitId).Name,
                    Value = address.Name
                });
                address = address.ParentId == null
                    ? null
                    : Context.Address.First(a => a.Id == address.ParentId);
            }

            ret.Reverse();
            return ret;
        }


        public AddressResponse SaveAddress(CustomAddressRequest data)
        {
            var existing = Context.Address.FirstOrDefault(old =>
                old.ParentId == data.ParentId && old.UnitId == data.UnitId &&
                old.Name.Trim().ToLower() == data.CustomAddressName.Trim().ToLower());
            if (existing != null)
            {
                return ParseAddressResponse(existing, Context.AddressUnit.Find(existing.UnitId));
            }

            if (data.ParentId != null)
            {
                Context.Address.Find(data.ParentId); // throws an exception if parent address Id is incorrect
            }

            var address = new Address
            {
                Id = Guid.NewGuid(),
                ParentId = data.ParentId,
                UnitId = data.UnitId,
                Name = data.CustomAddressName
            };
            Context.Address.Add(address);
            Context.SaveChanges();

            return ParseAddressResponse(address, Context.AddressUnit.Find(address.UnitId));
        }


        private static AddressUnitResponse ParseAddressUnitResponse(AddressUnit unit)
        {
            return new AddressUnitResponse
            {
                Id = unit.Id,
                Name = unit.Name,
                Custom = unit.Custom ?? false
            };
        }

        private static AddressResponse ParseAddressResponse(Address address, AddressUnit unit)
        {
            return new AddressResponse
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