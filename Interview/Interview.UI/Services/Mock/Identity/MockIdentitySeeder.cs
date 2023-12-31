﻿using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Models.AppSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Interview.UI.Services.Mock.Identity
{
    
    public class MockIdentitySeeder
    {

        #region Declarations

        private readonly SqlContext _context;
        private readonly IOptions<JusticeOptions> _justiceOptions;

        #endregion

        #region Constructors

        public MockIdentitySeeder(SqlContext context, IOptions<JusticeOptions> justiceOptions)
        {
            _context = context;
            _justiceOptions = justiceOptions;
        }

        #endregion

        #region Public Methods

        public async Task EnsureUsers()
        {

            var dbUsers = await _context.MockUsers.ToListAsync();
            var expectedUsers = GetExpectedUsers();

            foreach (var mockUser in expectedUsers)
                if (!dbUsers.Any(x => x.UserName == mockUser.UserName))
                    _context.MockUsers.Add(mockUser);

            _context.SaveChanges();

        }

        #endregion

        #region Private Methods

        public List<MockUser> GetExpectedUsers()
        {

            List<MockUser> result = new List<MockUser>();
            string loggedInUserName = _justiceOptions.Value.MockLoggedInUserName;

            // Mock Logged In User
            result.Add(GetMockUser(loggedInUserName, loggedInUserName, loggedInUserName, loggedInUserName, UserTypes.Internal));

            for (int i = 0; i < 100; i++)
            {
                result.Add(GetMockUser($"{UserTypes.Internal.ToString()}_{i}", $"FirstName_{i}", $"LastName_{i}", $"Email_{i}", UserTypes.Internal));
                result.Add(GetMockUser($"{UserTypes.ExistingExternal.ToString()}_{i}", $"FirstName_{i}", $"LastName_{i}", $"Email_{i}", UserTypes.ExistingExternal));
            }
            
            return result;

        }

        private MockUser GetMockUser(string userName, string firstName, string lastName, string email, UserTypes userType)
        {

            return new MockUser()
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserType = userType
            };

        }

        #endregion

    }

}
