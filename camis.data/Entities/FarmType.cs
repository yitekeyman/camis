using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class FarmType
    {
        public FarmType()
        {
            Farm = new HashSet<Farm>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Farm> Farm { get; set; }
    }
}
