using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.Dashboard;
using Interview.UI.Models.Stats;

namespace Interview.UI.Services.Stats
{
    
    public interface IStats
    {

        VmInterviewCounts GetInterviewCounts(List<Process> processes);

        List<VmEquityStat> GetCandiateEquityStats(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmEquityStat> GetBoartdMemberEquityStats(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmEquityStat> GetInterviewerAndLeadEquityStatsForInterviews(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmEquityStat> GetCandidateEquityStatsEquityStatsForInterviews(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmEeCandidate> GetEeCandidatesForInterviews(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmDashboardItem> GetDashboardItems(List<Process> processes, List<Equity> equities, string cultureName, VmPeriodOfTimeTypes periodOfTimeType);

    }

}
