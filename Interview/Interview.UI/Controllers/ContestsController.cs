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

            var contests = await _dal.GetAllContests();
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
            }

            await ContestsSetViewBag();

            return View(vmContest);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContestNext(VmContest vmContest)
        {
            
            if (ModelState.IsValid)
            {

                var contest = _mapper.Map<Contest>(vmContest);

                if (vmContest.Id == null)
                    await _dal.AddEntity<Contest>(contest);
                else
                    await _dal.UpdateEntity(contest);

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

            if (ModelState.IsValid)
            {

                var contest = _mapper.Map<Contest>(vmContest);

                if (vmContest.Id == null)
                    await _dal.AddEntity<Contest>(contest);
                else
                    await _dal.UpdateEntity(contest);

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

            // Schedules
            var result = new List<Schedule>();
            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Candidate, StartValue = 0 });
            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Members, StartValue = 45 });
            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Marking, StartValue = 90 });
            ViewBag.Schedules = result;

            // Departments
            var mockDepartments = await _dal.GetAllMockDepatments();
            ViewBag.MockDepartments = mockDepartments;

        }

        #endregion

    }

}
