using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmEquity : VmBase
    {

        public VmEquity()
        {
            EmailTemplateEquities = new List<VmUserSettingEquity>();
            UserSettings = new List<VmUserSetting>();
        }

        public string? Name { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set; }
        public string? ViewFR { get; set; }
        public string? ViewEN { get; set; }

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
        public List<VmUserSettingEquity> EmailTemplateEquities { get; set; }
        public List<VmUserSetting> UserSettings { get; set; }

        public bool IsSelected { get; set;  }

    }

}
