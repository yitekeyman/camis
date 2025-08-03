using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class PortalUser
    {
        public PortalUser()
        {
            ApplicationEvaluation = new HashSet<ApplicationEvaluation>();
            EvaluationTeamMember = new HashSet<EvaluationTeamMember>();
            Investor = new HashSet<Investor>();
        }

        public string UserName { get; set; }
        public int Role { get; set; }
        public string Region { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string EMail { get; set; }
        public bool Active { get; set; }
        public string CamisUserName { get; set; }
        public string CamisPassword { get; set; }

        public Regions RegionNavigation { get; set; }
        public ICollection<ApplicationEvaluation> ApplicationEvaluation { get; set; }
        public ICollection<EvaluationTeamMember> EvaluationTeamMember { get; set; }
        public ICollection<Investor> Investor { get; set; }
    }
}
