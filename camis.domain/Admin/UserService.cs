using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Exeptions;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Workflows.Models;
using Microsoft.EntityFrameworkCore;

namespace intapscamis.camis.domain.Admin
{
    public interface IUserService
    {
        void SetSession(UserSession session);

        LoginReturnViewModel LoginUser(LoginViewModel loginView);

        void RegisterUser(RegisterViewModel user);

        void RevokeUserRole(string username, int[] roles);
        void AddUserRole(string username, int[] roles);

        IList<UserDetialViewModel> GetAllUsers(int status);
        bool CheckUser(string username);
        void UpdateUser(UserViewModel userModle);
        void ActivateUser(string username);
        void DeactivateUser(string username);
        IList<UserActionViewModel> GetAllActions();
        User GetUser(string username);
        IList<UserViewModel> GetUsers(string query, int status);
        bool HasRole(string username, int role);
        void ResetPassword(string username, string newPassword);
        void ChangePassword(string username, string oldPassword, string newPassword);
        object GetRoles();
        SingleUserDetailsViewModel GetSingleUserDetails(string username);
        DashboardViewModel GetDashboard();
    }

    public class UserService : IUserService
    {
        private readonly IUserActionService _actionService;
        private readonly CamisContext _context;
        private UserSession _userSession;

        public UserService(CamisContext context, IUserActionService actionService)
        {
            _context = context;
            _actionService = actionService;
        }

        public void SetSession(UserSession session)
        {
            _userSession = session;
        }

        public LoginReturnViewModel LoginUser(LoginViewModel loginView)
        {
            var hashPassword = loginView.Password.Hash();
            try
            {
                var ret = new LoginReturnViewModel();
                var dbUser = _context.User.First(u => u.Username == loginView.UserName && u.Password == hashPassword);
                if (dbUser.Status == 0) throw new AccessDeniedException("User Deactivated");

                _actionService.AddUserAction(new UserSession { Username = dbUser.Username }, UserActionType.Login);
                _context.SaveChanges();
                ret.UserName = dbUser.Username;
                ret.FullName = dbUser.FullName;
                return ret;
            }
            catch (InvalidOperationException e)
            {
                Console.Error.WriteLine(e);
                throw new AccessDeniedException("Invalid Username or Password, Please Try Again");
            }
        }

        public void RegisterUser(RegisterViewModel userVm)
        {
            var user = new User
            {
                Username = userVm.Username,
                FullName = userVm.FullName,
                Password = userVm.Password.Hash(),
                PhoneNo = userVm.PhoneNo,
                Email = userVm.Email,
                RegOn = DateTime.Now.Ticks
            };


            _context.User.Add(user);

            foreach (var ur in userVm.Roles)
            {
                var role = GetRole(ur);

                var userRole = new UserRole
                {
                    User = user,
                    Role = role
                };
                _context.UserRole.Add(userRole);
            }

            _context.SaveChanges(_userSession.Username, (int)UserActionType.Register);
        }

        public void ChangePassword(string username, string oldPassword, string newPassword)
        {
            var hashPassword = oldPassword.Hash();

            var user = GetUser(username);

            if (!user.Password.Equals(hashPassword))
                throw new AccessDeniedException("Incorrect Old Password");
            user.Password = newPassword.Hash();

            _context.User.Update(user);
            _context.SaveChanges(_userSession.Username, (int)UserActionType.ChangePassword);
        }

        public void ResetPassword(string username, string newPassword)
        {
            var user = GetUser(username);


            user.Password = newPassword.Hash();

            _context.SaveChanges(_userSession.Username, (int)UserActionType.ResetPassword);
        }

        public void AddUserRole(string username, int[] roles)
        {
            var user = GetUser(username);
            var userRoles = _context.UserRole.Where(u => u.UserId == user.Id).Select(ur => ur.Role.Id);
            foreach (var role in roles)
            {
                if (userRoles.Contains(role)) continue;
                var userRole = new UserRole { User = user, Role = GetRole(role) };
                _context.UserRole.Add(userRole);
            }

            _context.SaveChanges(_userSession.Username, (int)UserActionType.UpdateUser);
        }

        public void RevokeUserRole(string username, int[] roles)
        {
            var user = GetUser(username);


            foreach (var role in roles)
            {
                var userRole = _context.UserRole.First(ur => ur.UserId == user.Id && ur.Role.Id == role);

                _context.UserRole.Remove(userRole);
            }

            _context.SaveChanges(_userSession.Username, (int)UserActionType.RevokeRole);
        }

