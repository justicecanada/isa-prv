using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class ProcessGroup : EntityBase
    {

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

        public Guid ProcessId { get; set; }
        public Guid GroupId { get; set; }

        public Process Process { get; set; }
        public Group Group { get; set; }

    }

}
