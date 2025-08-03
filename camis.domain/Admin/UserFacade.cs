using System.Collections.Generic;
using intapscamis.camis.domain.Infrastructure;

namespace intapscamis.camis.domain.Admin
{
    public interface IUserFacade
    {
        void LoginUser(UserSession session, LoginViewModel loginView);
        void RegisterUser(UserSession session, RegisterViewModel registerViewModel);
        void ChangePassword(UserSession session, string username, string oldPassword, string newPassword);
        IList<UserDetialViewModel> GetAllUsers(UserSession session);
        void DeactivateUser(UserSession session, string username);
        void ActivateUser(UserSession session, string username);
        void UpdateUser(UserSession session, UserViewModel userModel);
        void ResetPassword(UserSession getSession, string resetedUsername, string newPassword);
        bool CheckUser(UserSession userSession, string username);
        IList<UserActionViewModel> GetAllAction(UserSession getSession);
        IList<UserViewModel> GetUsers(UserSession userSession, string query);
        void AddUserRole(UserSession userSession, string username, int[] roles);

        object GetRoles(UserSession session);
    }

    public class UserFacade : IUserFacade
    {
        private readonly IUserService _userService;

        public UserFacade(IUserService service)
        {
            _userService = service;
        }

        public void LoginUser(UserSession userSession, LoginViewModel loginView)
        {
            _userService.SetSession(userSession);
            _userService.LoginUser(loginView);
        }

        public void RegisterUser(UserSession userSession, RegisterViewModel registerViewModel)
        {
            _userService.SetSession(userSession);
            _userService.RegisterUser(registerViewModel);
        }

        public void ChangePassword(UserSession userSession, string username, string oldPassword, string newPassword)
        {
            _userService.SetSession(userSession);
            _userService.ChangePasswrod(username, newPassword, oldPassword);
        }

        public void ResetPassword(UserSession userSession, string username, string newPassword)
        {
            _userService.SetSession(userSession);
            _userService.ResetPassword(username, newPassword);
        }

        public IList<UserViewModel> GetUsers(UserSession userSession, string query)
        {
            _userService.SetSession(userSession);

            return _userService.GetUsers(query);
        }

        public object GetRoles(UserSession session)
        {
            _userService.SetSession(session);

            return _userService.GetRoles();
        }

        public IList<UserDetialViewModel> GetAllUsers(UserSession userSession)
        {
            _userService.SetSession(userSession);

            return _userService.GetAllUsers();
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
    }
}