using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class Investor
    {
        public Investor()
        {
            ApplicationEvaluation = new HashSet<ApplicationEvaluation>();
            InvestorApplication = new HashSet<InvestorApplication>();
            InvestorApplicationDocument = new HashSet<InvestorApplicationDocument>();
            InvestorApplicationResubmissionRequest = new HashSet<InvestorApplicationResubmissionRequest>();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string DefaultProfile { get; set; }

        public PortalUser UserNameNavigation { get; set; }
        public ICollection<ApplicationEvaluation> ApplicationEvaluation { get; set; }
        public ICollection<InvestorApplication> InvestorApplication { get; set; }
        public ICollection<InvestorApplicationDocument> InvestorApplicationDocument { get; set; }
        public ICollection<InvestorApplicationResubmissionRequest> InvestorApplicationResubmissionRequest { get; set; }
    }
}
