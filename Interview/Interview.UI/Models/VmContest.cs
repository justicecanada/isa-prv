using Interview.Entities;
using Interview.UI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
    
    public class VmContest : VmBase
    {

        [Required]
        [Display(Name = "NoProcessus")]
        [MaxLength(128, ErrorMessage = "NoProcessusMaxLength")]
        public string? NoProcessus { get; set; }

        [Required]
        [Display(Name = "GroupNiv")]
        [MaxLength(128, ErrorMessage = "GroupNivMaxLength")]
        public string? GroupNiv { get; set; }

        [Display(Name = "InitUserId")]
        public Guid? InitUserId { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [Required]
        [Display(Name = "StartDate")]
        public DateTime? StartDate { get; set; }

        [Required]
        [Display(Name = "EndDate")]
        [CompareDateTimeOffsets(">", "StartDate", null, ErrorMessage = "StartDateAfterEndDate")]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "MinTime")]
        public TimeSpan? MinTime { get; set; }

        [Required]
        [Display(Name = "MaxTime")]
        [CompareTimeSpans(">", "MinTime", null, ErrorMessage = "MinTimeAfterMaxTime")]
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
        [MaxLength(4000, ErrorMessage = "MembersIntroEnMaxLength")]
        public string? MembersIntroEN { get; set; }

        [Display(Name = "MembersIntroFR")]
        [MaxLength(4000, ErrorMessage = "MembersIntroFrMaxLength")]
        public string? MembersIntroFR { get; set; }

        [Display(Name = "CandidatesIntroEN")]
        public string? CandidatesIntroEN { get; set; }

        [Display(Name = "CandidatesIntroFR")]
        public string? CandidatesIntroFR { get; set; }

        [Display(Name = "EmailServiceSentFrom")]
        public string? EmailServiceSentFrom { get; set; }

        [Display(Name = "DepartmentId")]
        public Guid? DepartmentId { get; set; }


        //public List<EmailTemplate> EmailTemplates { get; set; }
        //public List<Interview> Interviews { get; set; }
        //public List<VmUserSetting>? UserSettings { get; set; }
        //public List<Schedule> Schedules { get; set; }

        //public List<EmailTemplate> EmailTemplates { get; set; }
        //public List<Interview.Entities.Interview> Interviews { get; set; }
        public List<VmUserSetting>? UserSettings { get; set; }
        //public List<Schedule> Schedules { get; set; }
        //public List<Group> Groups { get; set; }

    }

}
