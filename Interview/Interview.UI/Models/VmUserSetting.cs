using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmUserSetting : VmBase
    {

        public Guid ContestId { get; set; }
        public Guid? UserLanguageId { get; set; }
        public Guid? EquityId { get; set; }
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public bool? IsExternal { get; set; }
        public DateTime DateInserted { get; set; }

        public VmUserLanguage? UserLanguage { get; set; }
        public VmRole? Role { get; set; }

    }

}
