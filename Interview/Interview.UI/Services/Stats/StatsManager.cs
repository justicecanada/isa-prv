using Interview.Entities;
using Interview.UI.Models.Stats;

namespace Interview.UI.Services.Stats
{
    
    public class StatsManager : IStats
    {

        #region Declarations


        #endregion

        #region Constructgors


        #endregion

        #region Public Inteface Methods

        public VmInterviewStats GetInterviewStats(List<Process> processes)
        {

            VmInterviewStats result = new VmInterviewStats();

            result.TotalInterviews = processes.Sum(x => x.Interviews.Count);
            result.CompletedInterviews = processes.SelectMany(x => x.Interviews.Where(x => x.Status == InterviewStates.Reserve && 
                x.StartDate.Date < DateTime.Now)).Count();
            result.RemainingInterviews = result.TotalInterviews - result.CompletedInterviews;
            result.InterviewDays = processes.SelectMany(x => x.Interviews.Select(y => y.StartDate.Date.ToString("dd/MM/yyyy")).Distinct()).Distinct().Count();

            return result;

        }

        #endregion

    }

}
