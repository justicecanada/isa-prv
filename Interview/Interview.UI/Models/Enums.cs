using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{

    // The only reason for these Enums to be turned into view models is for localization.
    // The following Enums are not exposed by the UI so no need to localize.
        // UserTypes        // Currently only used by MockUser, this may go away
        // ScheduleTypes
    // EmailTypes

    public enum VmRoleTypes
    {
        [Display(Name = "Admin")]
        Admin = 1,
        [Display(Name = "Owner")]
        Owner = 2,
        [Display(Name = "System")]
        System = 3
    }

    public enum VmRoleUserTypes
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

    public enum VmPeriodOfTimeTypes
    {
        [Display(Name = "Daily")]
        Daily = 1,
        //[Display(Name = "Weekly")]
        //Weekly = 2,
        [Display(Name = "Monthly")]
        Monthly = 3
    }

}
