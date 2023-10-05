using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class ContestGroup : EntityBase
    {

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

        public Guid ContestId { get; set; }
        public Guid GroupId { get; set; }

        public Contest Contest { get; set; }
        public Group Group { get; set; }

    }

}