        public object GetRoles()
        {
            return _context.UserRole.Where(ur => ur.User.Username.Equals(_userSession.Username))
                .Select(ur => new { id = ur.RoleId, name = ur.Role.Name }).ToList();
        }

        public void DeactivateUser(string username)
        {
            var user = GetUser(username);

            user.Status = 0;
            _context.User.Update(user);
            _context.SaveChanges(_userSession.Username, (int)UserActionType.DeactivateUser);
        }

        public void ActivateUser(string username)
        {
            var user = GetUser(username);
            user.Status = 1;

            _context.User.Update(user);

            _context.SaveChanges(_userSession.Username, (int)UserActionType.ActivateUser);
        }

        public IList<UserViewModel> GetUsers(string filter, int status)
        {
            var users = _context.User.Where(u =>
                (u.Username.Contains(filter) || u.FullName.Contains(filter) || u.PhoneNo.Contains(filter) ||
                 u.Email.Contains(filter)) && u.Status == status).ToList();
            var userVms = new List<UserViewModel>();

            foreach (var user in users)
            {
                userVms.Add(new UserViewModel
                {
                    UserName = user.Username,
                    FullName = user.FullName,
                    PhoneNo = user.PhoneNo,
                    Email = user.Email,
                    Status = user.Status,
                    Roles = _context.UserRole.Where(u => u.UserId == user.Id)
                        .Select(ur => new LookUpModel { Id = ur.Role.Id, Name = ur.Role.Name }).ToArray()
                });
            }

            return userVms;
        }

        public IList<UserDetialViewModel> GetAllUsers(int status)
        {
            var users = _context.User.Where(u => u.Status == status).ToList();

            var userVms = new List<UserDetialViewModel>();

            foreach (var user in users)
            {
                var userVm = new UserDetialViewModel
                {
                    UserName = user.Username,
                    FullName = user.FullName,
                    PhoneNo = user.PhoneNo,
                    Email = user.Email,
                    Status = user.Status
                };
                userVm.Roles = _context.UserRole.Where(u => u.UserId == user.Id)
                    .Select(ur => new LookUpModel { Id = ur.Role.Id, Name = ur.Role.Name }).ToArray();


                var time = _context.UserAction.Where(u => u.Username == user.Username && u.ActionTypeId == 1)
                    .Max(ua => ua.Timestamp);
                var lastSeen = time ?? 0;
                var dt = new DateTime(lastSeen);
                userVm.LastSeen = string.Format("{0:G}", dt);
                var regDate = new DateTime(user.RegOn);
                userVm.RegOn = string.Format("{0:G}", regDate);

                userVms.Add(userVm);
            }

            return userVms;
        }


        public IList<UserActionViewModel> GetAllActions()
        {
            var actions = _context.UserAction.Where(a => a.ActionTypeId != 1).OrderByDescending(action => action.Id)
                .Take(500).ToList();

            var actionVms = new List<UserActionViewModel>();

            foreach (var userAction in actions)
            {
                var username = GetUser(userAction.Username).Username;
                var fullname = GetUser(userAction.Username).FullName;
                var actionType = GetActionType(userAction.ActionTypeId).Name;
                var lastAction = userAction.Timestamp ?? -1;
                var dt = new DateTime(lastAction);
                if (!actionType.Equals("Login"))
                {
                    actionVms.Add(GetUserAction(userAction.Id));
                }
            }

            return actionVms;
        }

        public void UpdateUser(UserViewModel userVm)
        {
            var user = GetUser(userVm.UserName);

            user.FullName = userVm.FullName;
            user.PhoneNo = userVm.PhoneNo;
            user.Email = userVm.Email;

            var userRoles = _context.UserRole.Where(u => u.UserId == user.Id);

            //Remove Previous roles
            foreach (var role in userRoles)
            {
                _context.UserRole.Remove(role);
            }

            _context.SaveChanges();


            foreach (var role in userVm.Roles)
            {
                var ur = _context.Role.First(r => r.Id == role.Id);

                var userRole = new UserRole { User = user, Role = ur };

                _context.UserRole.Add(userRole);
            }

            _context.User.Update(user);

            _context.SaveChanges(_userSession.Username, (int)UserActionType.UpdateUser);
        }

        public User GetUser(string username)
        {
            return _context.User.First(u => u.Username == username);
        }

        public bool HasRole(string username, int role)
        {
            var userId = GetUser(username).Id;
            var roleId = GetRole(role).Id;

            return _context.UserRole.Any(ur => ur.RoleId == roleId && ur.UserId == userId);
        }

