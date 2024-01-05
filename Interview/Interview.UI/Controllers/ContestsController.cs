using AutoMapper;
using Interview.Entities;
using Interview.UI.Data;
using Interview.UI.Models;
using Interview.UI.Services.Automapper;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Services.State;
using Interview.UI.Models.AppSettings;
using Microsoft.Extensions.Options;
using Interview.UI.Services.Mock.Identity;
using Interview.UI.Services.Options;
using Interview.UI.Models.Options;

namespace Interview.UI.Controllers
{
    public class ContestsController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IOptions _options;

        #endregion

        #region Constructors

        public ContestsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IOptions<JusticeOptions> justiceOptions,
            IOptions options) 
            : base(modelAccessor, justiceOptions, dal)
        {
            _mapper = mapper;
            _state = state;
            _options = options;
        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            await IndexSetViewBag();

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> DeleteContest(Guid contestId)
        {

            await _dal.DeleteEntity<Contest>(contestId);

            _state.ContestId = null;

            return RedirectToAction("Index");

        }

        public async Task IndexSetViewBag()
        {

            List<Contest> contests = await GetContestsForLoggedInUser();
            Guid? contestId = null;
            List<DepartmentOption> departments = _options.GetDepartmentOptions();

            ViewBag.Contests = contests;
            ViewBag.Departments = departments;

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
            RegisterContestsClientResources();

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
                RegisterContestsClientResources();

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
                RegisterContestsClientResources();

                return View("Contest", vmContest);
            }

        }

        private async Task ContestsSetViewBag()
        {

            // Departments
            List<DepartmentOption> departments = _options.GetDepartmentOptions();
            ViewBag.Departments = departments;

        }

        private void RegisterContestsClientResources()
        {

            WebTemplateModel.HTMLBodyElements.Add("<script src=\"/assets/vendor/ckeditor5/build/ckeditor.js\"></script>");

            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/JusRichTextBoxFor.js?v={BuildId}\"></script>");

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

            if (vmContest.VmScheduleCandidateStart > vmContest.VmScheduleMembersStart)
                ModelState.AddModelError("VmScheduleCandidateStart", "Schedule Candidate Start cannot be > Schedule Members Start");

            if (vmContest.VmScheduleMembersStart > vmContest.VmScheduleMarkingStart)
                ModelState.AddModelError("VmScheduleMembersStart", "Schedule Members Start cannot be > Schedule Marking Start");

            if (vmContest.VmScheduleMarkingStart > vmContest.InterviewDuration)
                ModelState.AddModelError("VmScheduleMarkingStart", "Schedule Marking Start cannot be > Interview Duration");

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

            dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Candidate).First().StartValue =
                postedSchedules.Where(x => x.ScheduleType == ScheduleTypes.Candidate).First().StartValue;
            dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Members).First().StartValue =
                postedSchedules.Where(x => x.ScheduleType == ScheduleTypes.Members).First().StartValue;
            dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Marking).First().StartValue =
                postedSchedules.Where(x => x.ScheduleType == ScheduleTypes.Marking).First().StartValue;

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
