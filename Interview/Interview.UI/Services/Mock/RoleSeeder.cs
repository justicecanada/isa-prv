using Interview.Entities;
using Interview.UI.Data;
using Microsoft.EntityFrameworkCore;

namespace Interview.UI.Services.Mock
{

    public class RoleSeeder
    {

        #region Declarations

        private readonly SqlContext _context;

        #endregion

        #region Constructors

        public RoleSeeder(SqlContext context)
        {
            _context = context;
        }

        #endregion

        #region Public Methods

        public async Task EnsureRoles()
        {

            try
            {

                var dbRoles = await _context.Roles.ToListAsync();
                var expectedRoles = GetExpectedRoles();

                foreach (var expectedRole in expectedRoles)
                    if (!dbRoles.Any(x => x.Name == expectedRole.Name))
                        _context.Roles.Add(expectedRole);

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

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

        #endregion

    }

}
