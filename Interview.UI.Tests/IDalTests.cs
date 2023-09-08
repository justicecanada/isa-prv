using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Interview.UI.Tests
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
            Contest contest = (Contest)GetEntity<Contest>();

            result = await _dal.AddEntity(contest);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result != Guid.Empty);

        }

        [TestMethod]
        public async Task GetEntity()
        {

            EntityBase result = null;
            Guid id;
            Contest contest = (Contest)GetEntity<Contest>();

            // Add
            id = await _dal.AddEntity(contest);

            // Get
            result = await _dal.GetEntity<Contest>(id);
            Assert.IsTrue(result != null);
            Assert.IsTrue(result is Contest);

        }

        [TestMethod]
        public async Task GetEntityWithChildObjects()
        {

            Contest? result = null;
            Contest contest = (Contest)GetEntity<Contest>();
            Interview.Entities.Interview interview;
            InterviewUser interviewUser;
            for (int x = 0; x < 4; x++)
            {
                interview = (Interview.Entities.Interview)GetEntity<Interview.Entities.Interview>();
                contest.Interviews.Add(interview);
                for (int y = 0; y < x; y++)
                {
                    interviewUser = (InterviewUser)GetEntity<InterviewUser>();
                    interview.InterviewUsers.Add(interviewUser);
                }
            }
            Guid id;

            // Add
            id = await _dal.AddEntity(contest);

            // Get
            result = await _dal.GetEntity<Contest>(id, true) as Contest;
            Assert.IsTrue(result != null);
            Assert.IsTrue(result is Contest);
            for (int x = 0; x < 4; x++)
                Assert.IsTrue(result.Interviews[x].InterviewUsers.Count == x);

        }

        [TestMethod]
        public async Task UpdateEntity()
        {

            Contest? contest = (Contest)GetEntity<Contest>();
            Guid id;
            string newValue = "New value";

            // Add
            id = await _dal.AddEntity(contest);

            // Update
            contest.ContactName = newValue;
            await _dal.UpdateEntity(contest);

            // Get
            contest = await _dal.GetEntity<Contest>(id) as Contest;
            Assert.IsTrue(contest != null);
            Assert.IsTrue(contest.ContactName == newValue);

        }

        [TestMethod]
        public async Task DeleteEntity()
        {

            Contest? contest = (Contest)GetEntity<Contest>();
            Guid id;

            // Add
            id = await _dal.AddEntity(contest);

            // Delete
            await _dal.DeleteEntity<Contest>(id);

            // Get
            contest = await _dal.GetEntity<Contest>(id) as Contest;
            Assert.IsTrue(contest == null);

        }

        [TestMethod]
        public async Task DeleteEntityWithEntity()
        {

            Contest? contest = (Contest)GetEntity<Contest>();
            Guid id;

            // Add
            id = await _dal.AddEntity(contest);

            // Delete
            contest.Id = id;
            await _dal.DeleteEntity(contest);

            // Get
            contest = await _dal.GetEntity<Contest>(id) as Contest;
            Assert.IsTrue(contest == null);

        }

        #endregion

        #region Private Methods



        #endregion

    }

}