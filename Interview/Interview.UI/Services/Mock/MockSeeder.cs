using Interview.Entities;
using Interview.UI.Data;
using Microsoft.EntityFrameworkCore;

namespace Interview.UI.Services.Mock
{

    public class MockSeeder
    {

        #region Declarations

        private readonly SqlContext _context;

        #endregion

        #region Constructors

        public MockSeeder(SqlContext context)
        {
            _context = context;
        }

        #endregion

        #region Public Methods

        public async Task EnsureEquities()
        {

            var dbEquities = await _context.Equities.ToListAsync();
            var expectedEquities = GetExpectedEquities();

            foreach (var expectedEquity in expectedEquities)
                if (!dbEquities.Any(x => x.Name == expectedEquity.Name))
                    _context.Equities.Add(expectedEquity);

            _context.SaveChanges();

        }

        #endregion

        #region Private Methods

        private List<Equity> GetExpectedEquities()
        { 
        
            List<Equity> result = new List<Equity>();

            result.Add(GetEquity("Woman", "Femme", "Woman", "", ""));
            result.Add(GetEquity("Indigenous", "Personne autochtone", "Indigenous Person", "", ""));
            result.Add(GetEquity("Disability", "Personne en situation de handicap", "Person with a Disability", "", ""));
            result.Add(GetEquity("Racialized", "Membre d’un groupe racialisé", "Member of a Racialized group", "", ""));
            result.Add(GetEquity("SOGIE", "Orientation sexuelle, identité et expression du genre (OSIEG)", "Sexual Orientation, Gender Identity and Expression (SOGIE)", "", ""));

            return result;

        }

        private Equity GetEquity(string name, string nameFR, string nameEN, string viewFR, string viewEN)
        {

            return new Equity()
            {
                Name = name,
                NameFR = nameFR,
                NameEN = nameEN,
                ViewFR = viewFR,
                ViewEN = viewEN
            };

        }

        #endregion

    }

}
