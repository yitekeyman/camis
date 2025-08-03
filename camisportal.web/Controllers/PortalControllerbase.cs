using intaps.camisPortal.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace intaps.camisPortal.Controllers
{
    public class PortalControllerbase: Microsoft.AspNetCore.Mvc.Controller
    {
        private Service.PortalService _service = null;

        protected Service.PortalService Service
        {
            get
            {
                if (_service == null)
                    _service = new Service.PortalService();
                return _service;
            }
        }
        private bool _sessionRetreieved= false;
        private UserSession _session = null;
        protected UserSession GetSession()
        {
            if (_sessionRetreieved)
                return _session;
            byte[] data;
            if (!HttpContext.Session.TryGetValue("sessionInfo", out data))
                _session = null;
            else
            {
                var json=System.Text.UTF8Encoding.UTF8.GetString(data);
                if (json == null)
                    _session = null;
                else
                {
                    _session = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSession>(json);
                }
            }
            return _session;

        }
        protected void assertSession()
        {
            if (GetSession()==null)
            {
                throw new InvalidOperationException("User not logedin or sesssion has expired");
                
            }
        }
        protected Entities.PortalUser assertUserRole(PortalModel.UserRole role)
        {
            var usr = Service.GetUser(GetSession().userName);
            if (usr.Role != (int)role)
                throw new InvalidOperationException($"Access denied");
            return usr;
        }

        protected Entities.PortalUser assertUser()
        {
            return Service.GetUser(GetSession().userName);
        }
        
    }
}
