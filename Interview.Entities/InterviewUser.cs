using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class InterviewUser : EntityBase
    {
    
        public Guid InterviewId { get; set; }
        public string? UserId { get; set; }
        public int? RoleId { get; set; }

    }

}
