using Interview.Entities;

namespace Interview.UI.Models
{

    public class VmRoleUser : VmBase
    {

        public VmRoleUser()
        {
            RoleUserEquities = new List<VmRoleUserEquity>();
            Equities = new List<VmEquity>();
        }

        public Guid ProcessId { get; set; }
        public Guid UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public bool? IsExternal { get; set; }
        public DateTime? DateExternalEmailSent { get; set; }
        public VmRoleTypes RoleType { get; set; }
        public VmLanguageTypes? LanguageType { get; set; }
        public DateTime DateInserted { get; set; }

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
        public List<VmRoleUserEquity> RoleUserEquities { get; set; }
        public List<VmEquity> Equities { get; set; }

    }

}
