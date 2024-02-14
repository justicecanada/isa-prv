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

            VmInterviewStats result = null;
            List<Process> processes = new List<Process>();
            var process = (Process)GetEntity<Process>(true);
            int completed = 10;
            int remaining = 20;
            int interviewDays;

            processes.Add(process);
            process.Interviews = GetInterviews(process.Id, completed, remaining);
            interviewDays = GetInterviewDays(processes);
            result = _statsManager.GetInterviewStats(processes);

            Assert.IsTrue(result.TotalInterviews == completed + remaining);
            Assert.IsTrue(result.CompletedInterviews == completed);
            Assert.IsTrue(result.RemainingInterviews == remaining);
            Assert.IsTrue(result.InterviewDays == interviewDays);

        }

        [TestMethod]
        public void GetInterviewStats_MultipleProcesses()
        {

            VmInterviewStats result = null;
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
            
            result = _statsManager.GetInterviewStats(processes);

            Assert.IsTrue(result.TotalInterviews == (completed + remaining) * numberProcesses);
            Assert.IsTrue(result.CompletedInterviews == completed * numberProcesses);
            Assert.IsTrue(result.RemainingInterviews == remaining * numberProcesses);
            Assert.IsTrue(result.InterviewDays == interviewDays);

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
                interview.Status = InterviewStates.Reserve;
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
