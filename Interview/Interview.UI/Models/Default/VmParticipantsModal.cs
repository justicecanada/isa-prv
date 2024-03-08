using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Default
{
    
    public class VmParticipantsModal
    {

        public VmParticipantsModal()
        {

            BoardMemberUserIds = new List<Guid>();
            BoardMemberLeadUserIds = new List<Guid>();

        }

        public Guid InterviewId { get; set; }

        [Display(Name = "CandidateUserId")]
        public Guid? CandidateUserId { get; set; }

        [Display(Name = "BoardMemberUserIds")]
        public List<Guid> BoardMemberUserIds { get; set; }

        [Display(Name = "BoardMemberLeadUserIds")]
        public List<Guid> BoardMemberLeadUserIds { get; set; }

    }

}
