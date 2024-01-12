using Interview.Entities;
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

        Task<List<Process>> GetAllProcesses();

        Task<List<Process>> GetProcessesForGroupOwner(Guid userId);

        Task<List<Process>> GetProcessesForRoleUser(Guid userId);

        Task<List<Process>> GetAllProcessesWithUserRoles();

        Task<List<Group>> GetGroups(Guid? userId = null);

        Task<List<GroupOwner>> GetGroupOwnersByGroupIdAndUserId(Guid groupId, Guid userId);

        Task<List<GroupOwner>> GetGroupOwnersByContextIdAndUserId(Guid processId, Guid userId);

        Task<List<ProcessGroup>> GetProcessGroupByGroupIdAndProcessId(Guid groupId, Guid processId);

        Task<List<Equity>> GetAllEquities();

        Task<List<RoleUser>> GetRoleUsersByProcessId(Guid processId);

        Task<List<RoleUserEquity>> GetRoleUserEquitiesByRoleUserId(Guid userId);

        Task<List<Interview.Entities.Interview>> GetInterViewsByProcessId(Guid processId);

        Task<List<InterviewUser>> GetInterviewUsersByInterviewId(Guid interviewId);

        Task<List<Schedule>> GetSchedulesByProcessId(Guid processId);

        Task<List<EmailTemplate>> GetEmailTemplatesByProcessId(Guid processId);

        Task<RoleUser> GetRoleUserByProcessIdAndUserId(Guid processId, Guid userId);

        Task<List<GroupOwner>> GetGroupOwnersByProcessIdAndUserId(Guid processId, Guid userId);
        
        #endregion

        #region Mock Identity Methods

        Task<List<MockUser>> LookupInteralMockUser(string query);

        Task<List<MockUser>> GetListExistingExternalMockUser();

        Task<MockUser?> GetMockUserByIdAndType(Guid id, UserTypes userType);

        Task<MockUser?> GetMockUserById(Guid id);

        Task<MockUser?> GetMockUserByName(string name);

        Task<Guid> AddMockUser(MockUser mockUser);

        #endregion

    }

}
