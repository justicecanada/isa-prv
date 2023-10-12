using Interview.Entities;
using Interview.UI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
    public class VmEmails : VmBase
    {
        [Required]
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
}
