using Interview.Entities;
using Interview.UI.Data;
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

            switch(typeof(t).Name)
            {
                case nameof(EmailTemplate):

                    if (getChildObjects)
                        result = await _context.EmailTemplates.Where(x => x.Id == id)
                            .Include(x => x.EmailType)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.EmailTemplates.FindAsync(id);
                    break;

                case nameof(EmailType):

                    // No child objects
                    result = await _context.EmailTypes.FindAsync(id);
                    break;

                case nameof(Equity):

                    // No child objects
                    result = await _context.Equities.FindAsync(id);
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

                case nameof(Contest):

                    if (getChildObjects)
                        result = await _context.Contests.Where(x => x.Id == id)
                            .Include(x => x.EmailTemplates)
                            .ThenInclude(x => x.EmailType)
                            .Include(x => x.Interviews)
                            .ThenInclude(x => x.InterviewUsers)
                            .Include(x => x.UserSettings)
                            .ThenInclude(x => x.Equities)
                            .Include(x => x.UserSettings)
                            .ThenInclude(x => x.UserLanguage)
                            .Include(x => x.UserSettings)
                            .ThenInclude(x => x.Role)
                            .Include(x => x.Schedules)
                            .ThenInclude(x => x.ScheduleType)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.Contests.FindAsync(id);
                    break;

                case nameof(Role):

                    // No child objects
                    result = await _context.Roles.FindAsync(id);
                    break;

                case nameof(Schedule):

                    if (getChildObjects)
                        result = await _context.Schedules.Where(x => x.Id == id)
                            .Include(x => x.ScheduleType)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.Schedules.FindAsync(id);
                    break;

                case nameof(ScheduleType):

                    // No child objects
                    result = await _context.ScheduleTypes.FindAsync(id);
                    break;

                case nameof(UserLanguage):

                    // No child objects
                    result = await _context.UserLanguages.FindAsync(id);
                    break;

                case nameof(UserSetting):

                    if (getChildObjects)
                        result = await _context.UserSettings.Where(x => x.Id == id)
                            .Include(x => x.Role)
                            .Include(x => x.Equities)
                            .Include(x => x.UserLanguage)
                            .FirstOrDefaultAsync();
                    else
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

        #endregion

    }

}
