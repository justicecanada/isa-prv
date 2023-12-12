using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class RoleUserEquity : EntityBase
    {

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

        public Guid RoleUserId { get; set; }
        public Guid EquityId { get; set; }

        public RoleUser RoleUser { get; set;  }
        public Equity Equity { get; set; }

    }

}
