using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Dashboard
{
    
    public class VmFilter
    {

        [Display(Name = "ProcessId")]
        public Guid? ProcessId { get; set; }

        [Display(Name = "PeriodOfTimeType")]
        public VmPeriodOfTimeTypes PeriodOfTimeType { get; set; }

        [Display(Name = "StartDate")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "EndDate")]
        public DateTime? EndDate { get; set; }

    }

}
