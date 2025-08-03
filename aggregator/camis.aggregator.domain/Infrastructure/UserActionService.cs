using camis.aggregator.data.Entities;
using camis.aggregator.domain.Admin;
using System;
using System.Linq;


namespace camis.aggregator.domain.Infrastructure
{
    public interface IUserActionService
    {
        UserAction AddUserAction(UserSession session, UserActionType actionType);
    }

    public class UserActionService : IUserActionService
    {
        private readonly aggregatorContext _aggregatorContext;

        public UserActionService(aggregatorContext camisContext)
        {
            _aggregatorContext = camisContext;
        }

        public UserAction GetUserAction(long aid)
        {
            var ac = _aggregatorContext.UserAction.Where(a => a.Id == aid);
            if (ac.Any())
                return ac.First();
            return null;
        }
        public UserAction AddUserAction(UserSession session, UserActionType type)
        {
            var actionType = GetActionType((int) type);

            var user = _aggregatorContext.User.First(u => u.Username.Equals(session.Username));

            var action = new UserAction
            {
                Timestamp = DateTime.Now.Ticks,
                Username = user.Username,
                ActionTypeId=(int)type
                
            };
            _aggregatorContext.UserAction.Add(action);
            _aggregatorContext.SaveChanges();

            return action;
        }

        private ActionType GetActionType(int id)
        {
            return _aggregatorContext.ActionType.First(at => at.Id == id);
        }
    }
}