using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmSchedule : VmBase
    {

        public Guid ContestId { get; set; }
        //public Guid ScheduleTypeId { get; set; }            // From existing interview app
        public ScheduleTypes ScheduleType { get; set; }
        public int? StartValue { get; set; }
        public bool? IsDeleted { get; set; }

    }

}
