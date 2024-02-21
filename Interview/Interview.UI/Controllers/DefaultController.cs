using AutoMapper;
using Azure.Core;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Default;
using Interview.UI.Models.Graph;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Graph;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using Process = Interview.Entities.Process;

namespace Interview.UI.Controllers
{
    public class DefaultController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IStringLocalizer<DefaultController> _localizer;
        private readonly IToken _tokenManager;
        private readonly IEmails _emailsManager;

        #endregion

        #region Constructors

        public DefaultController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IStringLocalizer<DefaultController> localizer, 
            IStringLocalizer<BaseController> baseLocalizer, IToken tokenManager, IEmails emailsManager) 
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
            _state = state;
            _localizer = localizer;
            _tokenManager = tokenManager;
            _emailsManager = emailsManager;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();
            List<Entities.Process> processes = await GetProcessesForLoggedInUser();
            Guid? processId = null;
            RoleUser roleUser = null;

            // Look to Session for ProcessId
            if (_state.ProcessId != null)
                processId = _state.ProcessId;
            // Look to first item in list if _state.ProcessId isn't set by user
            else if (processes.Any())
                processId = processes.First().Id;

            // Handle RoleUser and set various view bag properties
            roleUser = await GetAndHandleRoleUser(processId);
            await SetIndexViewBag(processes, processId, roleUser);
            SetLanguageStatusViewbagAndRegisterClientResources(roleUser);
            SetPrivacyStatementViewbagAndRegisterClientResources(roleUser);

            RegisterIndexClientResources();
            _state.ProcessId = processId;
            result.ProcessId = processId;

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchProcess(Guid processId)
        {

            _state.ProcessId = processId;

            return RedirectToAction("Index");

        }

		private async Task SetIndexViewBag(List<Entities.Process> processes, Guid? processId, RoleUser? roleUser)
        {

            ViewBag.Processes = processes;
            ViewBag.ProcessId = processId;
            ViewBag.UserHasAccess = roleUser != null;

            if (processId != null)
            {

                Entities.Process process = processes.Where(x => x.Id == processId).First();
				List<Interview.Entities.Interview> interviews = await _dal.GetInterViewsByProcessId((Guid)processId);
                List<VmInterview> vmInterviews = _mapper.Map<List<VmInterview>>(interviews);

				ViewBag.ProccessStartDate = process.StartDate;
				ViewBag.ProccessEndDate = process.EndDate;
				ViewBag.VmInterviews = vmInterviews;

            }

            ViewBag.ShowManageButtonsPartial = User.IsInRole(RoleTypes.Admin.ToString());

        }

        private void RegisterIndexClientResources()
        {

            WebTemplateModel.HTMLHeaderElements.Add("<link rel=\"stylesheet\" href=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/magnific-popup.css\" />");
            WebTemplateModel.HTMLBodyElements.Add("<script src=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/jquery.magnific-popup.min.js\"></script>");

            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/Index.js?v={BuildId}\"></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/InterviewModal.js?v={BuildId}\"></script>");

        }

        private async Task<RoleUser> GetAndHandleRoleUser(Guid? processId)
        {

            RoleUser result = null;

            if (processId != null)
            {
                result = await _dal.GetRoleUserByProcessIdAndUserId((Guid)processId, EntraId);

                if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.System.ToString()) ||
                    User.IsInRole(RoleTypes.Owner.ToString()))
                {
                    bool hasAccess = true;

                    if (User.IsInRole(RoleTypes.Owner.ToString()))
                    {
                        List<GroupOwner> groupOwners = await _dal.GetGroupOwnersByProcessIdAndUserId((Guid)processId, EntraId);
                        hasAccess = groupOwners.Count > 0;
                    }

                    // TODO: Come back to this...

                    //if (hasAccess)
                    //{
                    //    result = new RoleUser()
                    //    {
                    //        ProcessId = (Guid)processId,
                    //        RoleType = RoleTypes.Admin,
                    //        UserId = EntraId,
                    //        LanguageType = LanguageTypes.Bilingual,
                    //        HasAcceptedPrivacyStatement = true,
                    //    };
                    //    await _dal.AddEntity<RoleUser>(result);
                    //}

                }
            }

            return result;

        }

        #endregion

        #region Interview Modal

        [HttpGet]
        public async Task<PartialViewResult> InterViewModal(Guid? id)
        {

            VmInterview result = null;
            Interview.Entities.Interview interview = null;

            if (id == null)
                result = new VmInterview() { ProcessId = (Guid)_state.ProcessId };
            else
            {
                interview = await _dal.GetEntity<Interview.Entities.Interview>((Guid)id, true) as Interview.Entities.Interview;
                result = _mapper.Map<VmInterview>(interview);

                result.VmInterviewerUserIds.CandidateUserId = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).FirstOrDefault()?.UserId;
                result.VmInterviewerUserIds.InterviewerUserId = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Interviewer).FirstOrDefault()?.UserId;
                result.VmInterviewerUserIds.InterviewerLeadUserId = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Lead).FirstOrDefault()?.UserId;
                result.VmInterviewerUserIds.InterviewId = id;
            }

			await SetInterviewModalViewBag(interview);

			return PartialView(result);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InterviewModal(VmInterview vmInterview)
        {

            await HandleInterviewModalModelState(vmInterview);

            if (ModelState.IsValid)
            {

                Interview.Entities.Interview interview = _mapper.Map<Interview.Entities.Interview>(vmInterview);
                interview.Status = InterviewStates.Waiting;

                // Handle Interview
                interview.ProcessId = (Guid)_state.ProcessId;
                if (vmInterview.Id == null)
                {
                    vmInterview.Id = await _dal.AddEntity<Interview.Entities.Interview>(interview);
                }
                else
                {
                    await _dal.UpdateEntity(interview);

                    // Handle Users
                    List<InterviewUser> dbInterviewUsers = await _dal.GetInterviewUsersByInterviewId((Guid)vmInterview.Id);
                    await ResolveInterviewUser(vmInterview.VmInterviewerUserIds.CandidateUserId, dbInterviewUsers, RoleUserTypes.Candidate, (Guid)vmInterview.Id);
                    await ResolveInterviewUser(vmInterview.VmInterviewerUserIds.InterviewerUserId, dbInterviewUsers, RoleUserTypes.Interviewer, (Guid)vmInterview.Id);
                    await ResolveInterviewUser(vmInterview.VmInterviewerUserIds.InterviewerLeadUserId, dbInterviewUsers, RoleUserTypes.Lead, (Guid)vmInterview.Id);

                    // Email Candidate (given the UX of InterviewModal, the Candiate is only added when updating, so send email here).
                    if (vmInterview.VmInterviewerUserIds.CandidateUserId != null)
                    {
                        InterviewUser dbCandidateInterviewUser = dbInterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).FirstOrDefault();

                        // Check to see if Candidate user has changed
                        if (dbCandidateInterviewUser == null || 
                            (dbCandidateInterviewUser != null && dbCandidateInterviewUser.UserId != vmInterview.VmInterviewerUserIds.CandidateUserId))
                            await SendInterviewEmailToCandiate(vmInterview);
                    }

                }

                return new JsonResult(new { result = true, item = vmInterview })
                {
                    StatusCode = 200
                };

            }
            else
            {
                Interview.Entities.Interview interview = vmInterview.Id == null ? null : await _dal.GetEntity<Interview.Entities.Interview>((Guid)vmInterview.Id, true) as Interview.Entities.Interview;

                await SetInterviewModalViewBag(interview);

                return PartialView(vmInterview);
            }

        }

        [HttpGet]
        public async Task<ActionResult> InterviewDelete(Guid id)
        {

            await _dal.DeleteEntity<Interview.Entities.Interview>(id);

            return RedirectToAction("Index");

        }

        private async Task SetInterviewModalViewBag(Interview.Entities.Interview? interview)
        {

            // Handle Interview Schedule
            if (interview != null)
            {

                List<Schedule> schedules = await _dal.GetSchedulesByProcessId(interview.ProcessId);
                TimeSpan startTime = interview.StartDate.TimeOfDay;
                TimeSpan candidateArrival = new TimeSpan(0, (int)schedules.Where(x => x.ScheduleType == ScheduleTypes.Candidate).First().StartValue, 0);
                TimeSpan interviewerArrival = new TimeSpan(0, (int)schedules.Where(x => x.ScheduleType == ScheduleTypes.Members).First().StartValue, 0);
                TimeSpan marking = new TimeSpan(0, (int)schedules.Where(x => x.ScheduleType == ScheduleTypes.Marking).First().StartValue, 0);

                ViewBag.CandidateArrival = candidateArrival.Add(startTime).ToString(@"hh\:mm");
                ViewBag.InterviewerArrival = interviewerArrival.Add(startTime).ToString(@"hh\:mm");
                ViewBag.Marking = marking.Add(startTime).ToString(@"hh\:mm"); ;

            }
            else
            {
                ViewBag.CandidateArrival = string.Empty;
                ViewBag.InterviewerArrival = string.Empty;
                ViewBag.Marking = string.Empty;
            }

            // Handle Interview Start and End Dates
            Entities.Process process = await _dal.GetEntity<Entities.Process>((Guid)_state.ProcessId) as Entities.Process;
			ViewBag.ProccessStartDate = process.StartDate;
			ViewBag.ProccessEndDate = process.EndDate;

			// Handle Interview Users
			List<RoleUser> roleUsers = await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
			if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.Owner.ToString()) || User.IsInRole(RoleTypes.System.ToString()))
			{
				bool hasAccess = true;
				if (User.IsInRole(RoleTypes.Owner.ToString()))
				{
					// Despit the above line's dal call returning a list, it treats the returned type as a single entity, so need to 
					// get the list as a variable first. Moving on...
					var groupOwners = await _dal.GetGroupOwnersByContextIdAndUserId((Guid)_state.ProcessId, EntraId);
					hasAccess = groupOwners.Any();
				}
			}

			ViewBag.CandidateUsers = roleUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).ToList();
			ViewBag.InterviewerUsers = roleUsers.Where(x => x.RoleUserType == RoleUserTypes.Interviewer).ToList();
			ViewBag.LeadUsers = roleUsers.Where(x => x.RoleUserType == RoleUserTypes.Lead).ToList();

		}

        private async Task ResolveInterviewUser(Guid? postedUserId, List<InterviewUser> dbInterviewUsers, RoleUserTypes roleUserType, Guid interviewId)
        {

            InterviewUser dbInterviewUser = dbInterviewUsers.Where(x => x.RoleUserType == roleUserType).FirstOrDefault();

            if (dbInterviewUser == null && postedUserId != null)
            {
                // Add
                InterviewUser newInterviewUser = new InterviewUser()
                {
                    UserId = (Guid)postedUserId,
                    RoleUserType = roleUserType,
                    InterviewId = interviewId
                };
                await _dal.AddEntity<InterviewUser>(newInterviewUser);
            }
            else if ((dbInterviewUser != null && postedUserId != null) && (dbInterviewUser.Id != postedUserId))
            {
                // Update
                InterviewUser newInterviewUser = new InterviewUser()
                {
                    UserId = (Guid)postedUserId,
                    RoleUserType = roleUserType,
                    InterviewId = interviewId
                };
                await _dal.DeleteEntity(dbInterviewUser);
                await _dal.AddEntity<InterviewUser>(newInterviewUser);
            }
            else if (dbInterviewUser != null && postedUserId == null)
            {
                // Delete
                await _dal.DeleteEntity(dbInterviewUser);
            }

        }

        private async Task HandleInterviewModalModelState(VmInterview vmInterview)
        {

            if (vmInterview.VmStartTime != null && vmInterview.Duration != null)
            {

                Entities.Process process = await _dal.GetEntity<Entities.Process>((Guid)_state.ProcessId) as Entities.Process;
                //DateTime interviewEndTime = DateTime.Now.AddMinutes(vmInterview.VmStartTime.TotalMinutes).AddMinutes((int)vmInterview.Duration);
                DateTime interviewEndTime = new DateTime().AddMinutes(vmInterview.VmStartTime.TotalMinutes).AddMinutes((int)vmInterview.Duration);

                if (vmInterview.VmStartTime < process.MinTime || interviewEndTime.TimeOfDay > process.MaxTime)
                {
                    ModelState.AddModelError("VmStartTime", _localizer["InterviewVmStartDateAndDuration"].Value
                        .Replace("{min}", ((TimeSpan)process.MinTime).ToString())
                        .Replace("{max}", ((TimeSpan)process.MaxTime).ToString()));
                }

            }

        }

        private async Task SendInterviewEmailToCandiate(VmInterview vmInterview)
        {

            Process process = await _dal.GetEntity<Process>((Guid)_state.ProcessId) as Process;
            EmailTemplate emailTemplate = await GetEmailTemplateForInterviewEmailToCandiate(); 
            List<Schedule> schedules = await _dal.GetSchedulesByProcessId((Guid)_state.ProcessId);
            RoleUser externalRoleUser = await _dal.GetEntity<RoleUser>((Guid)vmInterview.VmInterviewerUserIds.CandidateUserId) as RoleUser;
            ExternalUser externalUser = await _dal.GetEntity<ExternalUser>((Guid)externalRoleUser.UserId) as ExternalUser;
            TokenResponse tokenResponse = await _tokenManager.GetToken();

            if (emailTemplate != null && schedules != null)
            {

                string noProcess = process.NoProcessus;
                string groupNiv = process.GroupNiv;
                string startDate = vmInterview.VmStartDate.ToLongDateString();
                string startTime = vmInterview.VmStartDate.ToLongTimeString();
                string startOral = "Figure this out";
                string location = $"{vmInterview.Location} {vmInterview.Room}";
                string contactName = string.IsNullOrEmpty(vmInterview.ContactName) ? process.ContactName : vmInterview.ContactName;
                string contactNumber = string.IsNullOrEmpty(vmInterview.ContactNumber) ? process.ContactNumber : vmInterview.ContactNumber;
                List<EmailRecipent> toRecipients = _emailsManager.GetEmailRecipients(externalUser.Email);

                string body = emailTemplate.EmailBody
                    .Replace("{0}", noProcess)
                    .Replace("{1}", groupNiv)
                    .Replace("{2}", startDate)
                    .Replace("{3}", startTime)
                    .Replace("{4}", startOral)
                    .Replace("{5}", location)
                    .Replace("{6}", contactName)
                    .Replace("{7}", contactNumber);

                EmailEnvelope emailEnvelope = new EmailEnvelope()
                {
                    message = new EmailMessage()
                    {
                        subject = emailTemplate.EmailSubject,
                        body = new EmailBody()
                        {
                            contentType = "HTML",
                            content = body
                        },
                        toRecipients = toRecipients,
                        //ccRecipients = GetEmailRecipients(emailTemplate.CcRecipients),
                    },
                    saveToSentItems = "false"
                };
                HttpResponseMessage responseMessage = await _emailsManager.SendEmailAsync(emailEnvelope, tokenResponse.access_token, User.Identity.Name);

            }

        }

        private async Task<EmailTemplate> GetEmailTemplateForInterviewEmailToCandiate()
        {

            EmailTemplate result = null;
            List<EmailTemplate> emailTemplates = await _dal.GetEmailTemplatesByProcessId((Guid)_state.ProcessId);

            if (User.IsInRole(RoleTypes.Admin.ToString()))
                result = emailTemplates.Where(x => x.EmailType == EmailTypes.CandidateAddedByHR).FirstOrDefault();
            else
                result = emailTemplates.Where(x => x.EmailType == EmailTypes.CandidateRegisteredTimeSlot).FirstOrDefault();

            return result;
        
        }

        #endregion

        #region Language Status Modal

        [HttpGet]
        public PartialViewResult LanguageStatusModal()
        {

            VmLanguageStatusModal result = new VmLanguageStatusModal();

            return PartialView(result);

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> LanguageStatusModal(VmLanguageStatusModal vmLanguageStatusModal)
        {

            if (ModelState.IsValid)
            {
                RoleUser roleUser = await _dal.GetRoleUserByProcessIdAndUserId((Guid)_state.ProcessId, EntraId);

                roleUser.LanguageType = vmLanguageStatusModal.LanguageType;
                await _dal.UpdateEntity(roleUser);

                return new JsonResult(new { result = true, item = vmLanguageStatusModal })
                {
                    StatusCode = 200
                };
            }
            else
            {
                return PartialView(vmLanguageStatusModal);
            }

        }

        private void SetLanguageStatusViewbagAndRegisterClientResources(RoleUser roleUser)
        {

            bool showLanguageStatusModal = false;

            if (roleUser != null && roleUser.LanguageType == null)
            {
                showLanguageStatusModal = true;
                WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/LanguageStatusModal.js?v={BuildId}\"></script>");
            }

            ViewBag.ShowLanguageStatusModal = showLanguageStatusModal;

        }

        #endregion

        #region Privacy Statement Modal

        [HttpGet]
        public async Task<PartialViewResult> PrivacyStatementModal()
        {

            VmPrivacyStatementModal result = new VmPrivacyStatementModal();

            await SetPrivacyStatementModalViewBag();

            return PartialView(result);

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> PrivacyStatementModal(VmPrivacyStatementModal vmPrivacyStatementModal)
        {

            if (ModelState.IsValid)
            {
                RoleUser roleUser = await _dal.GetRoleUserByProcessIdAndUserId((Guid)_state.ProcessId, EntraId);

                roleUser.HasAcceptedPrivacyStatement = vmPrivacyStatementModal.HasAcceptedPrivacyStatement;
                await _dal.UpdateEntity(roleUser);

                return new JsonResult(new { result = true, item = vmPrivacyStatementModal })
                {
                    StatusCode = 200
                };
            }
            else
            {
                await SetPrivacyStatementModalViewBag();
                return PartialView(vmPrivacyStatementModal);
            }
            
        }

        private async Task SetPrivacyStatementModalViewBag()
        {

            Entities.Process process = await _dal.GetEntity<Entities.Process>((Guid)_state.ProcessId) as Entities.Process;
            RoleUser roleUser = await _dal.GetRoleUserByProcessIdAndUserId((Guid)_state.ProcessId, EntraId);

            ViewBag.MessageKey = roleUser.RoleUserType == RoleUserTypes.Candidate ? "PrivacyStatementCandidate" : "PrivacyStatementBoardMember";
            ViewBag.EmailSentFrom = process.EmailServiceSentFrom;

        }

        private void SetPrivacyStatementViewbagAndRegisterClientResources(RoleUser roleUser)
        {

            bool showPrivacyStatementModal = false;

            if (roleUser != null && !roleUser.HasAcceptedPrivacyStatement && (roleUser.RoleUserType == RoleUserTypes.Candidate || roleUser.RoleUserType == RoleUserTypes.Interviewer
                || roleUser.RoleUserType == RoleUserTypes.Lead))
            {
                showPrivacyStatementModal = true;
                WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/PrivacyStatementModal.js?v={BuildId}\"></script>");
            }

            ViewBag.ShowPrivacyStatementModal = showPrivacyStatementModal;

        }

        #endregion

    }
}
