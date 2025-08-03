using System;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;

namespace intapscamis.camis.domain.Infrastructure
{
    public interface IUserActionService
    {
        UserAction AddUserAction(UserSession session, UserActionType actionType);
    }

    public class UserActionService : IUserActionService
    {
        private readonly CamisContext _camisContext;

        public UserActionService(CamisContext camisContext)
        {
            _camisContext = camisContext;
        }

        public UserAction GetUserAction(long aid)
        {
            var ac = _camisContext.UserAction.Where(a => a.Id == aid);
            if (ac.Any())
                return ac.First();
            return null;
        }
        public UserAction AddUserAction(UserSession session, UserActionType type)
        {
            var actionType = GetActionType((int) type);

            var user = _camisContext.User.First(u => u.Username.Equals(session.Username));

            var action = new UserAction
            {
                Timestamp = DateTime.Now.Ticks,
                Username = user.Username,
                ActionTypeId=(int)type
                
            };
            _camisContext.UserAction.Add(action);
            _camisContext.SaveChanges();

            return action;
        }

        private ActionType GetActionType(int id)
        {
            return _camisContext.ActionType.First(at => at.Id == id);
        }
    }
}