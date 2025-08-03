using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace intaps.camisPortal.model
{
    public class AddressModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public int UnitId { get; set; }
    }

    public class AddressSchema
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AddressSchemaUnit
    {
        public string Id { get; set; }
        public int Order { get; set; }
        public int SchemeId { get; set; }
        public int UnitId { get; set; }
    }

    public class AddressUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AddressResponse
    {
        public string Id { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
    }
    public class AddressSchemeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<AddressResponse> Addresses { get; set; }
    }
}