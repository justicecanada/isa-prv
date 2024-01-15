using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Default
{
    
    public class VmPrivacyStatementModal
    {

        [Display(Name = "HasAcceptedPrivacyStatement")]
        public bool HasAcceptedPrivacyStatement { get; set; }

    }

}
