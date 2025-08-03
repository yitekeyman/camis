using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class FarmOperatorType
    {
        public FarmOperatorType()
        {
            FarmOperator = new HashSet<FarmOperator>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<FarmOperator> FarmOperator { get; set; }
    }
}
