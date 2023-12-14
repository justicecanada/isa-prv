using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmRoleUserEquity : VmBase
    {

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

        public Guid RoleUserId { get; set; }
        public Guid EquityId { get; set; }

        public VmRoleUser RoleUser { get; set; }
        public VmEquity Equity { get; set; }

    }

}
