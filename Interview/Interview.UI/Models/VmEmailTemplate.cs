using Interview.Entities;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
    
    public class VmEmailTemplate : VmBase
    {

        public Guid ContestId { get; set; }

        [Required]
        [Display(Name = "EmailType")]
        public EmailTypes EmailType { get; set; }

        [Required]
        [Display(Name = "EmailSubject")]
        public string? EmailSubject { get; set; }

        [Required]
        [Display(Name = "EmailBody")]
        public string? EmailBody { get; set; }

        [Display(Name = "EmailCC")]
        public string? EmailCC { get; set; }

    }

}
