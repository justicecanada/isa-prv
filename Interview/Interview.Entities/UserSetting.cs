using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{

    public class UserSetting : EntityBase
    {

        public UserSetting()
        {
            Equities = new List<Equity>();
        }

        public Guid ContestId { get; set; }
        public string? UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public bool? IsExternal { get; set; }
        public DateTimeOffset? DateInserted { get; set; }

        public UserLanguage UserLanguage { get; set; }
        public Role Role { get; set;  }
        public List<Equity> Equities { get; set; }

    }

}
