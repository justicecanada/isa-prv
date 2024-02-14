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

        public List<VmEquityStat> GetBoartdMemberEquityStats(List<Process> processes, List<Equity> equities, string cultureName)
        {

            List<VmEquityStat> result = new List<VmEquityStat>();
            List<RoleUser> filteredRoleUsers = processes.SelectMany(x => x.RoleUsers.Where(x => x.RoleUserType == RoleUserTypes.Interviewer || x.RoleUserType == RoleUserTypes.Lead)).ToList();
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

        public List<VmEquityStat> GetInterviewerAndLeadEquityStatsForInterviews(List<Process> processes, List<Equity> equities, string cultureName)
        {

            List<VmEquityStat> result = new List<VmEquityStat>();
            List<RoleUserEquity> roleUserEquities = processes.SelectMany(x => x.RoleUsers.SelectMany(x => x.RoleUserEquities)).ToList();
            int totalInterviews = processes.Sum(x => x.Interviews.Count);
            int countForEquity;

            foreach (Equity equity in equities)
            {
                countForEquity = processes.SelectMany(x => x.Interviews.SelectMany(x => x.InterviewUsers.Where(y => (y.RoleUserType == RoleUserTypes.Interviewer || y.RoleUserType == RoleUserTypes.Lead) &&
                    roleUserEquities.Where(a => a.RoleUserId == y.UserId && a.EquityId == equity.Id).Count() > 0))).Count();
                result.Add(new VmEquityStat()
                {
                    Description = cultureName == Constants.EnglishCulture ? equity.NameEN : equity.NameFR,
                    Total = totalInterviews,
                    Count = countForEquity,
                    Percentage = countForEquity == 0 ? 0 : Math.Round(((double)countForEquity / (double)totalInterviews) * 100, 2)
                });

            }

            return result;

        }

        public List<VmEquityStat> GetCandidateEquityStatsEquityStatsForInterviews(List<Process> processes, List<Equity> equities, string cultureName)
        {

            List<VmEquityStat> result = new List<VmEquityStat>();
            List<RoleUserEquity> roleUserEquities = processes.SelectMany(x => x.RoleUsers.SelectMany(x => x.RoleUserEquities)).ToList();
            int totalInterviews = processes.Sum(x => x.Interviews.Count);
            int countForEquity;

            foreach (Equity equity in equities)
            {
                countForEquity = processes.SelectMany(x => x.Interviews.SelectMany(x => x.InterviewUsers.Where(y => (y.RoleUserType == RoleUserTypes.Candidate) &&
                    roleUserEquities.Where(a => a.RoleUserId == y.UserId && a.EquityId == equity.Id).Count() > 0))).Count();
                result.Add(new VmEquityStat()
                {
                    Description = cultureName == Constants.EnglishCulture ? equity.NameEN : equity.NameFR,
                    Total = totalInterviews,
                    Count = countForEquity,
                    Percentage = countForEquity == 0 ? 0 : Math.Round(((double)countForEquity / (double)totalInterviews) * 100, 2)
                });

            }

            return result;

        }

        #endregion

    }

}
