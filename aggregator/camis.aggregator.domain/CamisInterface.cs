using camis.aggregator.data.Entities;
using camis.aggregator.data.Entities;
using camis.aggregator.domain.Infrastructure;
using camis.aggregator.domain.LandBank;
using camis.aggregator.domain.Report;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.LandBank;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RestSharp;

namespace camis.aggregator.domain
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
        private string sessionID;
        private readonly string regionCode;
        private readonly string username;
        private readonly string password;
        private readonly string baseUrl;
        private readonly RestClient client;

        public CamisInterface(data.Entities.TRegions region)
        {
            regionCode = region.Csaregionid;
            username = region.Username;
            password = region.Password;
            baseUrl = region.Url.TrimEnd('/') + "/";

            var options = new RestClientOptions(baseUrl)
            {
                ThrowOnAnyError = true
            };
            client = new RestClient(options);
        }

        class VoidType
        {
        }

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

        void Login()
        {
            var res = invokeServer<CamisUserNamePasword, CamisLoginRes>("api/admin/login",
                new CamisUserNamePasword() { Username = this.username, Password = this.password });
            if (res == null)
                throw new Exception("Login failed");
        }

        public class Role
        {
            public int role;
        }

        void setRole(int role)
        {
            var res = invokeServer<Role, CamisLoginRes>("api/admin/SetRole", new Role()
            {
                role = role
            });
        }


        public class WorkflowIDResponse
        {
            public Guid workflowId { get; set; }
            public bool success { get; set; }
            public String message { get; set; }
        }


        public ReportResponseModel GetReport(ReportRequestModel Request)
        {
            Login();
            setRole(6);
            var cmd = "api/Report/GetReport2";
            var res = invokeServer<ReportRequestModel, ResponseModel>(cmd, Request);
            if (!res.status)
            {
                throw new Exception(res.message);
            }

            return JsonConvert.DeserializeObject<ReportResponseModel>(res.response, SerializerSetting.Get());
        }

        public List<Farm> GetFarms()
        {
            Login();
            setRole(6);
            var cmd = "api/Report/GetAllFarms2";
            var res = invokeServer<VoidType, ResponseModel>(cmd, null);
            if (!res.status)
            {
                throw new Exception(res.message);
            }

            return JsonConvert.DeserializeObject<List<Farm>>(res.response, SerializerSetting.Get());
        }

        public List<LandBankFacadeModel.LandData> GetLandData(Guid[] excluded)
        {
            Login();
            setRole(3);
            var cmd = "api/LandBank/GetLandData";
            var res = invokeServer<Guid[], ResponseModel>(cmd, excluded);
            if (!res.status)
            {
                throw new Exception(res.message);
            }

            return JsonConvert.DeserializeObject<List<LandBankFacadeModel.LandData>>(res.response,
                SerializerSetting.Get());
        }
    }

    public class ResponseModel
    {
        public bool status { get; set; }
        public string response { get; set; }
        public string message { get; set; }
    }
}