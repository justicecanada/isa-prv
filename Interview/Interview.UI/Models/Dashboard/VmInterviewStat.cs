namespace Interview.UI.Models.Dashboard
{

    public class VmInterviewStat
    {

        public Guid ProcessId { get; set; }

        public string Dates { get; set; }

        public int NumberSlots { get; set; }

        public int ProgressCompleted { get; set; }

        public int ProgressRemaining { get; set; }

        public int CandidateInSlot { get; set; }

        public int CandidateNotInSlot { get; set; }

        public int Virtual { get; set; }

        public int InPerson { get; set; }

        public int NumberDaysOfInterview { get; set; }

        public int NumberCandiatesEeGroup { get; set; } 

        public int NumberBoardMemberEeGroup { get; set; }   

    }

}
