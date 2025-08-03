using intaps.camisPortal.Entities;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.LandBank;
using System;
using System.Collections.Generic;
using System.Net;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Projects.Models;
using intaps.camisPortal.Service;
using intaps.camisPortal;
using Newtonsoft.Json;
using RestSharp;


namespace intaps.camisPortal.Service
{
    public static class SerializerSetting
    {
        public static JsonSerializerSettings Get()
        {
            return new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
    public class CamisInterface
    {
        String sessionID = null;
        private PortalUser user;
        private string regionCode;
        private bool _sessionRetreieved= false;
        private readonly RestClient client;

        public CamisInterface(PortalUser user, string regionCode)
        {
            this.user = new PortalUser()
            {
                CamisUserName = "admin",
                CamisPassword = "spatni",
                UserName = user.UserName,
                Region = user.Region,
                Role = user.Role
            };
            this.regionCode = regionCode;
        }

        class VoidType {}
        private GetType invokeServer<DataType, GetType>(String cmd, DataType data)
            where DataType : class
            where GetType : class, new()
        {
            var request = new RestRequest(cmd);
            if (!string.IsNullOrEmpty(sessionID))
            {
                request.AddCookie(".ASPNetCoreSession", sessionID, "/", null);
            }

            RestResponse response;
            if (data == null)
            {
                response = client.GetAsync(request).GetAwaiter().GetResult();
            }
            else
            {
                request.AddJsonBody(data);
                response = client.PostAsync(request).GetAwaiter().GetResult();
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException(
                    $"Call to CAMIS failed. Status: {response.StatusCode}, Error: {response.ErrorMessage}");
            }

            var sessionCookie = response.Cookies.FirstOrDefault(c =>
                c.Name.Equals(".ASPNetCoreSession", StringComparison.OrdinalIgnoreCase));
            if (sessionCookie != null && !string.IsNullOrEmpty(sessionCookie.Value))
            {
                sessionID = sessionCookie.Value;
            }

            return JsonConvert.DeserializeObject<GetType>(response.Content, SerializerSetting.Get());
        }

        public class CamisUserNamePasword
        {
            public String Username { get; set; }
            public String Password { get; set; }
        }
        public class CamisLoginRes
        {
            public String message { get; set; }
        }
        void Login(String userName, String password)
        {
            var res = invokeServer<CamisUserNamePasword, CamisLoginRes>("api/admin/login", new CamisUserNamePasword() { Username = userName, Password = password });
            if (res == null)
                throw new Exception("Login failed");
        }
        
        public class Role
        {
            public int role;
        }
        void setRole(int role)
        {
            var res=invokeServer<Role, CamisLoginRes>("api/admin/SetRole", new Role()
            {
                role = role
            });
        }
        
        public LandBankFacadeModel.LandData getLandData(string upid,bool includeGeom,bool dd)
        {
            String cmd = String.Format("api/landbank/getland?landid&upin={0}&geom="+includeGeom+ "&dd=" + dd, upid);
            String userName = user.CamisUserName;
            String passWord = user.CamisPassword;
            Login(userName, passWord);
            var parcel = invokeServer<VoidType, LandBankFacadeModel.LandData>(cmd, null);
            return parcel;
        }
        
        public class WorkflowIDResponse
        {
            public Guid workflowId { get; set; }
            public bool success { get; set; }
            public String message { get; set; }
        }
        
        internal Guid StartFarmRegistration(FarmRequest farmRequest, String note)
        {
            FarmClerkLogin();
            var cmd = "api/farms/SaveNewFarmRegistration?description=" + note;
            var response = invokeServer<FarmRequest, WorkflowIDResponse>(cmd, farmRequest);
            if (!response.success)
                throw new InvalidOperationException($"Camis rejected farm registration request.\n{response.message}");
            return response.workflowId;
        }

        public FarmOperatorResponse GetLatestFarmOperator(Guid farmOperatorId, bool noLogin = false)
        {
            if (!noLogin) FarmClerkLogin();

            var cmd = "api/Farms/FarmOperator/" + farmOperatorId;
            return invokeServer<VoidType, FarmOperatorResponse>(cmd, null);
            // NOTE: the actual response is FarmOperatorResponse (currently, a superset model of FarmOperatorRequest)
        }
        
        public FarmRequest GetLatestFarm(Guid farmId, bool noLogin = false)
        {
            if (!noLogin) FarmClerkLogin();

            var cmd = "api/Farms/Farm/" + farmId;
            return invokeServer<VoidType, FarmRequest>(cmd, null);
        }
        
        public ActivityPlanResponse GetLatestPlanFromRootActivity(Guid rootActivityId, bool noLogin = false)
        {
            if (!noLogin) FarmClerkLogin();

            var cmd = "api/Projects/PlanFromRootActivity/" + rootActivityId;
            return invokeServer<VoidType, ActivityPlanResponse>(cmd, null);
        }


        public void FarmClerkLogin()
        {
            var userName = user.CamisUserName;
            var passWord = user.CamisPassword;
            Login(userName, passWord);
            setRole(2);
        }

        public void MnEExpertLogin()
        {
            var userName = user.CamisUserName;
            var passWord = user.CamisPassword;
            Login(userName, passWord);
            setRole(8);
        }


        public IList<ActivityStatusTypeResponse> ActivityStatusTypes()
        {
            MnEExpertLogin();
            const string cmd = "api/Projects/ActivityStatusTypes";
            return invokeServer<VoidType, List<ActivityStatusTypeResponse>>(cmd, null);
        }

        public IList<ActivityProgressMeasuringUnitResponse> ActivityProgressMeasuringUnits()
        {
            MnEExpertLogin();
            const string cmd = "api/Projects/ActivityProgressMeasuringUnits";
            return invokeServer<VoidType, List<ActivityProgressMeasuringUnitResponse>>(cmd, null);
        }

        public IList<ActivityProgressVariableResponse> ActivityProgressVariables()
        {
            MnEExpertLogin();
            const string cmd = "api/Projects/ActivityProgressVariables";
            return invokeServer<VoidType, List<ActivityProgressVariableResponse>>(cmd, null);
        }

        public IList<ActivityProgressVariableTypeResponse> ActivityProgressVariableTypes()
        {
            MnEExpertLogin();
            const string cmd = "api/Projects/ActivityProgressVariableTypes";
            return invokeServer<VoidType, List<ActivityProgressVariableTypeResponse>>(cmd, null);
        }

        public IList<ActivityVariableValueListResponse> ActivityVariableValueLists()
        {
            MnEExpertLogin();
            const string cmd = "api/Projects/ActivityVariableValueLists";
            return invokeServer<VoidType, List<ActivityVariableValueListResponse>>(cmd, null);
        }
        
        
        public ActivityPlanResponse ActivityPlan(Guid id)
        {
            MnEExpertLogin();
            var cmd = $"api/Projects/ActivityPlan/{id}";
            return invokeServer<VoidType, ActivityPlanResponse>(cmd, null);
        }
        
        public ActivityPlanResponse PlanFromRootActivity(Guid id)
        {
            MnEExpertLogin();
            var cmd = $"api/Projects/PlanFromRootActivity/{id}";
            return invokeServer<VoidType, ActivityPlanResponse>(cmd, null);
        }
        
        public ActivityResponse Activity(Guid id)
        {
            MnEExpertLogin();
            var cmd = $"api/Projects/Activity/{id}";
            return invokeServer<VoidType, ActivityResponse>(cmd, null);
        }
        
        public ActivityProgressReportResponse ProgressReport(Guid id)
        {
            MnEExpertLogin();
            var cmd = $"api/Projects/ProgressReport/{id}";
            return invokeServer<VoidType, ActivityProgressReportResponse>(cmd, null);
        }
        
        
        public PaginatorResponse<ActivityProgressReportResponse> SearchReports(Guid planId, string term, int skip, int take)
        {
            MnEExpertLogin();
            var cmd = $"api/Projects/SearchReports/{planId}?term={term}&skip={skip}&take={take}";
            return invokeServer<VoidType, PaginatorResponse<ActivityProgressReportResponse>>(cmd, null);
        }
        
        
        public double CalculateProgress(Guid planId, long? reportTime)
        {
           throw new NotImplementedException();
//            MnEExpertLogin();
//            var cmd = $"api/Projects/CalculateProgress/{planId}?reportTime={reportTime}";
//            // todo: handle the Double type...
//            return invokeServer<VoidType, Double>(cmd, null);
        }
        
        public IList<CalculatedVariableProgressResponse> CalculateResourceProgress(Guid activityId, long? reportTime)
        {
            MnEExpertLogin();
            var cmd = $"api/Projects/CalculateResourceProgress/{activityId}?reportTime={reportTime}";
            return invokeServer<VoidType, List<CalculatedVariableProgressResponse>>(cmd, null);
        }
        
        public IList<CalculatedVariableProgressResponse> CalculateOutcomeProgress(Guid activityId, long? reportTime)
        {
            MnEExpertLogin();
            var cmd = $"api/Projects/CalculateOutcomeProgress/{activityId}?reportTime={reportTime}";
            return invokeServer<VoidType, List<CalculatedVariableProgressResponse>>(cmd, null);
        }
        
        
        public Guid SubmitNewProgressReport(ActivityPlanRequest body, string description)
        {
            MnEExpertLogin();
            var cmd = $"api/Projects/SubmitNewProgressReport?description={description}";
            var response = invokeServer<ActivityPlanRequest, WorkflowIDResponse>(cmd, body);
            if (!response.success)
                throw new InvalidOperationException($"Camis rejected this new progress report submission.\n{response.message}");
            return response.workflowId;
        }
        
        
        public Guid RequestNewUpdatePlan(ActivityPlanRequest body, string description)
        {
            FarmClerkLogin();
            var cmd = $"api/Projects/RequestNewUpdatePlan?description={description}";
            var response = invokeServer<ActivityPlanRequest, WorkflowIDResponse>(cmd, body);
            if (!response.success)
                throw new InvalidOperationException($"Camis rejected this new Update Plan request.\n{response.message}");
            return response.workflowId;
        }
        

        public IList<ActivityPlanTemplateResponse> ActivityPlanTemplates()
        {
            MnEExpertLogin();
            const string cmd = "api/Projects/ActivityPlanTemplates";
            return invokeServer<VoidType, List<ActivityPlanTemplateResponse>>(cmd, null);
        }
        private string GetUrl()
        {
            var reg = new PortalService().GetRegion(regionCode);
            return reg.CamisUrl; 
        }
    }
}
