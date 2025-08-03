﻿using System;

namespace camis.aggregator.domain.Admin
{
    public enum UserActionType
    {
        Login = 1,
        Logout = 2,
        Register = 3,
        UpdateUser = 4,
        ActivateUser = 5,

        DeactivateUser = 6,
        ChangePassword = 7,
        ResetPassword = 8,

        AddWorkflow = 9,
        UpdateWorkflow = 10,
        AddWorkItem = 11,

        AddRegionUrl = 12,
        EditRegionUrl = 13,

        CreateLand = 14,
        UpdateLand = 15,
        RemoveLand = 16,

        RevokeRole=21,


        
    }

    public static class UserRoles
    {
        public const int Admin = 1;
        public const long User = 2;
    }

    public class UserViewModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int Status { get; set; }
        public string[] Roles { get; set; }
    }

    public class UserDetialViewModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int Status { get; set; }
        public string[] Roles { get; set; }
        public string RegOn { get; set; }
        public string LastSeen { get; set; }
    }

    public class UserActionViewModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Action { get; set; }
        public string ActionTime { get; set; }
    }

    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public long Role { get; set; }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int[] Roles { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserRoleViewModel
    {
        public string UserName { get; set; }
        public int[] Roles { get; set; }
    }
}