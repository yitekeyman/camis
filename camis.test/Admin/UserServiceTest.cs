using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace intapscamis.camis.test.Admin
{
    [TestFixture]
    public class UserServiceTest
    {
        private CamisContext _camisContext;

        [OneTimeSetUp]
        public void SetUp()
        {
            var dbOption = new DbContextOptionsBuilder<CamisContext>();

            dbOption.UseNpgsql("Server=map-server;Database=camis;Username=postgres;Password=toor");

            _camisContext = new CamisContext(dbOption.Options);
        }

        [Test]
        public void CanRegisterUser()
        {
            var userVm = new RegisterViewModel
            {
                Username = "Intaps",
                Password = "spatni",
                PhoneNo = "0910616253",
                FullName = "INTAPS Software Enginering",
                Roles = new[] {1}
            };

            var session = new UserSession
            {
                Username = "admin"
            };

            var facade = new UserFacade(new UserService(_camisContext, new UserActionService(_camisContext)));
            facade.RegisterUser(session, userVm);
        }
    }
}