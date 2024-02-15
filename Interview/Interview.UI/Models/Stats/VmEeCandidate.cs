using Interview.Entities;

namespace Interview.UI.Models.Stats
{

    public class VmEeCandidate
    {

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public List<Equity> Equities { get; set; }

        public DateTimeOffset InterviewDate { get; set; }

        public bool WeirdLogic { get; set; }

    }

}
