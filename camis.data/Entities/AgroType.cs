using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class AgroType
    {
        public AgroType()
        {
            AgroEchology = new HashSet<AgroEchology>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<AgroEchology> AgroEchology { get; set; }
    }
}
