using Interview.Entities;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Default
{
    
    public class VmInterviewerUserIds
    {

        public Guid InterviewId { get; set; }

        [Display(Name = "CandidateUserId")]
        public Guid? CandidateUserId { get; set; }

        [Display(Name = "InterviewerUserId")]
        public Guid? InterviewerUserId { get; set; }

        [Display(Name = "InterviewerLeadUserId")]
        public Guid? InterviewerLeadUserId { get; set; }

    }

}
