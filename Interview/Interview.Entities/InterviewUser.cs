using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class InterviewUser : EntityBase
    {

        public InterviewUser()
        {
            InterviewUserEmails = new List<InterviewUserEmail>();
        }
    
        public Guid InterviewId { get; set; }
        // This is a RoleUser.Id
        public Guid RoleUserId { get; set; }
        public RoleUser RoleUser { get; set; }
        public RoleUserTypes RoleUserType { get; set; }

        public List<InterviewUserEmail> InterviewUserEmails { get; set; }

    }

}
