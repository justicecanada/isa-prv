using Interview.Entities;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
    
    public class VmContest : VmBase
    {

        [Required]
        [Display(Name = "NoProcessus")]
        public string? NoProcessus { get; set; }

        [Required]
        [Display(Name = "GroupNiv")]
        public string? GroupNiv { get; set; }

        [Display(Name = "InitUserId")]
        public Guid? InitUserId { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Required]
        [Display(Name = "StartDate")]
        public DateTimeOffset? StartDate { get; set; }

        [Required]
        [Display(Name = "EndDate")]
        public DateTimeOffset? EndDate { get; set; }

        [Required]
        [Display(Name = "MinTime")]
        public TimeSpan? MinTime { get; set; }

        [Required]
        [Display(Name = "MaxTime")]
        public TimeSpan? MaxTime { get; set; }

        [Required]
        [Display(Name = "InterviewDuration")]
        public TimeSpan? InterviewDuration { get; set; }

        [Display(Name = "DeadlineInterviewer")]
        public DateTimeOffset? DeadlineInterviewer { get; set; }

        [Display(Name = "DeadlineCandidate")]
        public DateTimeOffset? DeadlineCandidate { get; set; }

        [Display(Name = "ContactName")]
        public string? ContactName { get; set; }

        [Display(Name = "ContactNumber")]
        public string? ContactNumber { get; set; }

        [Display(Name = "IsDeleted")]
        public bool IsDeleted { get; set; }

        [Display(Name = "MembersIntroEN")]
        public string? MembersIntroEN { get; set; }

        [Display(Name = "MembersIntroFR")]
        public string? MembersIntroFR { get; set; }

        [Display(Name = "CandidatesIntroEN")]
        public string? CandidatesIntroEN { get; set; }

        [Display(Name = "CandidatesIntroFR")]
        public string? CandidatesIntroFR { get; set; }

        [Display(Name = "EmailServiceSentFrom")]
        public string? EmailServiceSentFrom { get; set; }


        //public List<EmailTemplate> EmailTemplates { get; set; }
        //public List<Interview> Interviews { get; set; }
        //public List<UserSetting> UserSettings { get; set; }
        //public List<Schedule> Schedules { get; set; }

    }

}
