using Interview.Entities;
using Interview.UI.Services.Mock.Departments;
using Interview.UI.Services.Mock.Identity;

namespace Interview.UI.Services.DAL
{

    public interface IDal
    {

        #region Generic CRUD Methods

        Task<Guid> AddEntity<t>(EntityBase entity);
        Task UpdateEntity(EntityBase entity);
        Task<EntityBase> GetEntity<t>(Guid id, bool getChildObjects = false);
        Task DeleteEntity<t>(Guid id);
        Task DeleteEntity(EntityBase entity);

        #endregion

        #region Custom Line of Business Methods

        Task<List<Contest>> GetAllContests();

        Task<List<Contest>> GetContestsForGroupOwner(Guid userId);

        Task<List<Contest>> GetContestsForRoleUser(Guid userId);

        Task<List<Contest>> GetAllContestsWithUserRoles();

        Task<List<Group>> GetGroups(Guid? userId = null);

        Task<List<GroupOwner>> GetGroupOwnersByGroupIdAndUserId(Guid groupId, Guid userId);

        Task<List<GroupOwner>> GetGroupOwnersByContextIdAndUserId(Guid contestId, Guid userId);

        Task<List<ContestGroup>> GetContestGroupByGroupIdAndContestId(Guid groupId, Guid contestId);

        Task<List<Equity>> GetAllEquities();

        Task<List<RoleUser>> GetRoleUsersByContestId(Guid contestId);

        Task<List<RoleUserEquity>> GetRoleUserEquitiesByRoleUserId(Guid userId);

        Task<List<Interview.Entities.Interview>> GetInterViewsByContestId(Guid contestId);

        Task<List<InterviewUser>> GetInterviewUsersByInterviewId(Guid interviewId);

        Task<List<Schedule>> GetSchedulesByContestId(Guid contestId);
        
        #endregion

        #region Mock Identity Methods

        Task<List<MockUser>> LookupInteralMockUser(string query);

        Task<List<MockUser>> GetListExistingExternalMockUser();

        Task<MockUser?> GetMockUserByIdAndType(Guid id, UserTypes userType);

        Task<MockUser?> GetMockUserById(Guid id);

        Task<MockUser?> GetMockUserByName(string name);

        Task<Guid> AddMockUser(MockUser mockUser);

        #endregion

        #region Mock Departments Methods

        Task<List<MockDepartment>> GetAllMockDepatments();

        #endregion

    }

}
