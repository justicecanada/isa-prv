using Interview.Entities;

namespace Interview.UI.Models.Groups
{
    
    public class VmContestGroup : VmBase
    {

        public Guid ProcessId { get; set; }
        public Guid GroupId { get; set; }

        public VmProcess Process { get; set; }
        public VmGroup Group { get; set; }

    }

}
