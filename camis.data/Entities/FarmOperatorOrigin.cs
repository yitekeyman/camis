using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class FarmOperatorOrigin
    {
        public FarmOperatorOrigin()
        {
            FarmOperator = new HashSet<FarmOperator>();
        }

        public string Name { get; set; }
        public int Id { get; set; }

        public ICollection<FarmOperator> FarmOperator { get; set; }
    }
}
