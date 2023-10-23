using Interview.Entities;
using Interview.UI.Models.CustomValidation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

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

        [Required]
        [Display(Name = "EmailBody")]
        public string? EmailBody { get; set; }

	}
}
