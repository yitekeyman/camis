using System;
using System.Collections.Generic;

namespace intapscamis.camis.data.Entities
{
    public partial class ActivityProgressVariable
    {
        public ActivityProgressVariable()
        {
            ActivityPlanDetail = new HashSet<ActivityPlanDetail>();
            ActivityProgress = new HashSet<ActivityProgress>();
            ActivityVariableValueList = new HashSet<ActivityVariableValueList>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? DefaultUnitId { get; set; }
        public int TypeId { get; set; }

        public ActivityProgressMeasuringUnit DefaultUnit { get; set; }
        public ActivityProgressVariableType Type { get; set; }
        public ICollection<ActivityPlanDetail> ActivityPlanDetail { get; set; }
        public ICollection<ActivityProgress> ActivityProgress { get; set; }
        public ICollection<ActivityVariableValueList> ActivityVariableValueList { get; set; }
    }
}
