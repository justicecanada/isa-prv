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
            Contests = new List<VmContest>();
            GroupOwners = new List<VmGroupOwner>();
        }

        public string? NameFr { get; set; }
        public string? NameEn { get; set; }

        [Display(Name = "InternalName")]
        public string? InternalName { get; set; }

        public Guid? InternalId { get; set; }

        [Display(Name = "HasAccessEE")]
        public bool HasAccessEE { get; set; }

        [Display(Name = "ContestIdToAdd")]
        public Guid? ContestIdToAdd { get; set; }

        public bool EditThisGroup { get; set; }

        public List<VmContestGroup> ContestGroups { get; set; }
        public List<VmContest> Contests { get; set; }
        public List<VmGroupOwner> GroupOwners { get; set; }

    }

}
