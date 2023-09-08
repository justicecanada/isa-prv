using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class Schedule : EntityBase
    {

        public Guid ContestId { get; set; }
        public int? StartValue { get; set; }
        public bool? IsDeleted { get; set; }
       
        public ScheduleType ScheduleType { get; set; }

    }

}
