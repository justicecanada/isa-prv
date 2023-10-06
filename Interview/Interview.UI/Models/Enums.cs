using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{

    // UserTypes
    // Email Types

    public enum VmScheduleTypes
    {
        [Display(Name = "Candidate")]
        Candidate = 1,
        [Display(Name = "Members")]
        Members = 2,
        [Display(Name = "Display")]
        Marking = 3
    }

    public enum VmRoleTypes
    {
        [Display(Name = "HR")]
        HR = 1,
        [Display(Name = "Interviewer")]
        Interviewer = 2,
        [Display(Name = "Lead")]
        Lead = 3,
        [Display(Name = "Assistant")]
        Assistant = 4,
        [Display(Name = "Candidate")]
        Candidate = 5,
        [Display(Name = "Admin")]
        Admin = 6
    }

    public enum VmLanguageTypes
    {
        [Display(Name = "English")]
        English = 1,
        [Display(Name = "French")]
        French = 2,
        [Display(Name = "Bilingual")]
        Bilingual = 3
    }

}
