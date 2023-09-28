using Interview.Entities;
using Interview.UI.Services.Mock.Identity;

namespace Interview.UI.Services.DAL
{

    public interface IDal
    {

        #region Generic CRUD Methods

        Task<Guid> AddEntity(EntityBase entity);
        Task UpdateEntity(EntityBase entity);
        Task<EntityBase> GetEntity<t>(Guid id, bool getChildObjects = false);
        Task DeleteEntity<t>(Guid id);
        Task DeleteEntity(EntityBase entity);

        #endregion

        #region Custom Line of Business Methods

        Task<List<Contest>> GetAllContests();

        Task<List<Contest>> GetAllContestsWithUserSettingsAndRoles();

        Task<List<Role>> GetAllRoles();

        Task<List<UserLanguage>> GetAllUserLanguages();

        Task<List<Equity>> GetAllEquities();

        Task<List<UserSetting>> GetUserSettingsByContestId(Guid contestId);

        #endregion

        #region Mock Identity Methods

        Task<List<MockUser>> LookupInteralMockUser(string query);

        Task<List<MockUser>> GetListExistingExternalMockUser();

        Task<MockUser?> GetMockUserByIdAndType(Guid id, UserTypes userType);

        Task<Guid> AddMockUser(MockUser mockUser);

        #endregion

    }

}
