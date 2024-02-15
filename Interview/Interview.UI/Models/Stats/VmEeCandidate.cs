using Interview.Entities;

namespace Interview.UI.Models.Stats
{

    public class VmEeCandidate
    {

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public string EquitiesDescription { get; set; }

        public DateTimeOffset InterviewDate { get; set; }

        public bool IsInEeInterview { get; set; }

    }

}
