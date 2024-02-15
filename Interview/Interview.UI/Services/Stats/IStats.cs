using Interview.Entities;
using Interview.UI.Models.Stats;

namespace Interview.UI.Services.Stats
{
    
    public interface IStats
    {

        VmInterviewStats GetInterviewStats(List<Process> processes);

        List<VmEquityStat> GetCandiateEquityStats(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmEquityStat> GetBoartdMemberEquityStats(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmEquityStat> GetInterviewerAndLeadEquityStatsForInterviews(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmEquityStat> GetCandidateEquityStatsEquityStatsForInterviews(List<Process> processes, List<Equity> equities, string cultureName);

        List<VmEeCandidate> GetEeCandidatesForInterviews(List<Process> processes, List<Equity> equities, string cultureName);

    }

}
