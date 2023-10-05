using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmUserSetting : VmBase
    {

        public VmUserSetting()
        {
            UserSettingEquities = new List<VmUserSettingEquity>();
            Equities = new List<VmEquity>();
        }

        public Guid ContestId { get; set; }
        public Guid? UserLanguageId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public bool? IsExternal { get; set; }
        public RoleTypes RoleType { get; set; }
        public DateTime DateInserted { get; set; }

        public VmUserLanguage? UserLanguage { get; set; }

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
        public List<VmUserSettingEquity> UserSettingEquities { get; set; }
        public List<VmEquity> Equities { get; set; }

    }

}
