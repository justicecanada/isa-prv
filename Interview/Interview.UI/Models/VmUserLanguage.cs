using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmUserLanguage : VmBase
    {

        public Guid UserSettingsId { get; set; }
        public string Name { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set; }

        public List<VmUserSetting> UserSettings { get; set; }

    }

}
