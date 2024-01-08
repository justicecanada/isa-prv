using Interview.Entities;

namespace Interview.UI.Models.Groups
{
    
    public class VmProcess : VmBase
    {

        public VmProcess()
        {
            ProcessGroups = new List<VmProcessGroup>();
            Groups = new List<Group>();
        }

        public string? GroupNiv { get; set; }

        public Guid? EmployeeId { get; set; }

        public List<VmProcessGroup> ProcessGroups { get; set; }

        public List<Group> Groups { get; set; }

    }

}
