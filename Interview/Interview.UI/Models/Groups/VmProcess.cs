using Interview.Entities;

namespace Interview.UI.Models.Groups
{
    
    public class VmProcess : VmBase
    {

        public VmProcess()
        {
            ContestGroups = new List<VmContestGroup>();
            Groups = new List<Group>();
        }

        public string? GroupNiv { get; set; }

        public Guid? EmployeeId { get; set; }

        public List<VmContestGroup> ContestGroups { get; set; }

        public List<Group> Groups { get; set; }

    }

}
