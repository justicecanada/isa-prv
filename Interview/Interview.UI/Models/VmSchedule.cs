using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmSchedule : VmBase
    {

        public Guid ContestId { get; set; }
        public ScheduleTypes ScheduleType { get; set; }
        public int? StartValue { get; set; }
        public bool IsDeleted { get; set; }

    }

}
