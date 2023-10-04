using Interview.Entities;

namespace Interview.UI.Models.Groups
{
    
    public class VmContestGroup : VmBase
    {

        public Guid ContestId { get; set; }
        public Guid GroupId { get; set; }

        public VmContest Contest { get; set; }
        public VmGroup Group { get; set; }

    }

}
