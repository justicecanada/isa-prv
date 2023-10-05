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

        public async Task EnsureUserLanguages()
        {

            var dbLanguages = await _context.UserLanguages.ToListAsync();
            var expectedLanguages = GetExpectedLanguages();

            foreach (var expectedLanguage in expectedLanguages)
                if (!dbLanguages.Any(x => x.Name == expectedLanguage.Name))
                    _context.UserLanguages.Add(expectedLanguage);

            _context.SaveChanges();

        }

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

        private List<UserLanguage> GetExpectedLanguages()
        {

            List<UserLanguage> result = new List<UserLanguage>();

            result.Add(GetLanguage("Fr", "français", "french"));
            result.Add(GetLanguage("En", "anglais", "english"));
            result.Add(GetLanguage("En", "bilingue", "bilingual"));

            return result;

        }

        private UserLanguage GetLanguage(string name, string nameFr, string nameEn)
        {

            return new UserLanguage()
            {
                Name = name,
                NameFR = nameFr,
                NameEN = nameEn
            };

        }

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
