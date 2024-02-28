using Interview.UI.Data;
using Interview.UI.Services.DAL;
using Microsoft.EntityFrameworkCore;
using Interview.UI.Services.Stats;
using Interview.UI.Models.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interview.Entities;
using Interview.UI.Models.Dashboard;
using Interview.UI.Models;

namespace Interview.UI.Tests.Services.Stats
{

    [TestClass]
    public class IStatsTests : TestBase
    {

        #region Declarations

        private IStats _statsManager;

        #endregion

        #region Setup / Teardown

        [TestInitialize]
        public void Init()
        {

            _statsManager = new StatsManager();

        }

        #endregion

        #region Public GetInterviewStats Test Methods

        [TestMethod]
        public void GetInterviewStats_SingleProcess()
        {

            VmInterviewCounts result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            int completed = 10;
            int remaining = 20;
            int interviewDays;

            processes.Add(process);
            process.Interviews = GetInterviews(process.Id, completed, remaining);
            interviewDays = GetInterviewDays(processes);
            result = _statsManager.GetInterviewCounts(processes);

            Assert.IsTrue(result.TotalInterviews == completed + remaining);
            Assert.IsTrue(result.CompletedInterviews == completed);
            Assert.IsTrue(result.RemainingInterviews == remaining);
            Assert.IsTrue(result.InterviewDays == interviewDays);

        }

        [TestMethod]
        public void GetInterviewStats_MultipleProcesses()
        {

            VmInterviewCounts result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            int numberProcesses = 3;
            int completed = 10;
            int remaining = 20;
            int interviewDays;

            for (int i = 0; i < numberProcesses; i++)
            {

                process = (Process)GetEntity<Process>(true);
                process.Interviews = GetInterviews(process.Id, completed, remaining);
                processes.Add(process);

            }
            interviewDays = GetInterviewDays(processes);

            result = _statsManager.GetInterviewCounts(processes);

            Assert.IsTrue(result.TotalInterviews == (completed + remaining) * numberProcesses);
            Assert.IsTrue(result.CompletedInterviews == completed * numberProcesses);
            Assert.IsTrue(result.RemainingInterviews == remaining * numberProcesses);
            Assert.IsTrue(result.InterviewDays == interviewDays);

        }

        #endregion

        #region Public GetCandiateEquityStats Test Methods

        [TestMethod]
        public void GetCandiateEquityStats_SingleProcess()
        {

            List<VmEquityStat> result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;

            processes.Add(process);
            foreach (Process thisProcess in processes) {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++) {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

            }

            result = _statsManager.GetCandiateEquityStats(processes, equities, Constants.EnglishCulture);

            Assert.IsTrue(result.Count == equities.Count);
            Assert.IsTrue(Math.Round(((double)result.Sum(x => x.Percentage)), 0) == 100);

        }

        [TestMethod]
        public void GetCandiateEquityStats_MultipleProcesses()
        {

            List<VmEquityStat> result = null;
            List<Process> processes = new List<Process>();
            Process process = null;
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberProcesses = 3;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;

            for (int i = 0; i < numberProcesses; i++)
            {
                process = (Process)GetEntity<Process>(true);
                processes.Add(process);
            }

            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

            }

            result = _statsManager.GetCandiateEquityStats(processes, equities, Constants.EnglishCulture);

            Assert.IsTrue(result.Count == equities.Count);
            Assert.IsTrue(Math.Round(((double)result.Sum(x => x.Percentage)), 0) == 100);

        }

        #endregion

        #region Public GetBoartdMemberEquityStats Test Methods

        [TestMethod]
        public void GetBoartdMemberEquityStats_SingleProcess()
        {

            List<VmEquityStat> result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;

            processes.Add(process);
            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

            }

            result = _statsManager.GetBoartdMemberEquityStats(processes, equities, Constants.EnglishCulture);

