using Interview.Entities;
using Interview.UI.Data;
using Microsoft.EntityFrameworkCore;

namespace Interview.UI.Services.Mock.Departments
{
    
    public class MockDepartmentSeeder
    {

        #region Declarations

        private readonly SqlContext _context;

        #endregion

        #region Constructors

        public MockDepartmentSeeder(SqlContext context)
        {
            _context = context;
        }

        #endregion

        #region Public Methods

        public async Task EnsureDepartments()
        {

            var dbDepartments = await _context.MockDepartments.ToListAsync();
            var expectedDepartments = GetExpectedDepartments();

            foreach (var mockDepartment in expectedDepartments)
                if (!dbDepartments.Any(x => x.Key == mockDepartment.Key))
                    _context.MockDepartments.Add(mockDepartment);

            _context.SaveChanges();

        }

        #endregion

        #region Private Methods

        private List<MockDepartment> GetExpectedDepartments()
        {

            List<MockDepartment> result = new List<MockDepartment>();

            result.Add(GetDepartment(100001, "Justice Canada", "Justice Canada"));
            result.Add(GetDepartment(100002, "Public Prosecution Service Canada", "Service des poursuites pénales du Canada"));
            result.Add(GetDepartment(100003, "Shared Services Canada", "Services partagés Canada"));
            result.Add(GetDepartment(100004, "Law Commision of Canada", "Commission du droit du Canada"));

            return result;

        }

        private MockDepartment GetDepartment(int key, string nameEN, string nameFR)
        {

            return new MockDepartment()
            {
                Key = key,
                NameEN = nameEN,
                NameFR = nameFR
            };

        }

        #endregion

    }

}
