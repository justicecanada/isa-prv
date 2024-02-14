using Interview.Entities;
using Interview.UI.Models;
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

        public List<VmEquityStat> GetCandiateEquityStats(List<Process> processes, List<Equity> equities, string cultureName)
        {

            List<VmEquityStat> result = new List<VmEquityStat>();
            List<RoleUser> filteredRoleUsers = processes.SelectMany(x => x.RoleUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate)).ToList();
            int totalUserRolesForCandidate = filteredRoleUsers.Sum(x => x.RoleUserEquities.Count);
            int countForEquity;

            foreach (Equity equity in equities)
            {
                countForEquity = filteredRoleUsers.SelectMany(x => x.RoleUserEquities).Where(x => x.EquityId == equity.Id).Count();
                result.Add(new VmEquityStat()
                {
                    Description = cultureName == Constants.EnglishCulture ? equity.NameEN : equity.NameFR,
                    Total = totalUserRolesForCandidate,
                    Count = countForEquity,
                    Percentage = countForEquity == 0 ? 0 : Math.Round(((double)countForEquity / (double)totalUserRolesForCandidate) * 100, 2)
                });
            }

            return result;

        }

        #endregion

    }

}
