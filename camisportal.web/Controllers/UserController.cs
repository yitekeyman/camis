using intaps.camisPortal.Extensions;
using intaps.camisPortal.model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using intaps.camisPortal.Service;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace intaps.camisPortal.Controllers
{
    public class UserController: PortalControllerbase
    {
        
        String hashPassword(string pw)
        {
            if (pw == null)
                return null;
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pw));
                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToUpper();
            }
        }
        
        [HttpPost]
        public IActionResult Login([FromBody] UserModel loginModel)
        {
            try
            {
                var u = Service.GetUser(loginModel.Username);
                if (u == null || !u.Password.Equals(hashPassword(loginModel.Password)))
                    throw new InvalidOperationException("Invalid username or password");
                if(u.Active==false)
                    throw new InvalidOperationException("you are not active user");
                var user = new Entities.PortalUser()
                {
                    Role=u.Role,
                    Region = u.Region
                };
                
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new UserSession() { userName = loginModel.Username });
                base.HttpContext.Session.Set("sessionInfo",System.Text.UTF8Encoding.UTF8.GetBytes(json));
                return Json(user);
            }
            catch (Exception e)
            {
                return StatusCode(401, new {message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                return Json(Service.GetUsers());

            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message}); 
            }
            
        }
        [HttpGet]
        public IActionResult GetPublicUsers()
        {
            try
            {
                return Json(Service.GetPublicUsers());

            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }

        }
        [HttpGet]
        public IActionResult GetRoles()
        {
            try
            {
                return Json(Service.GetRoles());

            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }

        }

        [HttpGet]
        public IActionResult GetUser(String userName)
        {
            try
            {
                return Json(Service.GetUser(userName));

            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult ActivateUser([FromQuery]String userName)
        {
            try
            {
                Service.ActivateUser(userName);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult DeactivateUser([FromQuery]String userName)
        {
            try
            {
                Service.DeactivateUser(userName);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateUserRegistration([FromBody]Entities.PortalUser user)
        {
            try
            {
                Service.UpdateUserRegistration(user);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        [HttpPost]
        public IActionResult RegisterUser([FromBody]Entities.PortalUser user)
        {
            try
            {
                user.Password = hashPassword(user.Password);
                Service.RegisterUser(user);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
        
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Json(true);
        }

        public IActionResult ShowDocument(string data, string mime)
        {
            try
            {
                
                var file = Convert.FromBase64String(data);
                return File(file, mime, null);
            }
            catch(Exception e)

            {
                return StatusCode(500, new {message = e.Message});
            }
        }
        [HttpGet]
        public IActionResult DownloadDocument([FromQuery]string data, string mime, string fileName)
        {
            try
            {
                return File(data, mime, fileName);
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }

        public IActionResult ChangePassword([FromQuery] string userName, string oldPassword, string newPassword)
        {
            try
            {
                newPassword = hashPassword(newPassword);
                oldPassword = hashPassword(oldPassword);
                Service.ChangePassword(userName, oldPassword, newPassword);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }
        public IActionResult ResetPassword([FromQuery]  string userName, string newPassword)
        {
            try
            {
                assertSession();
                var usr=assertUserRole(PortalModel.UserRole.WebMaster);
                newPassword = hashPassword(newPassword);
                Service.ResetPassword(usr, userName, newPassword);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }
        
        public IActionResult CheckUsername([FromQuery]String username)
        {
            return Json(Service.CheckUsername(username));
        }
        
        public IActionResult CheckEmail([FromQuery]String email)
        {
            return Json(Service.CheckEmail(email));

        }

        [HttpGet]
        public IActionResult GetAllRegions()
        {
            try
            {
                return Json(Service.GetRegions());
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet]
        public IActionResult GetRegion([FromQuery] string code)
        {
            try
            {
                return Json(Service.GetRegion(code));
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }

        [HttpPost]
        public IActionResult AddRegion([FromBody] Entities.Regions region)
        {
            try
            {
                Service.AddRegion(region);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }
        [HttpPost]
        public IActionResult UpdateRegion([FromBody] Entities.Regions region)
        {
            try
            {
                Service.UpdateRegion(region);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }

        [HttpPost]
        public IActionResult CheckSession([FromQuery] string userName)
        {
            try
            {
                assertSession();
                if(userName!=assertUser().UserName)
                    throw new InvalidOperationException($"User not logedin or sesssion has expired");
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(400, new {message = e.Message});
            }
        }
    }
}