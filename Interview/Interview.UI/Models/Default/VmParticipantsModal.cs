using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Default
{
    
    public class VmParticipantsModal
    {

        public VmParticipantsModal()
        {

            InterviewerUserIds = new List<Guid>();
            InterviewerLeadUserIds = new List<Guid>();

        }

        public Guid InterviewId { get; set; }

        [Display(Name = "CandidateUserId")]
        public Guid? CandidateUserId { get; set; }

        [Display(Name = "InterviewerUserId")]
        public List<Guid> InterviewerUserIds { get; set; }

        [Display(Name = "InterviewerLeadUserId")]
        public List<Guid> InterviewerLeadUserIds { get; set; }

    }

}
