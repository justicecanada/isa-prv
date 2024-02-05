using Interview.Entities;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models
{
    public class VmInternalUser : VmBase
    {

        public string? EntraUserName { get; set; }

        [Required]
        [Display(Name = "RoleType")]
        public VmRoleTypes? RoleType { get; set; }

    }
}
