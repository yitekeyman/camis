using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityVariableValueList
    {
        public int VariableId { get; set; }
        public int Order { get; set; }
        public double Value { get; set; }
        public string Name { get; set; }

        public ActivityProgressVariable Variable { get; set; }
    }
}
