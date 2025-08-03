
using System;

namespace intaps.camisPortal.model
{
    public  class BidModel
    {
        public string  Id { get; set; }
        public DateTime BidDueDate { get; set; }
        public DateTime BidDeadline { get; set; }
        public string BidTitle { get; set; }
        public int BidFor { get; set; }
        public string ProcurementRefNo { get; set; }
        public string GlobalDevGrandNo { get; set; }
        public string BidSummery { get; set; }
        public string BidDetails { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string PhysicalAddress { get; set; }
        public BidDocument BidDoc=new BidDocument();
    }

    public class BidDocument
    {
        public string Id {get; set;}
        public string Data { get; set; }
        public string DocName { get; set; }
        public string Mime { get; set; }
        public string Ref { get; set; }
        
    }

    public class BidParticipationModel
    {
        public string Id { get; set;}
        public string Name { get; set; }
        public string Nationality { get; set; }
        public int OperatorType {get; set; }
        public string OperatorOrigin { get; set; }
        public string Capital { get; set; } 
        public int FarmType { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone{ get; set; }
        public string ContactEmail { get; set; }
        public string BidId { get; set; }
        public string Username { get; set; }
        public DateTime SubmissionDate { get; set; }
        public BidDocument TechnicalProposal=new BidDocument();
        public BidDocument FinancialProposal=new BidDocument();
    }
}