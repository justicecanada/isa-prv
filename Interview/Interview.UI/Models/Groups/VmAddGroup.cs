using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Groups
{
    
    public class VmAddGroup
    {

        [Display(Name = "NameFr")]
        [Required(ErrorMessage = "RequiredError")]
        public string NameFr { get; set; }

        [Display(Name = "NameEn")]
        [Required(ErrorMessage = "RequiredError")]
        public string NameEn { get; set; }

    }

}
