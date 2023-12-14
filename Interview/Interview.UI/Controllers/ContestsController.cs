using AutoMapper;
using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Models;
using Interview.UI.Services.Automapper;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Services.State;
using Interview.UI.Services.Mock.Departments;
using Interview.UI.Models.AppSettings;
using Microsoft.Extensions.Options;
using Interview.UI.Services.Mock.Identity;

namespace Interview.UI.Controllers
{
    public class ContestsController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;

        #endregion

        #region Constructors

        public ContestsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IOptions<JusticeOptions> justiceOptions) 
            : base(modelAccessor, justiceOptions, dal)
        {
            _mapper = mapper;
            _state = state;
        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<Contest> contests = null;
            Guid? contestId = null;

            if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin))
                contests = await _dal.GetAllContests();
            else if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.System))
                throw new NotImplementedException();                            // Will need to create appsetting for MockUser.Department Name (like MockLoggedInUserName)
            else if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Owner))
                contests = await _dal.GetContestsForGroupOwner((Guid)LoggedInMockUser.Id);
            contests.OrderByDescending(x => x.CreatedDate);
            List<MockDepartment> mockDepartments = await _dal.GetAllMockDepatments();

            ViewBag.Contests = contests;
            ViewBag.MockDepartments = mockDepartments;

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> DeleteContest(Guid contestId)
        {

            await _dal.DeleteEntity<Contest>(contestId);

            _state.ContestId = null;

            return RedirectToAction("Index");

        }

        #endregion

        #region Manage Methods

        [HttpGet]   
        public async Task<IActionResult> Contest(Guid? contestId)
        {

            VmContest vmContest = null;

            if (contestId == null)
            {
                vmContest = new VmContest();
            }
            else
            {
                var contest = await _dal.GetEntity<Contest>((Guid)contestId, true) as Contest;
                vmContest = _mapper.Map<VmContest>(contest);
                PopulateContestSchedulePropertiesFromSchedule(vmContest, contest.Schedules);
            }

            await ContestsSetViewBag();

            return View(vmContest);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContestNext(VmContest vmContest)
        {

            HandleScheduleModelState(vmContest);     // This is temporary until the slider is on the view

            if (ModelState.IsValid)
            {
                var contest = _mapper.Map<Contest>(vmContest);

                contest.CreatedDate = DateTime.Now;
                contest.InitUserId = LoggedInMockUser.Id;
                contest.Schedules = GetSchedules(vmContest);
                await _dal.AddEntity<Contest>(contest);

                return RedirectToAction("Index", "Emails", new { id = vmContest.Id });
            }
            else
            {
                await ContestsSetViewBag();

                return View("Contest", vmContest);
            }

        }

        [HttpPost]  
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContestSave(VmContest vmContest)
        {

            HandleScheduleModelState(vmContest);     // This is temporary until the slider is on the view

            if (ModelState.IsValid)
            {
                Contest postedContext = _mapper.Map<Contest>(vmContest);
                List<Schedule> postedSchedules = GetSchedules(vmContest);
                Contest dbContest = await _dal.GetEntity<Contest>((Guid)vmContest.Id, true) as Contest;
                
                // Resolve Schedules
                ResolveSchedules(postedSchedules, dbContest.Schedules);
                foreach (Schedule schedule in dbContest.Schedules)
                    await _dal.UpdateEntity(schedule);

                // Save Contest
                postedContext.CreatedDate = dbContest.CreatedDate;
                postedContext.InitUserId = dbContest.InitUserId;
                await _dal.UpdateEntity(postedContext);

                return RedirectToAction("Index", "Default");

            }
            else
            {
                await ContestsSetViewBag();

                return View("Contest", vmContest);
            }

        }

        private async Task ContestsSetViewBag()
        {

            // Departments
            var mockDepartments = await _dal.GetAllMockDepatments();
            ViewBag.MockDepartments = mockDepartments;

        }

        #endregion

        #region Private Schedules Methods

        // Typically Schedules would be rendered to the view as Contest.Schedules (List<Schedule), then a partial view would be created to represent each Schedule in the list.
        // The Schedule.StartValue would be represented as a text box.
        // However, the Schedule concerns will be represented with a slider within the view. At this time I'm not sure how the model abstraction would be handled
        // in the view for the slider. So keeping the model simple / flat (Schedule.VmScheduleCandidateStart, Schedule.VmScheduleMembersStart, Schedule.VmScheduleMarkingStart)
        // and let the controller stick handle mapping these properties to proper Schedule objects.

        private void HandleScheduleModelState(VmContest vmContest)
        {

            int candidateStart = vmContest.VmScheduleCandidateStart == null ? 0 : (int)vmContest.VmScheduleCandidateStart;
            int memberStart = vmContest.VmScheduleMembersStart == null ? 0 : (int)vmContest.VmScheduleMembersStart;
            int markingStart = vmContest.VmScheduleMarkingStart == null ? 0 : (int)vmContest.VmScheduleMarkingStart;

            if ((candidateStart + memberStart + markingStart) > vmContest.InterviewDuration)
                ModelState.AddModelError("InterviewDuration", "CandidateStart, MemberStart and MarkerStart cannot add up to > Interview Duration");

        }

        private List<Schedule> GetSchedules(VmContest vmContest)
        {

            List<Schedule> result = new List<Schedule>();

            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Candidate, StartValue = vmContest.VmScheduleCandidateStart });
            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Members, StartValue = vmContest.VmScheduleMembersStart });
            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Marking, StartValue = vmContest.VmScheduleMarkingStart });

            return result;

        }

        private void ResolveSchedules(List<Schedule> postedSchedules, List<Schedule> dbSchedules)
        {

            //List<VmSchedule> result = null;

            dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Candidate).First().StartValue =
                postedSchedules.Where(x => x.ScheduleType == ScheduleTypes.Candidate).First().StartValue;
            dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Members).First().StartValue =
                postedSchedules.Where(x => x.ScheduleType == ScheduleTypes.Members).First().StartValue;
            dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Marking).First().StartValue =
                postedSchedules.Where(x => x.ScheduleType == ScheduleTypes.Marking).First().StartValue;

                //result = _mapper.Map<List<VmSchedule>>(dbSchedules);

                //return result;

        }

        private void PopulateContestSchedulePropertiesFromSchedule(VmContest contest, List<Schedule> dbSchedules)
        {

            contest.VmScheduleCandidateStart = dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Candidate).First().StartValue;
            contest.VmScheduleMembersStart = dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Members).First().StartValue;
            contest.VmScheduleMarkingStart = dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Marking).First().StartValue;

        }

        #endregion

    }

}
