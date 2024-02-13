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
        // This is a RoleUser.Id
        public Guid UserId { get; set; }
        public RoleUserTypes RoleUserType { get; set; }

    }

}