        public bool CheckUser(string username)
        {
            return _context.User.Any(u => u.Username == username);
        }

        private ActionType GetActionType(int id)
        {
            return _context.ActionType.First(at => at.Id == id);
        }

        private UserAction GetUserAction(User user, UserActionType type)
        {
            var action = new UserAction
            {
                Timestamp = DateTime.Now.Ticks,
                Username = user.Username,
                ActionTypeId = (int)type,
            };


            return action;
        }

        private User GetUser(long id)
        {
            return _context.User.First(u => u.Id == id);
        }

        private Role GetRole(int role)
        {
            return _context.Role.First(r => r.Id == role);
        }

        public SingleUserDetailsViewModel GetSingleUserDetails(string username)
        {
            var ret = new SingleUserDetailsViewModel();
            var user = _context.User.First(u => u.Username == username);
            ret.UserName = user.Username;
            ret.FullName = user.FullName;
            ret.PhoneNo = user.PhoneNo;
            ret.Email = user.Email;
            ret.Status = user.Status;
            ret.Roles = _context.UserRole.Where(u => u.UserId == user.Id)
                .Select(ur => new LookUpModel { Id = ur.Role.Id, Name = ur.Role.Name }).ToArray();
            var time = _context.UserAction.Where(u => u.Username == user.Username && u.ActionTypeId == 1)
                .Max(ua => ua.Timestamp);
            var lastSeen = time ?? 0;
            var dt = new DateTime(lastSeen);
            ret.LastSeen = string.Format("{0:G}", dt);
            var regDate = new DateTime(user.RegOn);
            ret.RegOn = string.Format("{0:G}", regDate);
            var actions = _context.UserAction.Where(a => a.ActionTypeId != 1).OrderByDescending(action => action.Id)
                .Take(30).ToList();
            foreach (var action in actions)
            {
                ret.Actions.Add(GetUserAction(action.Id));
            }

            return ret;
        }

        private UserActionViewModel GetUserAction(long id)
        {
            var ret = new UserActionViewModel();
            var userAction = _context.UserAction.Where(a => a.Id == id).First();
            if (userAction != null)
            {
                ret.Id = userAction.Id;
                ret.UserName = userAction.Username;
                ret.FullName = GetUser(userAction.Username).FullName;
                ret.Action = GetActionType(userAction.ActionTypeId).Name;
                ret.ActionTime = string.Format("{0:G}", new DateTime(userAction.Timestamp ?? -1));
                if (userAction.Id > 0)
                    ret.AuditLog = GetAuditLog(userAction.Id);
            }

            return ret;
        }

        private AuditLogViewModel GetAuditLog(long userActionId)
        {
            var ret = new AuditLogViewModel();
            var audit = _context.AuditLog.Where(a => a.UserAction == userActionId).FirstOrDefault();
            var username = _context.UserAction.Where(a => a.Id == userActionId).First().Username;
            if (audit != null)
            {
                ret.Id = audit.Id;
                ret.UserAction = audit.UserAction ?? 0;
                ret.TableName = audit.TableName;
                ret.OldValues = audit.OldValues;
                ret.NewValues = audit.NewValues;
                ret.UserName = username;
            }

            return ret;
        }

        public DashboardViewModel GetDashboard()
        {
            return new DashboardViewModel
            {
                InvestmentDataStat = GetInvestmentDataStat(),
                IdentifiedParcels = GetParcelStat(LandStatus.Identified),
                PreparedParcel = GetParcelStat(LandStatus.Prepared),
                TransferredParcel = GetParcelStat(LandStatus.Transferred),
                IrrigatedParcel = GetIrrigatedParcel(),
                ParcelSuitableFor = GetParcelSuitableFor(),
            };
        }

        private InvestmentStat GetInvestmentDataStat()
        {
            return new InvestmentStat()
            {
                InvestedCapital = _context.Farm.Sum(i => i.InvestedCapital ?? 0),
                TotalInvestors = _context.FarmOperator.Count(),
                EthiopianOrigin = GetInvestorStat(3),
                ForeignOrigin = GetInvestorStat(2),
                Local = GetInvestorStat(1),
                InvestorCapital = _context.FarmOperator.Sum(i => i.Capital ?? 0)
            };
        }

