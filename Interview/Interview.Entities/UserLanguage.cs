using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class UserLanguage : EntityBase
    {

        public Guid UserSettingsId { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set;  }

        public List<UserSetting> UserSettings { get; set; }

    }

}
