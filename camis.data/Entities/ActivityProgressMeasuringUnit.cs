using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityProgressMeasuringUnit
    {
        public ActivityProgressMeasuringUnit()
        {
            ActivityProgressVariable = new HashSet<ActivityProgressVariable>();
            InverseConvertFromNavigation = new HashSet<ActivityProgressMeasuringUnit>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ConvertFrom { get; set; }
        public double? ConversionFactor { get; set; }
        public double? ConversionOffset { get; set; }

        public ActivityProgressMeasuringUnit ConvertFromNavigation { get; set; }
        public ICollection<ActivityProgressVariable> ActivityProgressVariable { get; set; }
        public ICollection<ActivityProgressMeasuringUnit> InverseConvertFromNavigation { get; set; }
    }
}
