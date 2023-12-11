namespace Interview.UI.Models
{
    
    public class VmInterviewUser : VmBase
    {

        public Guid InterviewId { get; set; }
        public string? UserId { get; set; }
        public int? RoleId { get; set; }

    }

}
