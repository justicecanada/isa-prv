using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmScheduleType : VmBase
    {

        public Guid ScheduleId { get; set; }
        public string? Type { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set; }
        public bool? IsDeleted { get; set; }

        public List<VmSchedule> Schedules { get; set; }

    }

}