        private InvestorStat GetInvestorStat(int originType)
        {
          
            string sql = @"SELECT 
               COALESCE(SUM(f.invested_capital), 0) as InvestedCapital 
        FROM frm.farm as f 
        INNER JOIN frm.farm_operator as fo ON fo.id = f.operator_id
        WHERE fo.origin_id = {0}";
            var result = _context.Database
                .SqlQueryRaw<InvestorCapitalResult>(sql, originType)
                .FirstOrDefault();
            return new InvestorStat()
            {
                TotalInvestors = _context.FarmOperator.Count(i => i.OriginId == originType),
                InvestorCapital = _context.FarmOperator.Where(i => i.OriginId == originType).Sum(i => i.Capital ?? 0),
                InvestmentCapital = result?.investedcapital??0
            };
        }

        private ParcelStat GetParcelStat(LandStatus landStatus)
        {
            var (totalParcels, totalArea) = GetParcelStatistics(landStatus);

            return new ParcelStat
            {
                ParcelStatus = new LookUpModel()
                {
                    Id = (int)landStatus,
                    Name = landStatus.ToString()
                },
                TotalParcels = totalParcels,
                TotalArea = totalArea
            };
        }
        private class InvestorCapitalResult
        {
            public double investedcapital { get; set; }
        }
        private (int TotalParcels, double TotalArea) GetParcelStatistics(LandStatus landStatus)
        {
            string sql =
                "SELECT COUNT(l.*) as TotalParcel, COALESCE(SUM(u.area), 0) as TotalArea  FROM lb.land as l INNER JOIN lb.land_upin as u ON u.land_id = l.id";
            if (landStatus == LandStatus.Prepared)
            {
                sql =
                    "SELECT COUNT(l.*) as TotalParcel, COALESCE(SUM(u.area), 0) as TotalArea  FROM lb.land as l INNER JOIN lb.land_upin as u ON u.land_id = l.id where l.land_type=2 or l.land_type=3";
            }

            if (landStatus == LandStatus.Transferred)
            {
                sql =
                    "SELECT COUNT(l.*) as TotalParcel, COALESCE(SUM(u.area), 0) as TotalArea  FROM lb.land as l INNER JOIN lb.land_upin as u ON u.land_id = l.id where  l.land_type=3";
            }

            var result = _context.Database
                .SqlQueryRaw<ParcelStatResult>(sql)
                .FirstOrDefault();

            return (result?.totalparcel ?? 0, result?.totalarea ?? 0);
        }


        public enum LandStatus
        {
            Identified = 1,
            Prepared = 2,
            Transferred = 3
        }

        private IList<ParcelStat> GetParcelSuitableFor()
        {
            var ret = new List<ParcelStat>();
            var landInvestment = _context.InverstmentType.ToList();
            foreach (var item in landInvestment)
            {
                string sql = @"SELECT COUNT(l.*) as TotalParcel, 
                                COALESCE(SUM(u.area), 0) as TotalArea 
                                FROM lb.land as l 
                                INNER JOIN lb.land_upin as u ON u.land_id = l.id 
		                        INNER JOIN lb.land_investment as lm ON l.id=lm.land_id
                                WHERE lm.investment = {0}";
                var result = _context.Database
                    .SqlQueryRaw<ParcelStatResult>(sql, item.Id)
                    .FirstOrDefault();
               ret.Add(new ParcelStat()
               {
                   ParcelStatus = new LookUpModel()
                   {
                       Id = item.Id,
                       Name = item.Name,
                   },
                   TotalParcels = result?.totalparcel ?? 0,
                   TotalArea = result?.totalarea ?? 0,
               });
            }

            return ret;
        }
        private ParcelStat GetIrrigatedParcel()
        {
                string sql = @"SELECT COUNT(l.*) as TotalParcel, 
                                       COALESCE(SUM(u.area), 0) as TotalArea 
                                FROM lb.land as l 
                                INNER JOIN lb.land_upin as u ON u.land_id = l.id 
		                        inner join lb.land_moisture as lm ON l.id=lm.land_id
                                WHERE lm.moisture = {0}";
                var result = _context.Database
                    .SqlQueryRaw<ParcelStatResult>(sql, 2)
                    .FirstOrDefault();
                return new ParcelStat()
                {
                    ParcelStatus = new LookUpModel()
                    {
                        Id = 2,
                        Name = "Irrigation",
                    },
                    TotalParcels = result?.totalparcel ?? 0,
                    TotalArea = result?.totalarea ?? 0,
                };

            
        }

        private class ParcelStatResult
        {
            public int totalparcel { get; set; }
            public double totalarea { get; set; }
        }
    }
}