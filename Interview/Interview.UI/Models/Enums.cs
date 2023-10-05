using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{

    public enum VmRoleTypes
    {
        [Display(Name = "HR")]
        HR = 1,
        [Display(Name = "INTERVIEWER")]
        INTERVIEWER = 2,
        [Display(Name = "LEAD")]
        LEAD = 3,
        [Display(Name = "ASSISTANT")]
        ASSISTANT = 4,
        [Display(Name = "CANDIDATE")]
        CANDIDATE = 5,
        [Display(Name = "ADMIN")]
        ADMIN = 6
    }

}
