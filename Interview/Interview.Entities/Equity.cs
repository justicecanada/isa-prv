using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class Equity : EntityBase
    {

        public Equity()
        {
            EmailTemplateEquities = new List<RoleUserEquity>();
            RoleUsers = new List<RoleUser>();
        }

        public string Name { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set; }
        public string? ViewFR { get; set; }
        public string? ViewEN { get; set; }

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
        public List<RoleUserEquity> EmailTemplateEquities { get; set; }
        public List<RoleUser> RoleUsers { get; set;  }

    }

}
