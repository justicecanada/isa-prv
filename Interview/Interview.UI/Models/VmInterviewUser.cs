using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmInterviewUser : VmBase
    {

        public Guid InterviewId { get; set; }
        public Guid UserId { get; set; }
        public RoleTypes RoleType { get; set; }

    }

}
