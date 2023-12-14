using Interview.Entities;

namespace Interview.UI.Services.Mock.Identity
{

    public class MockUser
    {

        public Guid? Id { get; set; }

        public string? UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public UserTypes? UserType { get; set; }

        public RoleTypes RoleType { get; set; }

    }

}
