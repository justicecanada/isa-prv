using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{

    public class UserSetting : EntityBase
    {

        public Guid ContestId { get; set; }
        public Guid? UserLanguageId { get; set;  }
        public Guid? EquityId { get; set; }
        public Guid RoleId { get; set;  }
        public Guid UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public bool? IsExternal { get; set; }
        public DateTime DateInserted { get; set; }

        public UserLanguage? UserLanguage { get; set;  }
        public Role? Role { get; set;  }

    }

}
