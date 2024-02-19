namespace Interview.UI.Models.Dashboard
{

    public class VmDashboardItem
    {

        public VmDashboardItem()
        {
            EeCandidates = new Dictionary<Guid, VmEeGroupItem>();
            EeBoardMembers = new Dictionary<Guid, VmEeGroupItem>();
        }

        public DateTime Date { get; set; }

        public string Dates { get; set; }

        public int NumberSlots { get; set; }

        public int NumberProgressCompleted { get; set; }

        public int NumberProgressRemaining { get; set; }

        public int NumberCandidateInSlots { get; set; }

        public int NumberCandidateNotInSlots { get; set; }

        public int NumberVirtuals { get; set; }

        public int NumberInPersons { get; set; }

        public int NumberDaysOfInterview { get; set; }

        public Dictionary<Guid, VmEeGroupItem> EeCandidates { get; set; }

        public Dictionary<Guid, VmEeGroupItem> EeBoardMembers { get; set; }

    }

}
