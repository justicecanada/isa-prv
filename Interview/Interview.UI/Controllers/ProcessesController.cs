﻿using AutoMapper;
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
using System.Diagnostics;
using Process = Interview.Entities.Process;

namespace Interview.UI.Controllers
{
    public class ProcessesController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IOptions _options;
        private readonly IStringLocalizer<ProcessesController> _localizer;

        #endregion

        #region Constructors

        public ProcessesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IOptions options, 
            IStringLocalizer<BaseController> baseLocalizer, IStringLocalizer<ProcessesController> localizer, 
            IOptions<SessionTimeoutOptions> sessionTimeoutOptions) 
            : base(modelAccessor, dal, baseLocalizer, sessionTimeoutOptions)
        {
            _mapper = mapper;
            _state = state;
            _options = options;
            _localizer = localizer;
        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            await IndexSetViewBag();
            IndexRegisterClientResources();
            HandleCommonPageMethods();

            return View();

        }

        [HttpGet]
        public PartialViewResult DeleteProcessModal(Guid id)
        {

            return ConfirmDeleteModal(id, _localizer["DeleteConfirmationString"].Value);

        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProcessModal(Guid id, bool hardDelete = false)
        {

            await _dal.DeleteEntity<Process>(id);

            if (_state.ProcessId == id)
                _state.ProcessId = null;

            Notify(_localizer["NotifyDeleteSuccess"].Value, "success");

            return new JsonResult(new { result = true, id = id })
            {
                StatusCode = 200
            };

        }

        public async Task IndexSetViewBag()
        {

            List<Process> processes = await GetProcessesForLoggedInUser();
            List<DepartmentOption> departments = _options.GetDepartmentOptions();

            ViewBag.Processes = processes;
            ViewBag.Departments = departments;

        }

        private void IndexRegisterClientResources()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css'>");
            WebTemplateModel.HTMLHeaderElements.Add("<link rel=\"stylesheet\" href=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/magnific-popup.css\" />");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/DeleteConfirmationModal.js?v={BuildId}'></script>");

        }

        private void HandleNotification()
        {

            string notificationMessage = _state.NoticationMessage;

            if (!string.IsNullOrEmpty(notificationMessage))
            {
                Notify(notificationMessage, "success");
                _state.NoticationMessage = null;
            }

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
            HandleCommonPageMethods();

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

                _state.ProcessId = processId;

                return RedirectToAction("Index", "Emails", new { id = vmProcess.Id });
            }
            else
            {
                await ProcesesSetViewBag();
                RegisterProcessesClientResources();
                HandleCommonPageMethods();

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
                HandleCommonPageMethods();

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
