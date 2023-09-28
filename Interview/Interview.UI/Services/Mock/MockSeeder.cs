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

        public async Task EnsureRoles()
        {

            var dbRoles = await _context.Roles.ToListAsync();
            var expectedRoles = GetExpectedRoles();

            foreach (var expectedRole in expectedRoles)
                if (!dbRoles.Any(x => x.Name == expectedRole.Name))
                    _context.Roles.Add(expectedRole);

            _context.SaveChanges();

        }

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

        private List<Role> GetExpectedRoles()
        {

            List<Role> result = new List<Role>();

            result.Add(GetRole("HR", "RH", "HR", false));
            result.Add(GetRole("HR", "Intervieweur", "Board Member", false));
            result.Add(GetRole("HR", "Intervieweur principal", "Lead Board Member", false));
            result.Add(GetRole("HR", "Adjoint(e)", "Board Member Assistant", false));
            result.Add(GetRole("HR", "Candidat(e)", "Candidate", false));
            result.Add(GetRole("HR", "Admin", "Admin", false));

            return result;

        }

        private Role GetRole(string name, string roleNameFr, string roleNameEn, bool isDeleted)
        {

            return new Role()
            {
                Name = name,
                RoleNameFR = roleNameFr,
                RoleNameEN = roleNameEn,
                IsDeleted = isDeleted
            };

        }

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
