using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmUserSettingEquity : VmBase
    {

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many

        public Guid UserSettingId { get; set; }
        public Guid EquityId { get; set; }

        public VmUserSetting UserSetting { get; set; }
        public VmEquity Equity { get; set; }

    }

}
