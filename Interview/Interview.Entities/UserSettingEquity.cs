using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class UserSettingEquity : EntityBase
    {

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

        public Guid UserSettingId { get; set; }
        public Guid EquityId { get; set; }

        public UserSetting UserSetting { get; set;  }
        public Equity Equity { get; set; }

    }

}
