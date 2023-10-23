using Interview.Entities;
using Interview.UI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
    public class VmEmails : VmBase
    {
        [Required]
<<<<<<< HEAD
        [Display(Name = "EmailSubject")]
        [MaxLength(128, ErrorMessage = "EmailSubjectMaxLength")]
		public string? EmailSubject { get; set; }

        [Display(Name = "EmailCC")]
        public string? EmailCC { get; set; }

        [Required]
        [Display(Name = "EmailBody")]
        public string? EmailBody { get; set; }

	}
=======
        [Display(Name = "Subject-EN")]
        [MaxLength(128, ErrorMessage = "SubjectMaxLength")]
        public string? Subject { get; set; }

        [Display(Name = "CC-EN")]
        [MaxLength(128, ErrorMessage = "CCMaxLength")]
        public string? CC { get; set; }

        [Required]
        [Display(Name = "Description-EN")]
        [MaxLength(128, ErrorMessage = "DescriptionMaxLength")]
        public string? Description { get; set; }

    }
>>>>>>> parent of ccdc6e7 (Latest Emails update)
}
