using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Interview.UI.Tests.Services.DAL
{

    [TestClass]
    public class IDalTests : TestBase
    {

        #region Declarations

        private IDal _dal;
        private string _environment = "Development";        // TODO: Move this to appsettings.

        #endregion

        #region Setup / teardown

        [TestInitialize]
        public void Init()
        {

            var options = new DbContextOptionsBuilder<SqlContext>().Options;
            var connStr = GetConnectionString();
            var context = new SqlContext(options, connStr);
            _dal = new DalSql(context);

        }

        private string GetConnectionString()
        {

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{_environment}.json")
                .Build();

            return config.GetConnectionString("SQLConnectionString");

        }

        #endregion

        #region Public Test Methods

        [TestMethod]
        public async Task AddEntity()
        {

            Guid result;
            Process process = (Process)GetEntity<Process>();

            result = await _dal.AddEntity<Process>(process);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result != Guid.Empty);

        }

        [TestMethod]
        public async Task GetEntity()
        {

            EntityBase result = null;
            Guid id;
            Process process = (Process)GetEntity<Process>();

            // Add
            id = await _dal.AddEntity<Process>(process);

            // Get
            result = await _dal.GetEntity<Process>(id);
            Assert.IsTrue(result != null);
            Assert.IsTrue(result is Process);

        }

        [TestMethod]
        public async Task GetEntityWithChildObjects()
        {

            Process? result = null;
            Process process = (Process)GetEntity<Process>();
            Entities.Interview interview;
            InterviewUser interviewUser;
            for (int x = 0; x < 4; x++)
            {
                interview = (Entities.Interview)GetEntity<Entities.Interview>();
                process.Interviews.Add(interview);
                for (int y = 0; y < x; y++)
                {
                    interviewUser = (InterviewUser)GetEntity<InterviewUser>();
                    interview.InterviewUsers.Add(interviewUser);
                }
            }
            Guid id;

            // Add
            id = await _dal.AddEntity<Process>(process);

            // Get
            result = await _dal.GetEntity<Process>(id, true) as Process;
            Assert.IsTrue(result != null);
            Assert.IsTrue(result is Process);
            for (int x = 0; x < 4; x++)
                Assert.IsTrue(result.Interviews[x].InterviewUsers.Count == x);

        }

        [TestMethod]
        public async Task UpdateEntity()
        {

            Process? process = (Process)GetEntity<Process>();
            Guid id;
            string newValue = "New value";

            // Add
            id = await _dal.AddEntity<Process>(process);

            // Update
            process.ContactName = newValue;
            await _dal.UpdateEntity(process);

            // Get
            process = await _dal.GetEntity<Process>(id) as Process;
            Assert.IsTrue(process != null);
            Assert.IsTrue(process.ContactName == newValue);

        }

        [TestMethod]
        public async Task DeleteEntity()
        {

            Process? process = (Process)GetEntity<Process>();
            Guid id;

            // Add
            id = await _dal.AddEntity<Process>(process);

            // Delete
            await _dal.DeleteEntity<Process>(id);

            // Get
            process = await _dal.GetEntity<Process>(id) as Process;
            Assert.IsTrue(process == null);

        }

        [TestMethod]
        public async Task DeleteEntityWithEntity()
        {

            Process? process = (Process)GetEntity<Process>();
            Guid id;

            // Add
            id = await _dal.AddEntity<Process>(process);

            // Delete
            process.Id = id;
            await _dal.DeleteEntity(process);

            // Get
            process = await _dal.GetEntity<Process>(id) as Process;
            Assert.IsTrue(process == null);

        }

        #endregion

        #region Public Many To Many Test Methods

        [TestMethod]
        public async Task AddManyToMany()
        {

            Process process = (Process)GetEntity<Process>();
            process.Id = await _dal.AddEntity<Process>(process);

            RoleUser roleUser = (RoleUser)GetEntity<RoleUser>();
            roleUser.ProcessId = process.Id;
            roleUser.Id = await _dal.AddEntity<RoleUser>(roleUser);
            RoleUser roleUser2 = (RoleUser)GetEntity<RoleUser>();
            roleUser2.ProcessId = process.Id;
            roleUser2.Id = await _dal.AddEntity<RoleUser>(roleUser2);

            Equity equity1 = (Equity)GetEntity<Equity>();
            equity1.Id = await _dal.AddEntity<Equity>(equity1);
            Equity equity2 = (Equity)GetEntity<Equity>();
            equity2.Id = await _dal.AddEntity<Equity>(equity2);

            RoleUserEquity roleUserEquity1 = new RoleUserEquity() { RoleUserId = roleUser.Id, EquityId = equity1.Id };
            await _dal.AddEntity<RoleUserEquity>(roleUserEquity1);
            RoleUserEquity roleUserEquity2 = new RoleUserEquity() { RoleUserId = roleUser.Id, EquityId = equity2.Id };
            await _dal.AddEntity<RoleUserEquity>(roleUserEquity2);
            RoleUserEquity roleUserEquity3 = new RoleUserEquity() { RoleUserId = roleUser2.Id, EquityId = equity1.Id };
            await _dal.AddEntity<RoleUserEquity>(roleUserEquity3);
            RoleUserEquity roleUserEquity4 = new RoleUserEquity() { RoleUserId = roleUser2.Id, EquityId = equity2.Id };
            await _dal.AddEntity<RoleUserEquity>(roleUserEquity4);

            List<RoleUser> getRoleUsers = await _dal.GetRoleUsersByProcessId(process.Id);
            foreach (var getRoleUser in getRoleUsers)
                Assert.IsTrue(getRoleUser.RoleUserEquities.Count == 2);

            await _dal.DeleteEntity(equity1);
            await _dal.DeleteEntity(equity2);

        }

        #endregion

        #region Private Methods



        #endregion

    }

}