using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class ScheduleType : EntityBase
    {

        public Guid ScheduleId { get; set; }
        public string? Type { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set; }
        public bool? IsDeleted { get; set; }

        public List<Schedule> Schedules { get; set; }


    }

}
