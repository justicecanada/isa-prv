namespace Interview.UI.Models.Dashboard
{

    public class VmDashboardItem
    {

        public VmDashboardItem()
        {
            EeCandidates = new Dictionary<int, VmEeGroupItem>();
            EeBoardMembers = new Dictionary<int, VmEeGroupItem>();
        }

        public Guid ProcessId { get; set; }

        public string Dates { get; set; }

        public int NumberSlots { get; set; }

        public int NumberProgressCompleted { get; set; }

        public int NumberProgressRemaining { get; set; }

        public int NumberCandidateInSlots { get; set; }

        public int NumberCandidateNotInSlots { get; set; }

        public int NumberVirtuals { get; set; }

        public int NumberInPersons { get; set; }

        public int NumberDaysOfInterview { get; set; }

        public Dictionary<int, VmEeGroupItem> EeCandidates { get; set; }

        public Dictionary<int, VmEeGroupItem> EeBoardMembers { get; set; }

    }

}
