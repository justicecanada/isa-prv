using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class InternalUser : EntityBase
    {

        public Guid EntraUserId { get; set; }
        public RoleTypes RoleType { get; set; }

    }

}
