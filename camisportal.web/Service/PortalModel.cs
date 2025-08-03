using intaps.camisPortal.Entities;
using intapscamis.camis.domain.LandBank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Projects.Models;

namespace intaps.camisPortal.Service
{
    public class PortalModel
    {
        public class PortalLand
        {
            public LandBankFacadeModel.LandData landData;
            public LandBankFacadeModel.LatLng location;
            public List<List<LandBankFacadeModel.LatLng>> polygon;
            public LandBankFacadeModel.LatLng bottomLeft;
            public LandBankFacadeModel.LatLng topRight;
        }
        public class DocumentData
        {
            public String docRef;
            public String mime;
            public String data;
            public int documentType = 0;
        }
        public const String REGION_CENTERAL = "99";

        public enum PromotionState
        {
            Posted=1,
            Approved=2,
            Closed =3,
            EvaluationStarted=4,
            EvaluationFinished = 5,
            ProposalAccepted=6,
            Finished = 7,
            Canceled=8,
        }

        public enum UserRole
        {
            Public=1,
            WebMaster=2,
            PromotionManager=3,
            ApplicationEvaluator=4,
        }

        public class UserRoleName
        {
            public int ID { get; set; }
            public String Name { get; set; }
        }

        public class InvestorSearchPar
        {
            public String registrationNumber;
        }
        public class SearchResult<T>
        {
            public List<T> result;
            public int NRec;
        }
        public class ValuListItem
        {
            public String name;
            public double value;
        }
        public class EvaluationCriterion
        {
            public String id;
            public String name;
            public double maxVal;
            public double weight;
            public List<ValuListItem> valueList;
            public List<EvaluationCriterion> cubCriterion;
        }
        public class EvaluationResult
        {
            public String id;
            public double val;
            public List<EvaluationResult> subResult;
        }
        public class EvaluationResultItem
        {
            public String investorId;
            public String teamId;
            public String userName;
            public double val;
        }

        
        public class PromotionUnitRequest
        {
            public class EvaluationTeam
            {
                public List<EvaluationTeamMember> members;
                public List<EvaluationCriterion> criterion;
                public double weight = 1;
                public string teamName;
                public string id;
            }
            public String landUPIN;
            public string id;
            public PromotionRequest promotion;
            public List<DocumentData> pictures;
            public List<DocumentData> documents;
            public List<EvaluationTeam> evalTeams;
            public LandBankFacadeModel.LandData landData;
            public int status;
            public Guid? winnerInvestor;

        }
        public class PromotionRequest
        {
            public String id;
            public DateTime applyDateFrom;
            public DateTime applyDateTo;
            public String title;
            public  String promotionRef;
            public String Summery;
            public String description;
            public String region;
            public int status;
            public DateTime postedOn;
            public String physicalAddress;
            public List<int> investmentTypes;
            public List<PromotionUnitRequest> promotionUnit;
        }

        public class PromotionSearchFilter
        {
            public String region;
            public int[] states;
        }
        public class PromotionSearchResultItem
        {
            public String promotion_id;
            public String regionCode;
            public String regionName;
            public String address;
            public String title;
            public String summary;
            public DateTime deadline;
            public double landArea;
            public int status;
            public DocumentData picture;
            public List<landUPIN> landUPINS;
        }
        public class landUPIN
        {
            public string upin;
            public string prom_id;
            public int status;
        }
        public class PromotionSeachResult
        {
            public int index=0;
            public int NRec;
            public List<PromotionSearchResultItem> result;
        }
        public class ApplicationContact
        {
            public String name;
            public String phone;
            public String email;
        }
        public class Application
        {
            public String promoID;
            public Entities.Investor invProfile;
            public List<DocumentData> proposalDocument;
            public String proposalAbstract; 
            public List<int> investmentTypes;
            public double proposedCapital = 0;
            public ApplicationContact contactAddress;
            public DateTime applicationTime;
            public bool rejected = false;
            public FarmRequest investment;
            public ActivityPlanResponse activityPlan;
            public Guid promotionUnitId;

            public bool isApproved = false;
        }

        public class SearchApplicationPar
        {
            public String promoID= null;
            public String investorID = null;
            public String promoUnitID = null;
        }

        public class EvaluationData
        {
            public String evaluationTeamID;
            public String evaluatorUserName;//automatically set from session
            public String promoID;
            public String investorID;
            public string promotionUnitID;
            public List<EvaluationResult> result;
        }

        public class EvaulationSummaryData
        {
            public List<PromotionUnitRequest.EvaluationTeam> teams;
            public List<EvaluationSummaryItem> evaluations;
        }
        public class EvaluationSummaryItem
        {
            public class TeamEvaluationSummary
            {
                public String teamName;
                public double weightedPoint;
            }
            public String investorID;
            public String investorName;
            public double totalPoint;
            public int rank;
            public List<TeamEvaluationSummary> teamEvaluation;
        }

        public class RegistrationDocument
        {
            public string Mimetype;
            public int Type;
            public string FileName;
            public string File;
            public string Note;
            public string Ref;
            public long Date;
        }

        public class Registration
        {
            public RegistrationDocument Document;
            public int TypeId;
            public int AuthorityId;
            public string RegistrationNumber;
        }
        public class DefaultProfile
        {
            public string Name;
            public string AddressId;
            public string Email;
            public string Phone;
            public double Capital;
            public string Nationality;
            public int TypeId;
            public int OriginId;
            public List<Registration> Registrations;
        }
        
        
    }
}
