using AutoMapper;
using Azure.Core;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Migrations;
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
using System.Text;
using InterviewUserEmail = Interview.Entities.InterviewUserEmail;
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
        private readonly IUsers _usersManager;

        #endregion

        #region Constructors

        public DefaultController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IStringLocalizer<DefaultController> localizer,
            IStringLocalizer<BaseController> baseLocalizer, IToken tokenManager, IEmails emailsManager, IUsers usersManager) 
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
            _state = state;
            _localizer = localizer;
            _tokenManager = tokenManager;
            _emailsManager = emailsManager;
            _usersManager = usersManager;
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
            RegisterIndexClientResources();
            SetLanguageStatusViewbagAndRegisterClientResources(roleUser);
            SetPrivacyStatementViewbagAndRegisterClientResources(roleUser);
            HandleNotification();
        
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

            if (processes.Count != 0 && processId != null)
            {

                Entities.Process process = processes.Where(x => x.Id == processId).First();
				List<Interview.Entities.Interview> interviews = await _dal.GetInterViewsByProcessId((Guid)processId);
                List<VmInterview> vmInterviews = _mapper.Map<List<VmInterview>>(interviews);
                List<VmRoleUser> vmRoleUsers = _mapper.Map<List<VmRoleUser>>(process.RoleUsers);

                foreach (VmInterview vmInterview in vmInterviews)
                {
                    foreach (VmInterviewUser vmInterviewUser in vmInterview.InterviewUsers)
                    {
                        vmInterview.VmInterviewerUserIds.CandidateUserId = vmInterview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).FirstOrDefault()?.RoleUserId;
                        vmInterview.VmInterviewerUserIds.InterviewerUserId = vmInterview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Interviewer).FirstOrDefault()?.RoleUserId;
                        vmInterview.VmInterviewerUserIds.InterviewerLeadUserId = vmInterview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Lead).FirstOrDefault()?.RoleUserId;
                        vmInterview.VmInterviewerUserIds.InterviewId = vmInterview.Id;
                    }
                }

				ViewBag.ProccessStartDate = process.StartDate;
				ViewBag.ProccessEndDate = process.EndDate;
				ViewBag.VmInterviews = vmInterviews;
                ViewBag.VmRoleUsers = vmRoleUsers;

            }

            ViewBag.ShowManageButtonsPartial = User.IsInRole(RoleTypes.Admin.ToString());

        }

        private void RegisterIndexClientResources()
        {

            WebTemplateModel.HTMLHeaderElements.Add("<link rel=\"stylesheet\" href=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/magnific-popup.css\" />");
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-DataTables/datatables.min.css'>");

            WebTemplateModel.HTMLBodyElements.Add("<script src=\"/lib/Magnific-Popup-master/Magnific-Popup-master/dist/jquery.magnific-popup.min.js\"></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-DataTables/datatables.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/Index.js?v={BuildId}\"></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/InterviewTable.js?v={BuildId}\"></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/InterviewModal.js?v={BuildId}\"></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Default/ParticipantsModal.js?v={BuildId}\"></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/DeleteConfirmationModal.js?v={BuildId}'></script>");

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

        #region Interview Modal

        [HttpGet]
        public async Task<PartialViewResult> InterViewModal(Guid? id)
        {

            VmInterviewModal result = null;
            Interview.Entities.Interview interview = null;

            if (id == null)
                result = new VmInterviewModal() { ProcessId = (Guid)_state.ProcessId };
            else
            {
                interview = await _dal.GetEntity<Entities.Interview>((Guid)id, true) as Entities.Interview;
                result = _mapper.Map<VmInterviewModal>(interview);
            }

			await SetInterviewModalViewBag(interview);

			return PartialView(result);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InterviewModal(VmInterviewModal vmInterviewModal)
        {

            await HandleInterviewModalModelState(vmInterviewModal);

            if (ModelState.IsValid)
            {

                Interview.Entities.Interview interview = _mapper.Map<Interview.Entities.Interview>(vmInterviewModal);
                
                // Handle Interview
                interview.ProcessId = (Guid)_state.ProcessId;
                if (vmInterviewModal.Id == null)
                {
                    interview.Status = InterviewStates.PendingCommitteeMembers;
                    vmInterviewModal.Id = await _dal.AddEntity<Interview.Entities.Interview>(interview);
                }
                else
                {
                    await _dal.UpdateEntity(interview);
                }

                return new JsonResult(new { result = true, item = vmInterviewModal })
                {
                    StatusCode = 200
                };

            }
            else
            {
                Interview.Entities.Interview interview = vmInterviewModal.Id == null ? null : await _dal.GetEntity<Interview.Entities.Interview>((Guid)vmInterviewModal.Id, true) as Interview.Entities.Interview;

                await SetInterviewModalViewBag(interview);

                return PartialView(vmInterviewModal);
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

		}

        private async Task HandleInterviewModalModelState(VmInterviewModal vmInterviewModal)
        {

            if (vmInterviewModal.VmStartTime != null && vmInterviewModal.Duration != null)
            {

                Entities.Process process = await _dal.GetEntity<Entities.Process>((Guid)_state.ProcessId) as Entities.Process;
                //DateTime interviewEndTime = DateTime.Now.AddMinutes(vmInterview.VmStartTime.TotalMinutes).AddMinutes((int)vmInterview.Duration);
                DateTime interviewEndTime = new DateTime().AddMinutes(vmInterviewModal.VmStartTime.TotalMinutes).AddMinutes((int)vmInterviewModal.Duration);

                if (vmInterviewModal.VmStartTime < process.MinTime || interviewEndTime.TimeOfDay > process.MaxTime)
                {
                    ModelState.AddModelError("VmStartTime", _localizer["InterviewVmStartDateAndDuration"].Value
                        .Replace("{min}", ((TimeSpan)process.MinTime).ToString())
                        .Replace("{max}", ((TimeSpan)process.MaxTime).ToString()));
                }

            }

        }

        #endregion

        #region Participants Modal

        [HttpGet]
        public async Task<PartialViewResult> ParticipantsModal(Guid id)
        {

            VmParticipantsModal result = new VmParticipantsModal();
            Entities.Interview interview = await _dal.GetEntity<Entities.Interview>((Guid)id, true) as Entities.Interview;

            result.InterviewId = id;
            result.CandidateUserId = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).FirstOrDefault()?.RoleUserId;
            result.InterviewerUserIds = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Interviewer).ToList().Select(x => x.RoleUserId).ToList();
            result.InterviewerLeadUserIds = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Lead).ToList().Select(x => x.RoleUserId).ToList();

            await SetParticipantsModalViewBag();

            return PartialView(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ParticipantsModal(VmParticipantsModal vmParticipantsModal)
        {

            Entities.Interview interview = await _dal.GetEntity<Entities.Interview>(vmParticipantsModal.InterviewId, true) as Entities.Interview;

            // Handle Users
            List<InterviewUser> dbInterviewUsers = await _dal.GetInterviewUsersByInterviewId((Guid)vmParticipantsModal.InterviewId);
            List<InterviewUserAction> interviewUserActions = new List<InterviewUserAction>();
            List<InterviewUserAction> roleSpecificInterviewUserActions;

            // Charge list of interviewUserActions so emails can be sent accordingly.
            // Candidates
            roleSpecificInterviewUserActions = await ResolveCandidateUser(vmParticipantsModal.CandidateUserId, dbInterviewUsers, RoleUserTypes.Candidate, (Guid)vmParticipantsModal.InterviewId);
            interviewUserActions.AddRange(roleSpecificInterviewUserActions);
            // Board Members
            roleSpecificInterviewUserActions = await ResolveInterviewUsers(vmParticipantsModal.InterviewerUserIds, dbInterviewUsers, RoleUserTypes.Interviewer, (Guid)vmParticipantsModal.InterviewId);
            interviewUserActions.AddRange(roleSpecificInterviewUserActions);
            // Lead Board Members
            roleSpecificInterviewUserActions = await ResolveInterviewUsers(vmParticipantsModal.InterviewerLeadUserIds, dbInterviewUsers, RoleUserTypes.Lead, (Guid)vmParticipantsModal.InterviewId);
            interviewUserActions.AddRange(roleSpecificInterviewUserActions);

            // Handle Emails
            dbInterviewUsers = await _dal.GetInterviewUsersByInterviewId((Guid)vmParticipantsModal.InterviewId);
            await HandleInterviewerEmails(interview, interviewUserActions, dbInterviewUsers);
            await HandleInterviewUserActions(interviewUserActions);

            // Handle Notification
            HandleInterviewUserNotification(interviewUserActions, dbInterviewUsers);

            interview.Status = GetInterviewState(vmParticipantsModal);
            await _dal.UpdateEntity(interview);

            return new JsonResult(new { result = true })
            {
                StatusCode = 200
            };

        }

        private async Task SetParticipantsModalViewBag()
        {

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

        private async Task<List<InterviewUserAction>> ResolveInterviewUsers(List<Guid> postedUserIds, List<InterviewUser> dbInterviewUsers, RoleUserTypes roleUserType, Guid interviewId)
        {

            List<InterviewUserAction> result = new List<InterviewUserAction>();
            List<InterviewUser> filteredDbUsers = dbInterviewUsers.Where(x => x.RoleUserType == roleUserType).ToList();
            Guid? addedInterviewUserId = null;
            InterviewUser interviewUser;

            foreach (Guid postedUserId in postedUserIds)
            {
                interviewUser = filteredDbUsers.Where(x => x.RoleUserId == postedUserId).FirstOrDefault();
                if (interviewUser == null)
                {
                    // Add
                    InterviewUser newInterviewUser = new InterviewUser()
                    {
                        RoleUserId = (Guid)postedUserId,
                        RoleUserType = roleUserType,
                        InterviewId = interviewId
                    };
                    addedInterviewUserId = await _dal.AddEntity<InterviewUser>(newInterviewUser);
                    result.Add(new InterviewUserAction() { InterviewUserId = (Guid)addedInterviewUserId, InterviewUserActionType = InterviewUserActionTypes.Added });
                }
            }

            foreach (InterviewUser dbInterviewUser in filteredDbUsers)
            {
                if (!postedUserIds.Contains(dbInterviewUser.RoleUserId))
                {
                    // Delete
                    await _dal.DeleteEntity(dbInterviewUser);
                    result.Add(new InterviewUserAction() { InterviewUserId = dbInterviewUser.Id, InterviewUserActionType = InterviewUserActionTypes.Removed });
                }
            }
            
            return result;

        }

        private async Task<List<InterviewUserAction>> ResolveCandidateUser(Guid? postedUserId, List<InterviewUser> dbInterviewUsers, RoleUserTypes roleUserType, Guid interviewId)
        {

            List<InterviewUserAction> result = new List<InterviewUserAction>();
            Guid? addedInterviewUserId = null;
            InterviewUser dbInterviewUser = dbInterviewUsers.Where(x => x.RoleUserType == roleUserType).FirstOrDefault();

            if (dbInterviewUser == null && postedUserId != null)
            {
                // Add
                InterviewUser newInterviewUser = new InterviewUser()
                {
                    RoleUserId = (Guid)postedUserId,
                    RoleUserType = roleUserType,
                    InterviewId = interviewId
                };
                addedInterviewUserId = await _dal.AddEntity<InterviewUser>(newInterviewUser);
                result.Add(new InterviewUserAction() { InterviewUserId = (Guid)addedInterviewUserId, InterviewUserActionType = InterviewUserActionTypes.Added});
            }
            else if ((dbInterviewUser != null && postedUserId != null) && (dbInterviewUser.RoleUserId != postedUserId))
            {
                // Update
                InterviewUser newInterviewUser = new InterviewUser()
                {
                    RoleUserId = (Guid)postedUserId,
                    RoleUserType = roleUserType,
                    InterviewId = interviewId
                };
                await _dal.DeleteEntity(dbInterviewUser);
                result.Add(new InterviewUserAction() { InterviewUserId = dbInterviewUser.Id, InterviewUserActionType = InterviewUserActionTypes.Removed });
                addedInterviewUserId = await _dal.AddEntity<InterviewUser>(newInterviewUser);
                result.Add(new InterviewUserAction() { InterviewUserId = (Guid)addedInterviewUserId, InterviewUserActionType = InterviewUserActionTypes.Added });
            }
            else if (dbInterviewUser != null && postedUserId == null)
            {
                // Delete
                await _dal.DeleteEntity(dbInterviewUser);
                result.Add(new InterviewUserAction() { InterviewUserId = dbInterviewUser.Id, InterviewUserActionType = InterviewUserActionTypes.Removed });
            }

            return result;

        }

        private InterviewStates GetInterviewState(VmParticipantsModal vmParticipantsModal)
        {

            InterviewStates? result = InterviewStates.PendingCommitteeMembers;
            int interviewerCount = vmParticipantsModal.InterviewerUserIds.Count + vmParticipantsModal.InterviewerLeadUserIds.Count;
            bool hasALeadInterviewer = vmParticipantsModal.InterviewerLeadUserIds.Any();
            bool hasCandidate = vmParticipantsModal.CandidateUserId != null;

            if (interviewerCount == 3)
            {
                if (hasALeadInterviewer)
                    result = InterviewStates.AvailableForCandidate;
                else
                    result = InterviewStates.PendingCommitteeMembers;
            }

            if (result == InterviewStates.AvailableForCandidate && hasCandidate)
                result = InterviewStates.Booked;

            return (InterviewStates)result;

        }

        private async Task HandleInterviewerEmails(Entities.Interview interview, List<InterviewUserAction> interviewUserActions, List<InterviewUser> dbInterviewUsers)
        {
  
            InterviewUser interviewUser = null;
            VmInterview vmInterview = _mapper.Map<VmInterview>(interview);
            List<EmailTemplate> emailTemplates = await _dal.GetEmailTemplatesByProcessId((Guid)_state.ProcessId);
            EmailEnvelope emailEnvelope = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();
            HttpResponseMessage responseMessage = null;

            foreach (InterviewUserAction interviewUserAction in interviewUserActions)
            {

                if (interviewUserAction.InterviewUserActionType == InterviewUserActionTypes.Added)
                {

                    // Handle emailing
                    interviewUser = dbInterviewUsers.Where(x => x.Id == interviewUserAction.InterviewUserId).First();

                    if (interviewUser.RoleUser.RoleUserType == RoleUserTypes.Candidate)
                    {
                        if (User.IsInRole(RoleTypes.Admin.ToString()))
                            emailEnvelope = await GetEmailEnvelopeForCandidateAddedByHR(emailTemplates, vmInterview, interviewUser.RoleUser, tokenResponse);
                        else
                            emailEnvelope = await GetEmailEnvelopeForCandidateRegisteredTimeSlot(emailTemplates, vmInterview, interviewUser.RoleUser, tokenResponse);
                    }

                    responseMessage = await _emailsManager.SendEmailAsync(emailEnvelope, tokenResponse.access_token, User.Identity.Name);

                }
 
            }

        }

        private async Task<EmailEnvelope> GetEmailEnvelopeForCandidateAddedByHR(List<EmailTemplate> emailTemplates, VmInterview vmInterview, RoleUser roleUser,
            TokenResponse tokenResponse)
        {

            EmailEnvelope result = null;
            EmailTemplate emailTemplate = emailTemplates.Where(x => x.EmailType == EmailTypes.CandidateAddedByHR).FirstOrDefault();
            Process process = await _dal.GetEntity<Process>((Guid)_state.ProcessId, true) as Process;
            string email;
            string callbackUrl = null;

            if ((bool)roleUser.IsExternal)
            {
                ExternalUser externalUser = await _dal.GetEntity<ExternalUser>((Guid)roleUser.UserId) as ExternalUser;
                email = roleUser.ExternalUserEmail;
                callbackUrl = GetCallbackUrl(process.Id, externalUser.Id);
            }
            else
            {
                GraphUser graphUser = await _usersManager.GetUserInfoAsync(roleUser.UserId.ToString(), tokenResponse.access_token);
                email = graphUser.mail;
                callbackUrl = GetCallbackUrl(process.Id, null);
            }

            result = _emailsManager.GetEmailEnvelopeForCandidateAddedByHR(emailTemplate, process, vmInterview, email, callbackUrl);

            return result;

        }

        private async Task<EmailEnvelope> GetEmailEnvelopeForCandidateRegisteredTimeSlot(List<EmailTemplate> emailTemplates, VmInterview vmInterview, RoleUser roleUser, 
            TokenResponse tokenResponse)
        {

            EmailEnvelope result = null;
            EmailTemplate emailTemplate = emailTemplates.Where(x => x.EmailType == EmailTypes.CandidateRegisteredTimeSlot).FirstOrDefault();
            Process process = await _dal.GetEntity<Process>((Guid)_state.ProcessId, true) as Process;            
            string email;
            string callbackUrl = null;

            if ((bool)roleUser.IsExternal)
            {
                ExternalUser externalUser = await _dal.GetEntity<ExternalUser>((Guid)roleUser.UserId) as ExternalUser;
                email = roleUser.ExternalUserEmail;
                callbackUrl = GetCallbackUrl(process.Id, externalUser.Id);
            }
            else
            {
                GraphUser graphUser = await _usersManager.GetUserInfoAsync(roleUser.UserId.ToString(), tokenResponse.access_token);
                email = graphUser.mail;
                callbackUrl = GetCallbackUrl(process.Id, null);
            }

            result = _emailsManager.GetEmailEnvelopeForCandidateRegisteredTimeSlot(emailTemplate, process, vmInterview, email, callbackUrl);

            return result;

        }

        private string GetCallbackUrl(Guid processId, Guid? externalCandidateId)
        {

            string result;

            if (externalCandidateId == null)
            {
                result = Url.ActionLink(
                    action: "Internal",
                    controller: "Candidates",
                    new
                    {
                        processId = processId
                    },
                    protocol: Request.Scheme,
                    host: HostName
                );
            }
            else
            {
                result = Url.ActionLink(
                    action: "External",
                    controller: "Candidates",
                    new
                    {
                        processId = processId,
                        externalCandidateId = externalCandidateId
                    },
                    protocol: Request.Scheme,
                    host: HostName
                );
            }

            return result;

        }

        private async Task HandleInterviewUserActions(List<InterviewUserAction> interviewUserActions)
        {

            InterviewUserEmail interviewUserEmail = null;

            foreach (InterviewUserAction interviewUserAction in interviewUserActions)
            {
                if (interviewUserAction.InterviewUserActionType == InterviewUserActionTypes.Added)
                {
                    // Add
                    interviewUserEmail = new InterviewUserEmail() { InterviewUserId = interviewUserAction.InterviewUserId, EmailType = EmailTypes.CandidateRegisteredTimeSlot, DateSent = DateTime.Now };
                    await _dal.AddEntity<InterviewUserEmail>(interviewUserEmail);

                }
                else if (interviewUserAction.InterviewUserActionType == InterviewUserActionTypes.Removed)
                {
                    // No need to delete coresponding InterviewUserEmail because it was deleted when the InterviewUser was deleted.
                }
            }

        }

        private void HandleInterviewUserNotification(List<InterviewUserAction> interviewUserActions, List<InterviewUser> dbInterviewUsers)
        {
            
            StringBuilder sb = new StringBuilder();
            InterviewUser interviewUser = null;
            List<InterviewUserAction> addedInterviewUserActions = interviewUserActions.Where(x => x.InterviewUserActionType == InterviewUserActionTypes.Added).ToList();

            if (addedInterviewUserActions.Any())
            {
                sb.Append(_localizer["InterviewUserNotificationPrefix"].Value);
                sb.Append("<ul>");
                foreach (InterviewUserAction interviewUserAction in addedInterviewUserActions)
                {
                    interviewUser = dbInterviewUsers.Where(x => x.Id == interviewUserAction.InterviewUserId).First();
                    sb.Append($"<li>{interviewUser.RoleUser.UserFirstname} {interviewUser.RoleUser.UserLastname}</li>");
                }
                sb.Append("</ul>");
                sb.Append(_localizer["InterviewUserNotificationSuffix"].Value);

                _state.NoticationMessage = sb.ToString();
            }

        }

        #endregion

        #region Delete Interview Modal

        [HttpGet]
        public PartialViewResult DeleteInterviewModal(Guid id)
        {

            return ConfirmDeleteModal(id, _localizer["DeleteConfirmationString"].Value);

        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInterviewModal(Guid id, bool hardDelete = false)
        {

            await _dal.DeleteEntity<Entities.Interview>(id);

            return new JsonResult(new { result = true, id = id })
            {
                StatusCode = 200
            };

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