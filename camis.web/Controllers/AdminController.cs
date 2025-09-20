using System;
using System.Collections.Generic;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.Extensions;
using intapscamis.camis.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace intapscamis.camis.Controllers
{
    public class AdminController : BaseController
    {
        static Dictionary<String, UserSession> sessions = new Dictionary<string, UserSession>();
        public static UserSession GetSession(String sid)
        {
            if (sessions.ContainsKey(sid))
                return sessions[sid];
            return null;
        }
        private readonly IUserFacade _userFacade;

        public AdminController(IUserFacade facade)
        {
            _userFacade = facade;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                _userFacade.LoginUser(null, loginViewModel);
                var us = new UserSession
                {
                    Username = loginViewModel.UserName,
                    CreatedTime = DateTime.Now,
                    LastSeen = DateTime.Now,
                    Role = loginViewModel.Role,
                    id=Guid.NewGuid().ToString(),
                };
                HttpContext.Session.SetSession("sessionInfo", us);
                var sid = base.HttpContext.Session.Id;
                lock (sessions)
                {
                    if (sid != null)
                    {
                        if (sessions.ContainsKey(sid))
                            sessions.Remove(sid);
                        sessions.Add(sid, us);
                    }
                }
                return Json(new {sid=sid,message = "success"});
            }

            catch (Exception e)
            {
                return StatusCode(401, new {message = e.Message});
            }
        }

        [HttpGet]
        [Roles]
        public IActionResult GetRoles()
        {
            try
            {
                var session = GetSession();
                var roles = _userFacade.GetRoles(session);
                return Json(roles);
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }

        [HttpPost]
        [Roles]
        public IActionResult SetRole([FromBody] JObject userRole)
        {
            var role = (int) userRole["role"];
            var session = GetSession();

            HttpContext.Session.SetSession("sessionInfo", new UserSession
            {
                Username = session.Username,
                CreatedTime = DateTime.Now,
                LastSeen = DateTime.Now,
                Role = role
            });

            return Json(new {message = "success"});
        }

        [Roles(UserRoles.Admin)]
        [HttpPost]
        public IActionResult Register([FromBody] RegisterViewModel userModel)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                var session = GetSession();
                _userFacade.RegisterUser(session, userModel);
                return StatusCode(200, new {message = true});
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }

        [Roles]
        public IActionResult ChangePassword([FromBody] ChangePasswordViewModel changePassVm)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                var session = GetSession();
                _userFacade.ChangePassword(session, session.Username, changePassVm.OldPassword,
                    changePassVm.NewPassword);
                return Json(new {message = "success"});
            }
            catch (Exception e)
            {
                return Json(new {errorCode = e.GetType().Name, message = e.Message});
            }
        }

        [Roles(UserRoles.Admin)]
        public IActionResult ResetPassword([FromBody] ResetPasswordViewModel resetVm)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                _userFacade.ResetPassword(GetSession(), resetVm.UserName, resetVm.NewPassword);
                return Json(new {message = "success"});
            }
            catch (Exception e)
            {
                return Json(new {errorCode = e.GetType().Name, message = e.Message});
            }
        }

        [Roles(UserRoles.Admin)]
        [HttpGet]
        public IActionResult GetUsers([FromQuery] int status)
        {
            try
            {
                return Json(_userFacade.GetAllUsers(GetSession(),status));
            }
            catch (Exception e)
            {
                return Json(new {errorCode = 500, message = e.Message});
            }
        }

        [Roles(UserRoles.Admin)]
        [HttpGet]
        public IActionResult CheckUser(string username)
        {
            try
            {
                return Json(_userFacade.CheckUser(GetSession(), username));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return Json(false);
            }
        }

        [HttpPost]
        [Roles(UserRoles.Admin)]
        public IActionResult Update([FromBody] UserViewModel userVm)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                _userFacade.UpdateUser(GetSession(), userVm);
                return new JsonResult(new {message = "Success"}) {StatusCode = 200};
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {message = "Internal server error occurred"});
            }
        }

        [Roles(UserRoles.Admin)]
        [HttpPost]
        public IActionResult AddRole([FromBody] UserRoleViewModel roleVm)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                _userFacade.AddUserRole(GetSession(), roleVm.UserName, roleVm.Roles);
                return Json(true);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {message = "Internal server error occurred"});
            }
        }

        [Roles(UserRoles.Admin)]
        [HttpPost]
        public IActionResult Deactivate([FromBody] JObject user)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                var username = (string) user["username"];
                _userFacade.DeactivateUser(GetSession(), username);
                return Json(true);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {message = "Internal server error occurred"});
            }
        }

        [Roles(UserRoles.Admin)]
        [HttpPost]
        public IActionResult Activate([FromBody] JObject user)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                var username = (string) user["username"];
                _userFacade.ActivateUser(GetSession(), username);
                return Json(true);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {message = "Internal server error occurred"});
            }
        }


        [Roles]
        [HttpGet]
        public IActionResult Search([FromQuery]string query, int status)
        {
            try
            {
                if (string.IsNullOrEmpty(query)) return Json(_userFacade.GetAllUsers(GetSession(), status));

                return Json(_userFacade.GetUsers(GetSession(), query,status));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new {message = "Internal server error occurred"});
            }
        }


        [Roles]
        [HttpPost]
        public IActionResult Logout(String sid)
        {
            if (!string.IsNullOrEmpty(sid))
            {
                lock (sessions)
                {
                    if (sessions.ContainsKey(sid))
                        sessions.Remove(sid);
                }
            }
            base.HttpContext.Session.Clear();
            return Json(true);
        }
    }
}