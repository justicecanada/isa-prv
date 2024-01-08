namespace Interview.Entities
{
    
    public class Process : EntityBase
    {

        public Process()
        {
            EmailTemplates = new List<EmailTemplate>();
            Interviews = new List<Interview>();
            RoleUsers = new List<RoleUser>();
            Schedules = new List<Schedule>();
            ProcessGroups = new List<ProcessGroup>();
            Groups = new List<Group>();
        }

        public string? NoProcessus { get; set; }
        public string? GroupNiv { get; set; }
        public Guid? InitUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? MinTime { get; set; }
        public TimeSpan? MaxTime { get; set; }
        public TimeSpan? InterviewDuration { get; set; }
        public DateTime? DeadlineInterviewer { get; set; }
        public DateTime? DeadlineCandidate { get; set; }
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public bool IsDeleted { get; set; }
        public string? MembersIntroEN { get; set; }
        public string? MembersIntroFR { get; set; }
        public string? CandidatesIntroEN { get; set; }
        public string? CandidatesIntroFR { get; set; }
        public string? EmailServiceSentFrom { get; set; }
        public string? DepartmentKey { get; set; }

        public List<EmailTemplate> EmailTemplates { get; set; }
        public List<Interview> Interviews { get; set; }
        public List<RoleUser> RoleUsers { get; set; }
        public List<Schedule> Schedules { get; set; }
        public List<ProcessGroup> ProcessGroups { get; set;  }
        public List<Group> Groups { get; set; }

    }

}