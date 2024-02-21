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
using Interview.UI.Services.Options;
using Interview.UI.Models.Options;
using Microsoft.Extensions.Localization;

namespace Interview.UI.Controllers
{
    public class ProcessesController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IOptions _options;

        #endregion

        #region Constructors

        public ProcessesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IOptions options, 
            IStringLocalizer<BaseController> baseLocalizer) 
            : base(modelAccessor, dal, baseLocalizer)
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
        public async Task<IActionResult> DeleteProcess(Guid processId)
        {

            await _dal.DeleteEntity<Process>(processId);

            _state.ProcessId = null;

            return RedirectToAction("Index");

        }

        public async Task IndexSetViewBag()
        {

            List<Process> processes = await GetProcessesForLoggedInUser();
            List<DepartmentOption> departments = _options.GetDepartmentOptions();

            ViewBag.Processes = processes;
            ViewBag.Departments = departments;

        }

        #endregion

        #region Manage Methods

        [HttpGet]   
        public async Task<IActionResult> Process(Guid? processId)
        {

            VmProcess vmProcess = null;

            if (processId == null)
            {
                vmProcess = new VmProcess();
            }
            else
            {
                var process = await _dal.GetEntity<Process>((Guid)processId, true) as Process;
                vmProcess = _mapper.Map<VmProcess>(process);
                PopulateProcessSchedulePropertiesFromSchedule(vmProcess, process.Schedules);
            }

            await ProcesesSetViewBag();
            RegisterProcessesClientResources();

            return View(vmProcess);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessNext(VmProcess vmProcess)
        {

            HandleScheduleModelState(vmProcess);     // This is temporary until the slider is on the view

            if (ModelState.IsValid)
            {
                var process = _mapper.Map<Process>(vmProcess);
                Guid processId;

                process.CreatedDate = DateTime.Now;
                process.InitUserId = EntraId;
                process.Schedules = GetSchedules(vmProcess);
                processId = await _dal.AddEntity<Process>(process);

                if (_state.ProcessId == null)
                    _state.ProcessId = processId;

                return RedirectToAction("Index", "Emails", new { id = vmProcess.Id });
            }
            else
            {
                await ProcesesSetViewBag();
                RegisterProcessesClientResources();

                return View("Process", vmProcess);
            }

        }

        [HttpPost]  
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessSave(VmProcess vmProcess)
        {

            HandleScheduleModelState(vmProcess);     // This is temporary until the slider is on the view

            if (ModelState.IsValid)
            {
                Process postedProcess = _mapper.Map<Process>(vmProcess);
                List<Schedule> postedSchedules = GetSchedules(vmProcess);
                Process dbProcess = await _dal.GetEntity<Process>((Guid)vmProcess.Id, true) as Process;
                
                // Resolve Schedules
                ResolveSchedules(postedSchedules, dbProcess.Schedules);
                foreach (Schedule schedule in dbProcess.Schedules)
                    await _dal.UpdateEntity(schedule);

                // Save Process
                postedProcess.CreatedDate = dbProcess.CreatedDate;
                postedProcess.InitUserId = dbProcess.InitUserId;
                await _dal.UpdateEntity(postedProcess);

                return RedirectToAction("Index", "Default");

            }
            else
            {
                await ProcesesSetViewBag();
                RegisterProcessesClientResources();

                return View("Process", vmProcess);
            }

        }

        private async Task ProcesesSetViewBag()
        {

            // Departments
            List<DepartmentOption> departments = _options.GetDepartmentOptions();
            ViewBag.Departments = departments;

        }

        private void RegisterProcessesClientResources()
        {

            WebTemplateModel.HTMLBodyElements.Add("<script src=\"/assets/vendor/ckeditor5/build/ckeditor.js\"></script>");

            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/JusRichTextBoxFor.js?v={BuildId}\"></script>");

        }

        #endregion

        #region Private Schedules Methods

        // Typically Schedules would be rendered to the view as Process.Schedules (List<Schedule), then a partial view would be created to represent each Schedule in the list.
        // The Schedule.StartValue would be represented as a text box.
        // However, the Schedule concerns will be represented with a slider within the view. At this time I'm not sure how the model abstraction would be handled
        // in the view for the slider. So keeping the model simple / flat (Schedule.VmScheduleCandidateStart, Schedule.VmScheduleMembersStart, Schedule.VmScheduleMarkingStart)
        // and let the controller stick handle mapping these properties to proper Schedule objects.

        private void HandleScheduleModelState(VmProcess vmProcess)
        {

            if (vmProcess.VmScheduleCandidateStart > vmProcess.VmScheduleMembersStart)
                ModelState.AddModelError("VmScheduleCandidateStart", "Schedule Candidate Start cannot be > Schedule Members Start");

            if (vmProcess.VmScheduleMembersStart > vmProcess.VmScheduleMarkingStart)
                ModelState.AddModelError("VmScheduleMembersStart", "Schedule Members Start cannot be > Schedule Marking Start");

            if (vmProcess.VmScheduleMarkingStart > vmProcess.InterviewDuration)
                ModelState.AddModelError("VmScheduleMarkingStart", "Schedule Marking Start cannot be > Interview Duration");

        }

        private List<Schedule> GetSchedules(VmProcess vmProcess)
        {

            List<Schedule> result = new List<Schedule>();

            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Candidate, StartValue = vmProcess.VmScheduleCandidateStart });
            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Members, StartValue = vmProcess.VmScheduleMembersStart });
            result.Add(new Schedule() { ScheduleType = ScheduleTypes.Marking, StartValue = vmProcess.VmScheduleMarkingStart });

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

        private void PopulateProcessSchedulePropertiesFromSchedule(VmProcess process, List<Schedule> dbSchedules)
        {

            process.VmScheduleCandidateStart = dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Candidate).First().StartValue;
            process.VmScheduleMembersStart = dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Members).First().StartValue;
            process.VmScheduleMarkingStart = dbSchedules.Where(x => x.ScheduleType == ScheduleTypes.Marking).First().StartValue;

        }

        #endregion

    }

}
