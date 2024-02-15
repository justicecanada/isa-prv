using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class GroupOwner : EntityBase
    {

        public Guid GroupId { get; set; }
        // This is an Internal (EntraID) Id
        public Guid UserId { get; set; }
        public bool? HasAccessEE { get; set; }

        public Group Group { get; set;  }

    }

}
