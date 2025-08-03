using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class PromotionUnit
    {
        public PromotionUnit()
        {
            ApplicationEvaluation = new HashSet<ApplicationEvaluation>();
            ApplicationEvaluationTeam = new HashSet<ApplicationEvaluationTeam>();
            InvestorApplication = new HashSet<InvestorApplication>();
            InvestorApplicationDocument = new HashSet<InvestorApplicationDocument>();
            InvestorApplicationResubmissionRequest = new HashSet<InvestorApplicationResubmissionRequest>();
            PromotionDoc = new HashSet<PromotionDoc>();
            PromotionPicture = new HashSet<PromotionPicture>();
        }

        public Guid Id { get; set; }
        public Guid PromotionId { get; set; }
        public string LandProfile { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InvestmentType { get; set; }
        public Guid? WinnerInvestor { get; set; }
        public int Status { get; set; }
        public Promotion Promotion { get; set; }
        
        public ICollection<ApplicationEvaluation> ApplicationEvaluation { get; set; }
        public ICollection<ApplicationEvaluationTeam> ApplicationEvaluationTeam { get; set; }
        public ICollection<InvestorApplication> InvestorApplication { get; set; }
        public ICollection<InvestorApplicationDocument> InvestorApplicationDocument { get; set; }
        public ICollection<InvestorApplicationResubmissionRequest> InvestorApplicationResubmissionRequest { get; set; }
        public ICollection<PromotionDoc> PromotionDoc { get; set; }
        public ICollection<PromotionPicture> PromotionPicture { get; set; }
    }
}
