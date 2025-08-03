using System;

namespace intapscamis.camis.domain.System.Addresses.Models
{
    public class CustomAddressRequest
    {
        public Guid? ParentId { get; set; }
        public int UnitId { get; set; }
        public string CustomAddressName { get; set; }
    }
}