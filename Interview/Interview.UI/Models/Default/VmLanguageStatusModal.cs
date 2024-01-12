using Interview.Entities;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Default
{
    
    public class VmLanguageStatusModal
    {

        [Display(Name = "LanguageType")]
        [Required]
        public LanguageTypes LanguageType { get; set; }

    }

}
