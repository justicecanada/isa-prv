using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class InterviewUserEmail : EntityBase
    {

        public Guid InterviewUserId { get; set; }

        public EmailTypes EmailType { get; set; }

        public DateTime DateSent { get; set; }

    }

}
