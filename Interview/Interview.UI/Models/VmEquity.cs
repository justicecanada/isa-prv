using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmEquity : VmBase
    {

        public string Name { get; set;  }
        public Guid UserSettingsId { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set; }
        public string? ViewFR { get; set; }
        public string? ViewEN { get; set; }

        public List<VmUserSetting> UserSettings { get; set; }

    }

}
