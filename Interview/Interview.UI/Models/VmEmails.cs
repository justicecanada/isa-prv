using Interview.Entities;
using Interview.UI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
    public class VmEmails : VmBase
    {
        [Required]
        [Display(Name = "EmailSubject")]
        [MaxLength(128, ErrorMessage = "EmailSubjectMaxLength")]
        public string? EmailSubject { get; set; }

        [Display(Name = "EmailCC")]
        public string? EmailCC { get; set; }

        [Display(Name = "EmailBody")]
        public string? EmailBody { get; set; }

    }
}