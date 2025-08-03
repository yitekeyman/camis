using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using camis.aggregator.data.Entities;
using camis.aggregator.domain.Admin;
using camis.aggregator.domain.Infrastructure;
using camis.aggregator.web.Extensions;
using camis.aggregator.web.Filter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace camis.aggregator.web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private IUserFacade _userFacade;
        static Dictionary<String, UserSession> sessions = new Dictionary<string, UserSession>();

        public static UserSession GetSession(String sid)
        {
            if (sessions.ContainsKey(sid))
                return sessions[sid];
            return null;
        }


        public AuthController(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }



        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            try
            {
                int role;
                var user = new User();
                //if (loginViewModel.UserName != "admin")
                //{
                    _userFacade.LoginUser(null, loginViewModel);
                    role = (int)_userFacade.GetUserRoles(loginViewModel.UserName)[0];
                    user = _userFacade.GetUser(loginViewModel.UserName);
                //}
                //else
                //{
                //    role = 1;
                //}


                var us = new UserSession
                {
                    Username = loginViewModel.UserName,
                    CreatedTime = DateTime.Now,
                    LastSeen = DateTime.Now,
                    Role = role,
                    id = loginViewModel.UserName != "amdin" ? user.Id.ToString() : "0",
                };
                HttpContext.Session.SetSession("sessionInfo", us);

                return SuccessfulResponse(us);
            }

            catch (Exception e)
            {

                return ErrorResponse(e);
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
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        [Roles]
        public IActionResult SetRole([FromBody] JObject userRole)
        {
            var role = (int)userRole["role"];
            var session = GetSession();

            HttpContext.Session.SetSession("sessionInfo", new UserSession
            {
                Username = session.Username,
                CreatedTime = DateTime.Now,
                LastSeen = DateTime.Now,
                Role = role
            });

            return Json(new { message = "success" });
        }

        [Roles((long)UserRoles.Admin)]
        [HttpPost]
        public IActionResult Register([FromBody] RegisterViewModel userModel)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                var session = GetSession();
                _userFacade.RegisterUser(session, userModel);
                return SuccessfulResponse(true);
            }
            catch (Exception e)
            {
                return ErrorResponse(e);
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
                return Json(new { message = "success" });
            }
            catch (Exception e)
            {
                return Json(new { errorCode = e.GetType().Name, message = e.Message });
            }
        }

        [Roles((long)UserRoles.Admin)]
        public IActionResult ResetPassword([FromBody] ResetPasswordViewModel resetVm)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                _userFacade.ResetPassword(GetSession(), resetVm.UserName, resetVm.NewPassword);
                return Json(new { message = "success" });
            }
            catch (Exception e)
            {
                return Json(new { errorCode = e.GetType().Name, message = e.Message });
            }
        }

        [Roles((long)UserRoles.Admin)]
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
               var res = _userFacade.GetAllUsers(GetSession());
                return SuccessfulResponse(res);
            }
            catch (Exception e)
            {
                return ErrorResponse(e);
            }
        }

        [Roles((long)UserRoles.Admin)]
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
        [Roles((long)UserRoles.Admin)]
        public IActionResult Update([FromBody] UserViewModel userVm)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                _userFacade.UpdateUser(GetSession(), userVm);
                return new JsonResult(new { message = "Success" }) { StatusCode = 200 };
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }

        [Roles((long)UserRoles.Admin)]
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
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }

        [Roles((long)UserRoles.Admin)]
        [HttpPost]
        public IActionResult Deactivate([FromBody] JObject user)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                var username = (string)user["username"];
                _userFacade.DeactivateUser(GetSession(), username);
                return Json(true);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }

        [Roles((long)UserRoles.Admin)]
        [HttpPost]
        public IActionResult Activate([FromBody] JObject user)
        {
            if (!ModelState.IsValid) return Json(false);

            try
            {
                var username = (string)user["username"];
                _userFacade.ActivateUser(GetSession(), username);
                return Json(true);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }


        [Roles]
        [HttpGet]
        public IActionResult Search(string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query)) return Json(_userFacade.GetAllUsers(GetSession()));

                return Json(_userFacade.GetUsers(GetSession(), query));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return StatusCode(500, new { message = "Internal server error occurred" });
            }
        }


        // [Roles]
        [HttpPost]
        public IActionResult Logout()
        {
            base.HttpContext.Session.Clear();
            return Json(true);
        }
    }

}