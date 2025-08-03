using System;
using System.Collections.Generic;

namespace intaps.camisPortal.Entities
{
    public partial class ApplicationEvaluation
    {
        public Guid PromotionUnitId { get; set; }
        public Guid InvestorId { get; set; }
        public long? SubmitDate { get; set; }
        public string EvaluationDetail { get; set; }
        public double? EvaluationPoint { get; set; }
        public string EvaluatorUserName { get; set; }
        public Guid TeamId { get; set; }

        public PortalUser EvaluatorUserNameNavigation { get; set; }
        public Investor Investor { get; set; }
        public PromotionUnit PromotionUnit { get; set; }
    }
}
