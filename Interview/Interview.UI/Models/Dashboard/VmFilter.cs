using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Dashboard
{
    
    public class VmFilter
    {

        [Display(Name = "ProcessId")]
        public Guid? processid { get; set; }

        [Display(Name = "PeriodOfTimeType")]
        public string periodoftime { get; set; }
        //public string IntPeriodOfTime { get; set; }

        [Display(Name = "StartDate")]
        public DateTime? startdate { get; set; }

        [Display(Name = "EndDate")]
        public DateTime? enddate { get; set; }

    }

}
