using Interview.Entities;
using Interview.UI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace Interview.UI.Models.Groups
{
    
    public class VmGroup : VmBase
    {

        public VmGroup()
        {
            ContestGroups = new List<VmContestGroup>();
            Processes = new List<VmProcess>();
            GroupOwners = new List<VmGroupOwner>();
        }

        public string? NameFr { get; set; }
        public string? NameEn { get; set; }

        [Display(Name = "InternalName")]
        public string? InternalName { get; set; }

        public Guid? InternalId { get; set; }

        [Display(Name = "HasAccessEE")]
        public bool HasAccessEE { get; set; }

        [Display(Name = "ProcessIdToAdd")]
        public Guid? ProcessIdToAdd { get; set; }

        public bool EditThisGroup { get; set; }

        public List<VmContestGroup> ContestGroups { get; set; }
        public List<VmProcess> Processes { get; set; }
        public List<VmGroupOwner> GroupOwners { get; set; }

    }

}
