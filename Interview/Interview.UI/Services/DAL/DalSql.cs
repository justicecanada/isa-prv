﻿using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Models.Roles;
using Interview.UI.Services.Mock.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Group = Interview.Entities.Group;

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

            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        }

        #endregion

        #region Public Interface Generic CRUD Methods

        public async Task<Guid> AddEntity<t>(EntityBase entity)
        {

            Guid result;


            switch (typeof(t).Name)
            {
                case nameof(EmailTemplate):
                    _context.EmailTemplates.Add((EmailTemplate)entity);
                    break;

                case nameof(Equity):
                    _context.Equities.Add((Equity)entity);
                    break;

                case nameof(Group):
                    _context.Groups.Add((Group)entity);
                    break;

                case nameof(GroupOwner):
                    _context.GroupsOwners.Add((GroupOwner)entity);
                    break;

                case nameof(Interview.Entities.Interview):
                    _context.Interviews.Add((Interview.Entities.Interview)entity); ;
                    break;

                case nameof(InterviewUser):
                    _context.InterviewUsers.Add((InterviewUser)entity);
                    break;

                case nameof(Process):
                    _context.Processes.Add((Process)entity);
                    break;

                case nameof(ProcessGroup):
                    _context.ProcessGroups.Add((ProcessGroup)entity);
                    break;

                case nameof(Schedule):
                    _context.Schedules.Add((Schedule)entity);
                    break;

                case nameof(RoleUser):
                    _context.RoleUsers.Add((RoleUser)entity);
                    break;

                case nameof(RoleUserEquity):
                    _context.RoleUserEquities.Add((RoleUserEquity)entity);
                    break;

            }

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

                case nameof(Process):

                    if (getChildObjects)
                        result = await _context.Processes.Where(x => x.Id == id)
                            .Include(x => x.EmailTemplates)
                            .Include(x => x.Interviews)
                            .ThenInclude(x => x.InterviewUsers)
                            .Include(x => x.RoleUsers)
                            .Include(x => x.Schedules)
                            .Include(x => x.ProcessGroups)
                            .ThenInclude(x => x.Group)
                            .ThenInclude(x => x.GroupOwners)
                            .FirstOrDefaultAsync();
                    else
                        result = await _context.Processes.FindAsync(id);
                    break;

                case nameof(EmailTemplate):

                    // No child objects
                    result = await _context.EmailTemplates.FindAsync(id);
                    break;

                case nameof(Equity):

                    if (getChildObjects)
                        result = await _context.Equities.Where(x => x.Id == id)
                            .Include(x => x.RoleUsers)
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

                case nameof(Schedule):

                    // No child objects
                    result = await _context.Schedules.FindAsync(id);
                    break;

                case nameof(RoleUser):

                    // No child objects
                    result = await _context.RoleUsers.FindAsync(id);
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

                case nameof(Process):
                    Process? process = await _context.Processes.FindAsync(id);
                    _context.Processes.Remove(process);
                    break;

                case nameof(ProcessGroup):
                    ProcessGroup? processGroup = await _context.ProcessGroups.FindAsync(id);
                    _context.ProcessGroups.Remove(processGroup);
                    break;

                case nameof(Schedule):
                    Schedule? schedule = await _context.Schedules.FindAsync(id);
                    _context.Schedules.Remove(schedule);
                    break;

                case nameof(RoleUser):
                    RoleUser? roleUser = await _context.RoleUsers.FindAsync(id);
                    _context.RoleUsers.Remove(roleUser);
                    break;

                case nameof(RoleUserEquity):
                    RoleUserEquity? roleUserEquity = await _context.RoleUserEquities.FindAsync(id);
                    _context.RoleUserEquities.Remove(roleUserEquity);
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

        public async Task<List<Process>> GetAllProcesses()
        {

            var result = await _context.Processes.Where(x => !x.IsDeleted)
                .Include(x => x.RoleUsers)
                .Include(x => x.Schedules)
                .ToListAsync();

            return result;

        }

        public async Task<List<Process>> GetProcessesForGroupOwner(Guid userId)
        {

            var result = await _context.Processes.Where(x => !x.IsDeleted &&
                (x.Groups.Any(y => y.GroupOwners.Any(z => z.UserId.Equals(userId)))
                || x.RoleUsers.Any(y => y.UserId.Equals(userId))))
                .Include(x => x.Schedules)
                .ToListAsync();

            return result;

        }

        public async Task<List<Process>> GetProcessesForRoleUser(Guid userId)
        {

            var result = await _context.Processes.Where(x => !x.IsDeleted &&
                x.RoleUsers.Any(y => y.UserId.Equals(userId)))
                .ToListAsync();

            return result;

        }

        public async Task<List<Process>> GetAllProcessesWithUserRoles()
        {

            var result = await _context.Processes.Where(x => !x.IsDeleted)
                .Include(x => x.RoleUsers)
                //.ThenInclude(x => x.Role)
                .ToListAsync();

            return result;

        }

        public async Task<List<Group>> GetGroups(Guid? userId = null)
        {

            List<Group> result = null;

            if (userId == null)
            { 
                result = await _context.Groups
                    .Include(x => x.ProcessGroups)
                    .ThenInclude(x => x.Process).Where(x => !(bool)x.IsDeleted)
                    .Include(x => x.GroupOwners)
                    .ToListAsync();
            }
            else
            {
                result = await _context.Groups
                    .Include(x => x.ProcessGroups)
                    .ThenInclude(x => x.Process).Where(x => !(bool)x.IsDeleted && x.GroupOwners.Any(y => y.UserId == userId))
                    .Include(x => x.GroupOwners)
                    .ToListAsync();
            }

            return result;

        }

        public async Task<List<GroupOwner>> GetGroupOwnersByGroupIdAndUserId(Guid groupId, Guid userId)
        {

            List<GroupOwner> result = await _context.GroupsOwners.Where(x => (x.GroupId == groupId && x.UserId == userId)).ToListAsync();

            return result;

        }

        public async Task<List<GroupOwner>> GetGroupOwnersByContextIdAndUserId(Guid processId, Guid userId)
        {

            List<GroupOwner> result = await _context.GroupsOwners.Where(x => (x.UserId == userId &&
                x.Group.Processes.Any(x => x.Id == processId))).ToListAsync();

            return result;

        }

        public async Task<List<ProcessGroup>> GetProcessGroupByGroupIdAndProcessId(Guid groupId, Guid processId)
        {

            List<ProcessGroup> result = await _context.ProcessGroups.Where(x => (x.GroupId == groupId && x.ProcessId == processId)).ToListAsync();

            return result;

        }

        public async Task<List<Equity>> GetAllEquities()
        {

            var result = await _context.Equities.ToListAsync();

            return result;

        }

        public async Task<List<RoleUser>> GetRoleUsersByProcessId(Guid processId)
        {

            var result = await _context.RoleUsers.Where(x => x.ProcessId == processId)
                .Include(x => x.RoleUserEquities)
                .ThenInclude(x => x.Equity)
                .ToListAsync();

            return result;

        }

        public async Task<List<RoleUserEquity>> GetRoleUserEquitiesByRoleUserId(Guid userId)
        {

            var result = await _context.RoleUserEquities.Where(x => x.RoleUserId == userId).ToListAsync();

            return result;

        }

        public async Task<List<Interview.Entities.Interview>> GetInterViewsByProcessId(Guid processId)
		{

            var result = await _context.Interviews.Where(x => (x.ProcessId == processId)).ToListAsync();

            return result;

        }

        public async Task<List<InterviewUser>> GetInterviewUsersByInterviewId(Guid interviewId)
        {

            var result = await _context.InterviewUsers.Where(x => x.InterviewId == interviewId).ToListAsync();

            return result;

        }

        public async Task<List<Schedule>> GetSchedulesByProcessId(Guid processId)
        {

            var result = await _context.Schedules.Where(x => x.Processid == processId).ToListAsync();

            return result;

        }

        public async Task<List<EmailTemplate>> GetEmailTemplatesByProcessId(Guid processId)
        {

            var result = await _context.EmailTemplates.Where(x => x.ProcessId == processId).ToListAsync();

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

        public async Task<MockUser?> GetMockUserById(Guid id)
        {

            MockUser? result = await _context.MockUsers.Where(x => (x.Id == id)).FirstOrDefaultAsync();

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

    }

}
