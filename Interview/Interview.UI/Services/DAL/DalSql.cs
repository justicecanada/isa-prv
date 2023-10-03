using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Models.Roles;
using Interview.UI.Services.Mock.Departments;
using Interview.UI.Services.Mock.Identity;
using Microsoft.EntityFrameworkCore;

namespace Interview.UI.Services.DAL
{
    
    public class DalSql : IDal
    {

        #region Declarations

        private readonly SqlContext _context;

        #endregion

        #region Constructors

        public DalSql(SqlContext context)
        {
            _context = context;
        }

        #endregion

        #region Public Interface Generic CRUD Methods

        public async Task<Guid> AddEntity(EntityBase entity)
        {

            Guid result;

            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync();
            result = (Guid)entity.Id;

            return result;

        }

        public async Task UpdateEntity(EntityBase entity)
        {

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public async Task<EntityBase> GetEntity<t>(Guid id, bool getChildObjects = false)
        {

            EntityBase? result = null;

            switch (typeof(t).Name)
            {

                case nameof(Contest):

                    if (getChildObjects)
                        result = await _context.Contests.Where(x => x.Id == id)
                            .Include(x => x.EmailTemplates)
                            .Include(x => x.Interviews)
                            .ThenInclude(x => x.InterviewUsers)
                            .Include(x => x.UserSettings)
                            .Include(x => x.Schedules)
                            .Include(x => x.Groups)
                            .ThenInclude(x => x.GroupOwners)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.Contests.FindAsync(id);
                    break;

                case nameof(EmailTemplate):

                    // No child objects
                    result = await _context.EmailTemplates.FindAsync(id);
                    break;

                case nameof(EmailType):

                    if (getChildObjects)
                        result = await _context.EmailTypes.Where(x => x.Id == id)
                            .Include(x => x.EmailTemplates)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.EmailTypes.FindAsync(id);
                    break;

                case nameof(Equity):

                    if (getChildObjects)
                        result = await _context.Equities.Where(x => x.Id == id)
                            .Include(x => x.UserSettings)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.Equities.FindAsync(id);
                    break;

                case nameof(Group):

                    if (getChildObjects)
                        result = await _context.Groups.Where(x => x.Id == id)
                            .Include(x => x.GroupOwners)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.Groups.FindAsync(id);
                    break;

                case nameof(GroupOwner):

                    result = await _context.GroupsOwners.FindAsync(id);
                    break;

                case nameof(Interview):

                    if (getChildObjects)
                        result = await _context.Interviews.Where(x => x.Id == id)
                            .Include(x => x.InterviewUsers)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.Interviews.FindAsync(id);
                    break;

                case nameof(InterviewUser):

                    // No child objects
                    result = await _context.InterviewUsers.FindAsync(id);
                    break;

                case nameof(Role):

                    if (getChildObjects)
                        result = await _context.Roles.Where(x => x.Id == id)
                            .Include(x => x.UserSettings)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.Roles.FindAsync(id);
                    break;

                case nameof(Schedule):

                    // No child objects
                    result = await _context.Schedules.FindAsync(id);
                    break;

                case nameof(ScheduleType):

                    if (getChildObjects)
                        result = await _context.ScheduleTypes.Where(x => x.Id == id)
                            .Include(x => x.Schedules)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.ScheduleTypes.FindAsync(id);
                    break;

                case nameof(UserLanguage):

                    if (getChildObjects)
                        result = await _context.UserLanguages.Where(x => x.Id == id)
                            .Include(x => x.UserSettings)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.UserLanguages.FindAsync(id);
                    break;

                case nameof(UserSetting):

                    // No child objects
                    result = await _context.UserSettings.FindAsync(id);
                    break;

            }

            return result;

        }

        public async Task DeleteEntity<t>(Guid id)
        {

            switch (typeof(t).Name)
            {
                case nameof(EmailTemplate):
                    EmailTemplate? emailTemplate = await _context.EmailTemplates.FindAsync(id); 
                    _context.EmailTemplates.Remove(emailTemplate);
                    break;

                case nameof(EmailType):
                    EmailType? ematilType = await _context.EmailTypes.FindAsync(id);
                    _context.EmailTypes.Remove(ematilType);
                    break;

                case nameof(Equity):
                    Equity? equity = await _context.Equities.FindAsync(id);
                    _context.Equities.Remove(equity);
                    break;

                case nameof(Group):
                    Group? group = await _context.Groups.FindAsync(id);
                    _context.Groups.Remove(group);
                    break;

                case nameof(GroupOwner):
                    GroupOwner groupOwner = await _context.GroupsOwners.FindAsync(id);
                    _context.GroupsOwners.Remove(groupOwner);
                    break;

                case nameof(Interview.Entities.Interview):
                    Interview.Entities.Interview? interview = await _context.Interviews.FindAsync(id);
                    _context.Interviews.Remove(interview);
                    break;

                case nameof(InterviewUser):
                    InterviewUser? interviewUser = await _context.InterviewUsers.FindAsync(id);
                    _context.InterviewUsers.Remove(interviewUser);
                    break;

                case nameof(Contest):
                    Contest? contest = await _context.Contests.FindAsync(id);
                    _context.Contests.Remove(contest);
                    break;

                case nameof(Role):
                    Role? role = await _context.Roles.FindAsync(id);
                    _context.Roles.Remove(role);
                    break;

                case nameof(Schedule):
                    Schedule? schedule = await _context.Schedules.FindAsync(id);
                    _context.Schedules.Remove(schedule);
                    break;

                case nameof(ScheduleType):
                    ScheduleType? scheduleType = await _context.ScheduleTypes.FindAsync(id);
                    _context.ScheduleTypes.Remove(scheduleType);
                    break;

                case nameof(UserLanguage):
                    UserLanguage? userLanguage = await _context.UserLanguages.FindAsync(id);
                    _context.UserLanguages.Remove(userLanguage);
                    break;

                case nameof(UserSetting):
                    UserSetting? userSettings = await _context.UserSettings.FindAsync(id);
                    _context.UserSettings.Remove(userSettings);
                    break;

                case nameof(UserSettingEquity):
                    UserSettingEquity? userSettingEquity = await _context.UserSettingEquities.FindAsync(id);
                    _context.UserSettingEquities.Remove(userSettingEquity);
                    break;

            }

            await _context.SaveChangesAsync();

        }

        public async Task DeleteEntity(EntityBase entity)
        {

            _context.Remove(entity);
            await _context.SaveChangesAsync();

        }

        #endregion

        #region Public Interface Custom Line of Business Methods

        public async Task<List<Contest>> GetAllContests()
        {

            var result = await _context.Contests.Where(x => !x.IsDeleted).ToListAsync();

            return result;

        }

        public async Task<List<Contest>> GetAllContestsWithUserSettingsAndRoles()
        {

            var result = await _context.Contests.Where(x => !x.IsDeleted)
                .Include(x => x.UserSettings)
                //.ThenInclude(x => x.Role)
                .ToListAsync();

            return result;

        }

        public async Task<List<Role>> GetAllRoles()
        {

            var result = await _context.Roles.ToListAsync();

            return result;

        }

        public async Task<List<UserLanguage>> GetAllUserLanguages()
        {

            var result = await _context.UserLanguages.ToListAsync();

            return result;

        }

        public async Task<List<Equity>> GetAllEquities()
        {

            var result = await _context.Equities.ToListAsync();

            return result;

        }

        public async Task<List<UserSetting>> GetUserSettingsByContestId(Guid contestId)
        {

            var result = await _context.UserSettings.Where(x => x.ContestId == contestId)
                .Include(x => x.Role)
                .Include(x => x.UserLanguage)
                .Include(x => x.UserSettingEquities)
                .ThenInclude(x => x.Equity)
                .ToListAsync();

            return result;

        }

        public async Task<UserSetting?> GetUserSettingByContestIdAndUserId(Guid contestId, Guid userId)
        {

            var result = await _context.UserSettings.Where(x => (x.ContestId == contestId && x.UserId == userId))
                .Include(x => x.Role)
                .Include(x => x.UserLanguage)
                .Include(x => x.UserSettingEquities)
                .ThenInclude(x => x.Equity)
                .FirstOrDefaultAsync();

            return result;

        }

        public async Task<List<UserSettingEquity>> GetUserSettingEquitiesByUserSettingId(Guid userSettingId)
        {

            var result = await _context.UserSettingEquities.Where(x => x.UserSettingId == userSettingId).ToListAsync();

            return result;

        }

        #endregion

        #region Mock Identity Methods

        public async Task<List<MockUser>> LookupInteralMockUser(string query)
        {

            List<MockUser> result = await _context.MockUsers.Where(x => ((x.FirstName.ToLower().StartsWith(query.ToLower()) || x.LastName.ToLower().StartsWith(query.ToLower()))
                    && x.UserType == UserTypes.Internal)).ToListAsync();

            return result;

        }

        public async Task<List<MockUser>> GetListExistingExternalMockUser()
        {

            List<MockUser> result = await _context.MockUsers.Where(x => x.UserType == UserTypes.ExistingExternal).ToListAsync();

            return result;

        }

        public async Task<MockUser?> GetMockUserByIdAndType(Guid id, UserTypes userType)
        {

            MockUser? result = await _context.MockUsers.Where(x => (x.Id == id &&
                        x.UserType == userType)).FirstOrDefaultAsync();

            return result;

        }

        public async Task<MockUser> GetMockUserByName(string name)
        {
            
            MockUser result = await _context.MockUsers.Where(x => x.UserName == name).FirstAsync();

            return result;

        }

        public async Task<Guid> AddMockUser(MockUser mockUser)
        {

            Guid result;

            _context.MockUsers.Add(mockUser).State = EntityState.Added;
            await _context.SaveChangesAsync();
            result = (Guid)mockUser.Id;

            return result;

        }

        #endregion

        #region Mock Departments Methods

        public async Task<List<MockDepartment>> GetAllMockDepatments()
        {

            List<MockDepartment> result = await _context.MockDepartments.ToListAsync();

            return result;

        }

        #endregion

    }

}
