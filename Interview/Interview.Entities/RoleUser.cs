using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{

    public class RoleUser : EntityBase
    {

        public RoleUser()
        {
            RoleUserEquities = new List<RoleUserEquity>();
            Equities = new List<Equity>();
        }

        public Guid ProcessId { get; set; }
        public Guid UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public bool? IsExternal { get; set; }
        public DateTime? DateExternalEmailSent { get; set; }
        public RoleTypes RoleType { get; set; }
        public LanguageTypes? LanguageType { get; set; }
        public DateTime DateInserted { get; set; }
        public bool HasAcceptedPrivacyStatement { get; set; }

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
        public List<RoleUserEquity> RoleUserEquities { get; set; }
        public List<Equity> Equities { get; set; }

    }

}
