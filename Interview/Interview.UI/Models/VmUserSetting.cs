using Interview.Entities;

namespace Interview.UI.Models
{
    
    public class VmUserSetting : VmBase
    {

        public Guid ContestId { get; set; }
        public string? UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public bool? IsExternal { get; set; }
        public DateTimeOffset? DateInserted { get; set; }

        //public UserLanguage UserLanguage { get; set; }
        public VmRole? Role { get; set; }
        //public List<Equity> Equities { get; set; }

    }

}
