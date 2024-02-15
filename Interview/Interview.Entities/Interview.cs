using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class Interview : EntityBase
    {

        public Interview()
        {
            InterviewUsers = new List<InterviewUser>();
        }

        public Guid ProcessId { get; set;  }
        public string? Room { get; set; }
        public string? Location { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public int? Duration { get; set; }
        public InterviewStates? Status { get; set; }
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }

        public List<InterviewUser> InterviewUsers { get; set; }

    }

}
