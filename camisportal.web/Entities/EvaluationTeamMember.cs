using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class EvaluationTeamMember
    {
        public Guid TeamId { get; set; }
        public string UserName { get; set; }
        public double? Weight { get; set; }

        public ApplicationEvaluationTeam Team { get; set; }
        public PortalUser UserNameNavigation { get; set; }
    }
}