            Assert.IsTrue(result.Count == equities.Count);
            Assert.IsTrue(Math.Round(((double)result.Sum(x => x.Percentage)), 0) == 100);

        }

        [TestMethod]
        public void GetBoartdMemberEquityStats_MultipleProcesses()
        {

            List<VmEquityStat> result = null;
            List<Process> processes = new List<Process>();
            Process process = null;
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberProcesses = 3;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;

            for (int i = 0; i < numberProcesses; i++)
            {
                process = (Process)GetEntity<Process>(true);
                processes.Add(process);
            }

            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

            }

            result = _statsManager.GetBoartdMemberEquityStats(processes, equities, Constants.EnglishCulture);

            Assert.IsTrue(result.Count == equities.Count);
            Assert.IsTrue(Math.Round(((double)result.Sum(x => x.Percentage)), 0) == 100);

        }

        #endregion

        #region Public GetInterviewerAndLeadEquityStatsForInterview Test Methods

        [TestMethod]
        public void GetInterviewerAndLeadEquityStatsForInterview_SingleProcess()
        {

            List<VmEquityStat> result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            processes.Add(process);
            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetInterviewerAndLeadEquityStatsForInterviews(processes, equities, Constants.EnglishCulture);

            Assert.IsTrue(result.Count == equities.Count);

        }

        [TestMethod]
        public void GetInterviewerAndLeadEquityStatsForInterview_MultipleProcesses()
        {

            List<VmEquityStat> result = null;
            List<Process> processes = new List<Process>();
            Process process = null;
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberProcesses = 3;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            for (int i = 0; i < numberProcesses; i++)
            {
                process = (Process)GetEntity<Process>(true);
                processes.Add(process);
            }

            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetInterviewerAndLeadEquityStatsForInterviews(processes, equities, Constants.EnglishCulture);

            Assert.IsTrue(result.Count == equities.Count);

        }

        #endregion

        #region Public GetCandidateEquityStatsEquityStatsForInterview Test Methods

        [TestMethod]
        public void GetCandidateEquityStatsEquityStatsForInterview_SingleProcess()
        {

            List<VmEquityStat> result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            processes.Add(process);
            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetCandidateEquityStatsEquityStatsForInterviews(processes, equities, Constants.EnglishCulture);

            Assert.IsTrue(result.Count == equities.Count);

        }

        [TestMethod]
        public void GetCandidateEquityStatsEquityStatsForInterview_MultipleProcesses()
        {

            List<VmEquityStat> result = null;
            List<Process> processes = new List<Process>();
            Process process = null;
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberProcesses = 3;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            for (int i = 0; i < numberProcesses; i++)
            {
                process = (Process)GetEntity<Process>(true);
                processes.Add(process);
            }

            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetCandidateEquityStatsEquityStatsForInterviews(processes, equities, Constants.EnglishCulture);

            Assert.IsTrue(result.Count == equities.Count);

        }

        #endregion

        #region Public GetEeCandidatesForInterviews Test Methods

        [TestMethod]
        public void GetEeCandidatesForInterviews_SingleProcess()
        {

            List<VmEeCandidate> result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            processes.Add(process);
            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetEeCandidatesForInterviews(processes, equities, Constants.EnglishCulture);


        }

        #endregion

        #region Public GetInterviewStats Test Methods

        [TestMethod]
        public void GetInterviewStatsDailyView_SingleProcess()
        {

            List<VmDashboardItem> result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            processes.Add(process);
            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetDashboardItems(processes, equities, Constants.EnglishCulture, VmPeriodOfTimeTypes.Daily);

        }

        [TestMethod]
        public void GetInterviewStatsDailyView_MultipleProcesses()
        {

            List<VmDashboardItem> result = null;
            List<Process> processes = new List<Process>();
            Process process = null;
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberProcesses = 3;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            for (int i = 0; i < numberProcesses; i++)
            {
                process = (Process)GetEntity<Process>(true);
                processes.Add(process);
            }

            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetDashboardItems(processes, equities, Constants.EnglishCulture, VmPeriodOfTimeTypes.Daily);

        }

        [TestMethod]
        public void GetInterviewStatsMonthlyView_SingleProcess()
        {

            List<VmDashboardItem> result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            processes.Add(process);
            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetDashboardItems(processes, equities, Constants.EnglishCulture, VmPeriodOfTimeTypes.Monthly);

        }

        [TestMethod]
        public void GetInterviewStatsMonthlyView_MultipleProcesses()
        {

            List<VmDashboardItem> result = null;
            List<Process> processes = new List<Process>();
            Process process = null;
            List<Equity> equities = GetEquities();
            Equity equity = null;
            int numberProcesses = 3;
            int numberCandidates = 5;
            int numberNonCandates = 10;
            int numberEquitiesPerRoleUser = 3;
            int numberInterviewUsers = 3;
            int completed = 10;
            int remaining = 20;

            for (int i = 0; i < numberProcesses; i++)
            {
                process = (Process)GetEntity<Process>(true);
                processes.Add(process);
            }

            foreach (Process thisProcess in processes)
            {

                // Add Role Users to Processes
                for (int i = 0; i < numberCandidates; i++)
                {
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, RoleUserTypes.Candidate));
                }
                for (int i = 0; i < numberNonCandates; i++)
                {
                    RoleUserTypes roleUserType = GetNonCandiateRoleUserType();
                    Assert.IsTrue(roleUserType != RoleUserTypes.Candidate);
                    thisProcess.RoleUsers.Add(GetRoleUser(thisProcess.Id, roleUserType));
                }

                // Add RoleUserEquities to RoleUsers
                foreach (RoleUser roleUser in thisProcess.RoleUsers)
                {
                    for (int i = 0; i < numberEquitiesPerRoleUser; i++)
                    {
                        equity = GetRandomEquity(equities);
                        roleUser.RoleUserEquities.Add(GetRoleUserEquity(roleUser.Id, equity.Id));
                    }
                }

                // Add Interviews
                thisProcess.Interviews = GetInterviews(process.Id, completed, remaining);
                foreach (Entities.Interview interview in thisProcess.Interviews)
                {
                    for (int i = 0; i < numberInterviewUsers; i++)
                    {
                        RoleUser roleUser = GetRandomRoleUser(thisProcess.RoleUsers);
                        interview.InterviewUsers.Add(GetInterviewUser(interview.Id, roleUser.Id));
                    }
                }

            }

            result = _statsManager.GetDashboardItems(processes, equities, Constants.EnglishCulture, VmPeriodOfTimeTypes.Monthly);

        }

        #endregion

        #region Private Entity Methods

        private List<Entities.Interview> GetInterviews(Guid processId, int completed, int remaining)
        {

            List<Entities.Interview> result = new List<Entities.Interview>();
            Entities.Interview interview = null;

            // Completed
            for (int i = 0; i < completed; i++)
            {
                interview = (Entities.Interview)GetEntity<Entities.Interview>(true);
                interview.ProcessId = processId;
                interview.Status = InterviewStates.Booked;
                interview.StartDate = DateTime.Now.AddDays(-1);
                result.Add(interview);
            }

            // Remaing
            for (int i = 0; i < remaining; i++)
            {
                interview = (Entities.Interview)GetEntity<Entities.Interview>(true);
                interview.ProcessId = processId;
                interview.StartDate = DateTime.Now.AddDays(GetRandomNumber(30));
                result.Add(interview);
            }

            return result;

        }

        private List<Equity> GetEquities()
        {

            List<Equity> result = new List<Equity>();

            result.Add(GetEquity(Guid.NewGuid(), "Woman", "Femme", "Woman", "", ""));
            result.Add(GetEquity(Guid.NewGuid(), "Indigenous", "Personne autochtone", "Indigenous Person", "", ""));
            result.Add(GetEquity(Guid.NewGuid(), "Disability", "Personne en situation de handicap", "Person with a Disability", "", ""));
            result.Add(GetEquity(Guid.NewGuid(), "Racialized", "Membre d’un groupe racialisé", "Member of a Racialized group", "", ""));
            result.Add(GetEquity(Guid.NewGuid(), "SOGIE", "Orientation sexuelle, identité et expression du genre (OSIEG)", "Sexual Orientation, Gender Identity and Expression (SOGIE)", "", ""));

            return result;

        }

        private Equity GetEquity(Guid id, string name, string nameFR, string nameEN, string viewFR, string viewEN)
        {

            return new Equity()
            {
                Id = id,
                Name = name,
                NameFR = nameFR,
                NameEN = nameEN,
                ViewFR = viewFR,
                ViewEN = viewEN
            };

        }

        private RoleUser GetRoleUser(Guid processId, RoleUserTypes roleUserType)
        {

            RoleUser result = (RoleUser)GetEntity<RoleUser>(true);

            result.ProcessId = processId;
            result.RoleUserType = roleUserType;

            return result;

        }

        private RoleUserTypes GetNonCandiateRoleUserType()
        {

            RoleUserTypes result;
            Array values = Enum.GetValues(typeof(RoleUserTypes));  
            Random random = new Random();

            result = (RoleUserTypes)values.GetValue(random.Next(values.Length));

            if (result == RoleUserTypes.Candidate)
                result = GetNonCandiateRoleUserType();

            return result;

        }

        private RoleUserTypes GetRandomRoleUserType()
        {

            RoleUserTypes result;
            Array values = Enum.GetValues(typeof(RoleUserTypes));
            Random random = new Random();

            result = (RoleUserTypes)values.GetValue(random.Next(values.Length));

            return result;

        }

        private Equity GetRandomEquity(List<Equity> equities)
        {

            Equity result = null;
            Random random = new Random();

            result = equities[random.Next(equities.Count -1)];

            return result;

        }

        private RoleUserEquity GetRoleUserEquity(Guid roleUserId, Guid equityId)
        {

            RoleUserEquity result = new RoleUserEquity()
            {
                RoleUserId = roleUserId,
                EquityId = equityId
            };

            return result;

        }

        private RoleUser GetRandomRoleUser(List<RoleUser> roleUsers)
        {

            RoleUser result = null;
            Random random = new Random();

            result = roleUsers[random.Next(roleUsers.Count- 1)];

            return result;

        }

        private InterviewUser GetInterviewUser(Guid interviewId, Guid roleUserId)
        {

            InterviewUser result = (InterviewUser)GetEntity<InterviewUser>(true);

            result.InterviewId = interviewId;
            result.RoleUserId = roleUserId;
            result.RoleUserType = GetRandomRoleUserType();

            return result;

        }

        #endregion

        #region Private Assertion Methods

        private int GetInterviewDays(List<Process> processs)
        {

            int result = 0;
            Dictionary<string, int> dict = new Dictionary<string, int>();
            string dateString;

            foreach (Process process in processs)
            {
                foreach (Entities.Interview interview in process.Interviews)
                {
                    dateString = interview.StartDate.ToString("dd/MM/yyyy");
                    if (dict.ContainsKey(dateString))
                    {
                        dict[dateString]++;
                    }
                    else
                    {
                        dict.Add(dateString, 1);
                    }
                }
            }
            result = dict.Count;

            return result;

        }


        #endregion

    }

}
