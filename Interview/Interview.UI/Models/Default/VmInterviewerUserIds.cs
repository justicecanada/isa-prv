using Interview.Entities;

namespace Interview.UI.Models.Default
{
    
    public class VmInterviewerUserIds
    {

        public Guid InterviewId { get; set; }

        public RoleTypes? RoleType { get; set;  }

        public Guid? CandidateUserId { get; set; }

        public Guid? InterviewerUserId { get; set; }

        public Guid? InterviewerLeadUserId { get; set; }

    }

}
