using System.Collections.Generic;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Infrastructure;

namespace intapscamis.camis.domain.Admin
{
    public interface IUserFacade
    {
        LoginReturnViewModel LoginUser(UserSession session, LoginViewModel loginView);
        void RegisterUser(UserSession session, RegisterViewModel registerViewModel);
        void ChangePassword(UserSession session, string username, string oldPassword, string newPassword);
        IList<UserDetialViewModel> GetAllUsers(UserSession session, int status);
        void DeactivateUser(UserSession session, string username);
        void ActivateUser(UserSession session, string username);
        void UpdateUser(UserSession session, UserViewModel userModel);
        void ResetPassword(UserSession getSession, string resetUsername, string newPassword);
        bool CheckUser(UserSession userSession, string username);
        IList<UserActionViewModel> GetAllAction(UserSession getSession);
        IList<UserViewModel> GetUsers(UserSession userSession, string query, int status);
        void AddUserRole(UserSession userSession, string username, int[] roles);

        object GetRoles(UserSession session);
        SingleUserDetailsViewModel GetSingleUserDetails(UserSession userSession, string username);
        DashboardViewModel GetDashboard(UserSession userSession);
        IList<SysConfig> GetAllSysConfig(UserSession userSession);
        void EditSysConfig(UserSession userSession,SysConfig sysConfig);
        SysConfig GetSysConfig(UserSession userSession,int id);
        SysConfig GetSysConfigByName(UserSession userSession,string name);
    }

    public class UserFacade : IUserFacade
    {
        private readonly IUserService _userService;

        public UserFacade(IUserService service)
        {
            _userService = service;
        }

        public LoginReturnViewModel LoginUser(UserSession userSession, LoginViewModel loginView)
        {
            _userService.SetSession(userSession);
            return _userService.LoginUser(loginView);
        }

        public void RegisterUser(UserSession userSession, RegisterViewModel registerViewModel)
        {
            _userService.SetSession(userSession);
            _userService.RegisterUser(registerViewModel);
        }

        public void ChangePassword(UserSession userSession, string username, string oldPassword, string newPassword)
        {
            _userService.SetSession(userSession);
            _userService.ChangePassword(username, oldPassword, newPassword);
        }

        public void ResetPassword(UserSession userSession, string username, string newPassword)
        {
            _userService.SetSession(userSession);
            _userService.ResetPassword(username, newPassword);
        }

        public IList<UserViewModel> GetUsers(UserSession userSession, string query, int status)
        {
            _userService.SetSession(userSession);

            return _userService.GetUsers(query, status);
        }

        public object GetRoles(UserSession session)
        {
            _userService.SetSession(session);

            return _userService.GetRoles();
        }

        public IList<UserDetialViewModel> GetAllUsers(UserSession userSession, int status)
        {
            _userService.SetSession(userSession);

            return _userService.GetAllUsers(status);
        }

        public IList<UserActionViewModel> GetAllAction(UserSession userSession)
        {
            _userService.SetSession(userSession);
            return _userService.GetAllActions();
        }

        public void AddUserRole(UserSession userSession, string username, int[] roles)
        {
            _userService.SetSession(userSession);
            _userService.AddUserRole(username, roles);
        }

        public void DeactivateUser(UserSession userSession, string username)
        {
            _userService.SetSession(userSession);
            _userService.DeactivateUser(username);
        }

        public void ActivateUser(UserSession userSession, string username)
        {
            _userService.SetSession(userSession);
            _userService.ActivateUser(username);
        }

        public void UpdateUser(UserSession userSession, UserViewModel userVm)
        {
            _userService.SetSession(userSession);
            _userService.UpdateUser(userVm);
        }

        public bool CheckUser(UserSession userSession, string username)
        {
            _userService.SetSession(userSession);

            return _userService.CheckUser(username);
        }

        public SingleUserDetailsViewModel GetSingleUserDetails(UserSession userSession, string username)
        {
            _userService.SetSession(userSession);
            return _userService.GetSingleUserDetails(username);
        }

        public DashboardViewModel GetDashboard(UserSession userSession)
        {
            _userService.SetSession(userSession);
            return _userService.GetDashboard();
        }

        public IList<SysConfig> GetAllSysConfig(UserSession userSession)
        {
            _userService.SetSession(userSession);
            return _userService.GetAllSysConfig();
        }

        public void EditSysConfig(UserSession userSession, SysConfig sysConfig)
        {
            _userService.SetSession(userSession);
            _userService.EditSysConfig(sysConfig);
        }

        public SysConfig GetSysConfig(UserSession userSession, int id)
        {
            _userService.SetSession(userSession);
            return _userService.GetSysConfig(id);
        }

        public SysConfig GetSysConfigByName(UserSession userSession, string name)
        {
            _userService.SetSession(userSession);
            return _userService.GetSysConfigByName(name);
        }
    }
}