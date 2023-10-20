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
        [Display(Name = "EmailSubjectEN")]
        [MaxLength(128, ErrorMessage = "EmailSubjectENMaxLength")]
		public string? EmailSubjectEN { get; set; }

        [Display(Name = "CCEN")]
        public string? CCEN { get; set; }

        [Required]
        [Display(Name = "DescriptionEN")]
        public string? DescriptionEN { get; set; }

		[Required]
		[Display(Name = "EmailSubjectFR")]
		[MaxLength(128, ErrorMessage = "EmailSubjectFRMaxLength")]
		public string? EmailSubjectFR { get; set; }

		[Display(Name = "CCFR")]
		public string? CCFR { get; set; }

		[Required]
		[Display(Name = "DescriptionFR")]
		public string? DescriptionFR { get; set; }

	}
}
