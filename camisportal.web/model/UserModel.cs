using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace intaps.camisPortal.model
{
    public class UserModel
    {
        public  string Username { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
    }

    public class LoginModel
    {
        public  string Username { get; set; }
        public string Password { get; set; }
    }

    public class SystemUserRequest
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string PhoneNo { get; set; }
        public string EMail { get; set; }
        public int Role { get; set; }
        public int Region { get; set; }
        public string Password { get; set; }
        public string CamisUserName { get; set; }
        public string CamisPassword { get; set; }
        public Boolean Active { get; set; }
    }

    public class SystemUserResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string PhoneNo { get; set; }
        public string EMail { get; set; }
        public int Role { get; set; }
        public int Region { get; set; }
        public string Password { get; set; }
        public string CamisUserName { get; set; }
        public string CamisPassword { get; set; }
        public Boolean Active { get; set; }
    }
}