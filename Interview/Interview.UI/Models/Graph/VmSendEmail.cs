using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Graph
{
    
    public class VmSendEmail
    {

        [Display(Name = "Subject")]
        [Required]
        public string? Subject { get; set; }

        [Display(Name = "Body")]
        [Required]
        public string? Body { get; set; }

        [Display(Name = "ToRecipients")]
        [Required]
        public string? ToRecipients { get; set; }

        [Display(Name = "CcRecipients")]
        //[Required]
        public string? CcRecipients { get; set; }

        public bool SaveToSentItems { get; set; }

    }

}
