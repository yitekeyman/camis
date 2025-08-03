using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class ApplicationEvaluationTeam
    {
        public ApplicationEvaluationTeam()
        {
            EvaluationTeamMember = new HashSet<EvaluationTeamMember>();
        }

        public Guid? PromotionUnitId { get; set; }
        public Guid Id { get; set; }
        public string TeamName { get; set; }
        public string EvaluationCriterion { get; set; }
        public double? TeamWeight { get; set; }

        public PromotionUnit PromotionUnit { get; set; }
        public ICollection<EvaluationTeamMember> EvaluationTeamMember { get; set; }
    }
}
