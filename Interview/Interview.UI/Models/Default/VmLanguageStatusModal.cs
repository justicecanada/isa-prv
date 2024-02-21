using Interview.Entities;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Default
{
    
    public class VmLanguageStatusModal
    {

        [Required(ErrorMessage = "LanguageTypeRequired")]
        [Display(Name = "LanguageType")]        
        public LanguageTypes LanguageType { get; set; }

    }

}
