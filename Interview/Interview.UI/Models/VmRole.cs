namespace Interview.UI.Models
{
    
    public class VmRole : VmBase
    {

        public Guid UserSettingsId { get; set; }

        public string? RoleNameFR { get; set; }

        public string? RoleNameEN { get; set; }

        public bool? IsDeleted { get; set; }

    }

}
