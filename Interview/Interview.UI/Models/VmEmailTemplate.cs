using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmEmailTemplate : VmBase
    {

        public Guid ContestId { get; set; }

        public EmailTypes EmailType { get; set; }

        public string? EmailSubject { get; set; }

        public string? EmailBody { get; set; }

        public string? EmailCC { get; set; }

    }

}
