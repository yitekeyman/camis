﻿using System;

namespace intapscamis.camis.domain.Admin
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

        CreateFarm = 12,
        UpdateFarm = 13,
        DeleteFarm = 14,

        CreateFarmOperator = 15,
        UpdateFarmOperator = 16,
        DeleteFarmOperator = 17,
        
        UpdateActivityPlan = 18,
        CreateActivityPlan = 19,
        CreateChildActivity = 20,
        RevokeRole=21,

        CreateDocument = 22,
        UpdateDocument = 23,
        DeleteDocument = 24,
        
        CreateActivityProgressReport = 25,
        
        CreateActivityPlanTemplate = 26,
        UpdateActivityPlanTemplate = 27,
        DeleteActivityPlanTemplate = 28,

        //1000 land 
        CreateLand = 1001,
        UpdateLand = 1002,
        DeleteLand = 1003,
        
    }

    public static class UserRoles
    {
        public const int Admin = 1;
        public const long FarmClerk = 2;
        public const long FarmSupervisor = 3;
        public const int LandClerk = 4;
        public const int LandSupervisor = 5;
        public const int LandAdmin = 6;
        public const int LandCertificateIssuer = 7;
        public const int MnEExpert = 8;
        public const int MnESupervisor = 9;
        public const int MnEDataEncoder = 10;
        public const int ConfigurationAdmin = 11;
    }

    public class UserViewModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int Status { get; set; }
        public string[] Roles { get; set; }
    }

    public class UserDetialViewModel
    {
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