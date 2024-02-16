using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.Dashboard;
using Interview.UI.Models.Stats;
using System.Text;

namespace Interview.UI.Services.Stats
{
    
    public class StatsManager : IStats
    {

        #region Declarations


        #endregion

        #region Constructgors


        #endregion

        #region Public Inteface Methods

        public VmInterviewCounts GetInterviewCounts(List<Process> processes)
        {

            VmInterviewCounts result = new VmInterviewCounts();

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

        public List<VmEeCandidate> GetEeCandidatesForInterviews(List<Process> processes, List<Equity> equities, string cultureName)
        {

            List<VmEeCandidate> result = new List<VmEeCandidate>();
            VmEeCandidate eeCandidate = null;
            List<RoleUser> filteredRoleUsers = processes.SelectMany(x => (x.RoleUsers.Where(x => x.RoleUserEquities.Count > 0 && x.RoleUserType == RoleUserTypes.Candidate))).ToList();
            InterviewUser interviewUser = null;
            Entities.Interview interview = null;
            Equity equity = null;
            StringBuilder sb = new StringBuilder();
            bool isInEeInterview = false;

            foreach (RoleUser roleUser in filteredRoleUsers)
            {

                List<Entities.Interview> interviews = processes.SelectMany(x => x.Interviews).ToList();
                interviewUser = interviews.SelectMany(x => x.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate && x.UserId == roleUser.Id).ToList()).FirstOrDefault();

                if (interviewUser != null)
                {
                    interview = interviews.Where(x => x.Id == interviewUser.InterviewId).First();
                    sb.Clear();
                    isInEeInterview = false;
                    if (interview != null)
                    {
                        foreach (RoleUserEquity roleUserEquity in roleUser.RoleUserEquities)
                        {
                            equity = equities.Where(x => x.Id == roleUserEquity.EquityId).First();
                            sb.Append(cultureName == Constants.EnglishCulture ? equity.NameEN : equity.NameFR);
                            if (roleUserEquity != roleUser.RoleUserEquities.Last())
                                sb.Append(", ");
                            isInEeInterview = true;
                        }

                        eeCandidate = new VmEeCandidate()
                        {
                            GivenName = roleUser.UserFirstname,
                            Surname = roleUser.UserLastname,
                            EquitiesDescription = sb.ToString(),
                            InterviewDate = interview.StartDate,
                            IsInEeInterview = isInEeInterview
                        };
                        result.Add(eeCandidate);

                    }
                }

            }

            return result;

        }

        public List<VmDashboardItem> GetProcessStatsDailyView(List<Process> processes, List<Equity> equities, string cultureName)
        {

            List<VmDashboardItem> result = new List<VmDashboardItem>();
            List<Entities.Interview> interviews = processes.SelectMany(x => x.Interviews).ToList();          
            VmDashboardItem dashboardItem;
            DateTime date;
            int numberSlots;
            int numberProgressCompleted;
            int numberProgressRemaining;
            int numberCanidateInSlots;
            int numberCandidatesNotInSlots;
            int numberVirtuals;
            int numberInPersons;
            int numberDaysOfInterview;
            RoleUser roleUser;

            foreach (Entities.Interview interview in interviews)
            {

                numberSlots = interviews.Count();
                date = interviews.Min(x => x.StartDate.Date);
                numberProgressCompleted = interviews.Where(x => x.Status == InterviewStates.Reserve && x.StartDate < DateTime.Now).Count();
                numberProgressRemaining = numberSlots - numberProgressCompleted;
                numberCanidateInSlots = interviews.Where(x => x.Status == InterviewStates.Reserve).Count();
                numberCandidatesNotInSlots = interviews.Where(x => x.Status == InterviewStates.Available).Count();
                numberVirtuals = interviews.Where(x => string.IsNullOrEmpty(x.Room)).Count();
                numberInPersons = numberSlots - numberVirtuals;
                numberDaysOfInterview = interviews.DistinctBy(x => x.StartDate.Day).Count();

                dashboardItem = new VmDashboardItem()
                {
                    ProcessId = interview.ProcessId,
                    Date = date,
                    Dates = date.ToLongDateString(),
                    NumberSlots = numberSlots,
                    NumberProgressCompleted = numberProgressCompleted,
                    NumberProgressRemaining = numberProgressRemaining,
                    NumberCandidateInSlots = numberCanidateInSlots,
                    NumberCandidateNotInSlots = numberCandidatesNotInSlots,
                    NumberVirtuals = numberVirtuals,
                    NumberInPersons = numberInPersons,
                    NumberDaysOfInterview = numberDaysOfInterview
                };

                foreach (InterviewUser interviewUser in interview.InterviewUsers)
                {
                    roleUser = processes.SelectMany(x => x.RoleUsers.Where(x => x.Id == interviewUser.UserId)).FirstOrDefault();
                    if (roleUser != null)
                    {
                        if (roleUser.RoleUserType == RoleUserTypes.Candidate)
                        {
                            foreach (Equity equity in equities)
                            {
                                if (!dashboardItem.EeCandidates.ContainsKey(equity.Id))
                                {
                                    dashboardItem.EeCandidates.Add(equity.Id, new VmEeGroupItem()
                                    {
                                        EquityId = equity.Id,
                                        Name = cultureName == Constants.EnglishCulture ? equity.NameEN : equity.NameFR,
                                        Count = 0
                                    });
                                }
                                else
                                    dashboardItem.EeCandidates[equity.Id].Count++;
                            }
                        }
                        else
                        {
                            foreach (Equity equity in equities)
                            {
                                if (!dashboardItem.EeBoardMembers.ContainsKey(equity.Id))
                                {
                                    dashboardItem.EeBoardMembers.Add(equity.Id, new VmEeGroupItem()
                                    {
                                        EquityId = equity.Id,
                                        Name = cultureName == Constants.EnglishCulture ? equity.NameEN : equity.NameFR,
                                        Count = 0
                                    });
                                }
                                else
                                    dashboardItem.EeBoardMembers[equity.Id].Count++;
                            }
                        }
                    }

                }

                result.Add(dashboardItem);

            }

            return result;

        }

        public List<VmInterviewCounts> GetInterviewStatsMonthlyView(List<Process> processes)
        {

            List<VmInterviewCounts> result = new List<VmInterviewCounts>();



            return result;

        }

        #endregion

    }

}
