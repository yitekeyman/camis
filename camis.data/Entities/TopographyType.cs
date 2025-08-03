using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class TopographyType
    {
        public TopographyType()
        {
            Topography = new HashSet<Topography>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Topography> Topography { get; set; }
    }
}
