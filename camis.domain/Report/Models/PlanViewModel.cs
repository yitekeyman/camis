using System;
using System.Collections.Generic;
using System.Text;
using intapscamis.camis.data.Entities;
namespace intapscamis.camis.domain.Report.Models
{
    public class PlanViewModel
    {
        public ActivityPlan Plan { get; set; }
        public ActivityViewModel RootActivity { get; set; }
        public List<ActivityViewModel> SubActivites { get; set; }
        public Dictionary<ActivityViewModel, ActivityViewModel[]> Hierarchy { get; set; }
    }

    public class ActivityViewModel
    {
        public Activity Activity { get; set; }
        public List<ActivityPlanDetailViewModel> Detail { get; set; }
    }

    public class ActivityPlanDetailViewModel
    {
        public ActivityPlanDetail Detail { get; set; }
        public ActivityProgressVariable Variable { get; set; }
        public ActivityProgressMeasuringUnit ProgressMeasuringUnit { get; set; }
        public IList<ActivityVariableValueList> VariableValueList { get; set; }
        public ActivityVariableValueList TargetVariableValueList { get; set; }
    }
}
