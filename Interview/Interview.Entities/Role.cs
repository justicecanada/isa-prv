using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class Role : EntityBase
    {

        public Guid UserSettingsId { get; set; }
        public string? RoleNameFR { get; set; }
        public string? RoleNameEN { get; set; }
        public bool? IsDeleted { get; set; }

    }

}
