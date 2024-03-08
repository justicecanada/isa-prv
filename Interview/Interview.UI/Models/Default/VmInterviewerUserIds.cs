using Interview.Entities;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Default
{
    
    public class VmInterviewerUserIds
    {

        public Guid? InterviewId { get; set; }

        [Display(Name = "CandidateUserId")]
        public Guid? CandidateUserId { get; set; }

        [Display(Name = "BoardMemberUserId")]
        public Guid? BoardMemberUserId { get; set; }

        [Display(Name = "BoardMemberLeadUserId")]
        public Guid? BoardMemberLeadUserId { get; set; }

    }

}
