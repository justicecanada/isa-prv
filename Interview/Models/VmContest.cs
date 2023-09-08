using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmContest : VmBase
    {

        public string? NoProcessus { get; set; }
        public string? GroupNiv { get; set; }
        public Guid? InitUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public TimeSpan? MinTime { get; set; }
        public TimeSpan? MaxTime { get; set; }
        public TimeSpan? InterviewDuration { get; set; }
        public DateTimeOffset? DeadlineInterviewer { get; set; }
        public DateTimeOffset? DSeadlineCandidate { get; set; }
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public bool IsDeleted { get; set; }
        public string? MembersIntroEN { get; set; }
        public string? MembersIntroFR { get; set; }
        public string? CandidatesIntroEN { get; set; }
        public string? CandidatesIntroFR { get; set; }
        public string? EmailServiceSentFrom { get; set; }

        //public List<EmailTemplate> EmailTemplates { get; set; }
        //public List<Interview> Interviews { get; set; }
        //public List<UserSetting> UserSettings { get; set; }
        //public List<Schedule> Schedules { get; set; }

    }

}
