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
                        vmInterview.VmInterviewerUserIds.CandidateUserId = vmInterview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).FirstOrDefault()?.UserId;
                        vmInterview.VmInterviewerUserIds.InterviewerUserId = vmInterview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Interviewer).FirstOrDefault()?.UserId;
                        vmInterview.VmInterviewerUserIds.InterviewerLeadUserId = vmInterview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Lead).FirstOrDefault()?.UserId;
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

        #endregion

        #region Participants Modal

        [HttpGet]
        public async Task<PartialViewResult> ParticipantsModal(Guid id)
        {

            VmParticipantsModal result = new VmParticipantsModal();
            Entities.Interview interview = await _dal.GetEntity<Entities.Interview>((Guid)id, true) as Entities.Interview;

            result.InterviewId = id;
            result.CandidateUserId = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).FirstOrDefault()?.UserId;
            result.InterviewerUserIds = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Interviewer).ToList().Select(x => x.UserId).ToList();
            result.InterviewerLeadUserIds = interview.InterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Lead).ToList().Select(x => x.UserId).ToList();

            await SetParticipantsModalViewBag();

            return PartialView(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ParticipantsModal(VmParticipantsModal vmParticipantsModal)
        {

            //Entities.Interview interview = await _dal.GetEntity<Entities.Interview>(vmParticipantsModal.InterviewId, true) as Entities.Interview;

            // Handle Users
            List<InterviewUser> dbInterviewUsers = await _dal.GetInterviewUsersByInterviewId((Guid)vmParticipantsModal.InterviewId);
            await ResolveCandidateUser(vmParticipantsModal.CandidateUserId, dbInterviewUsers, RoleUserTypes.Candidate, (Guid)vmParticipantsModal.InterviewId);
            await ResolveInterviewUsers(vmParticipantsModal.InterviewerUserIds, dbInterviewUsers, RoleUserTypes.Interviewer, (Guid)vmParticipantsModal.InterviewId);
            await ResolveInterviewUsers(vmParticipantsModal.InterviewerLeadUserIds, dbInterviewUsers, RoleUserTypes.Lead, (Guid)vmParticipantsModal.InterviewId);

            // Email Candidate (given the UX of InterviewModal, the Candiate is only added when updating, so send email here).
            if (vmParticipantsModal.CandidateUserId != null)
            {
                InterviewUser dbCandidateInterviewUser = dbInterviewUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).FirstOrDefault();

                // Check to see if Candidate user has changed
                if (dbCandidateInterviewUser == null || (dbCandidateInterviewUser != null && dbCandidateInterviewUser.UserId != vmParticipantsModal.CandidateUserId))
                {                  
                    VmInterview vmInterview = _mapper.Map<VmInterview>(interview);
                    vmInterview.VmInterviewerUserIds.CandidateUserId = dbCandidateInterviewUser.UserId;
                    await SendInterviewEmailToCandiate(vmInterview);
                }
            }

            //interview.Status = GetInterviewState(vmInterviewModal.VmInterviewerUserIds);

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

        private async Task ResolveInterviewUsers(List<Guid> postedUserIds, List<InterviewUser> dbInterviewUsers, RoleUserTypes roleUserType, Guid interviewId)
        {

            List<InterviewUser> filteredDbUsers = dbInterviewUsers.Where(x => x.RoleUserType == roleUserType).ToList();
            InterviewUser interviewUser;

            foreach (Guid postedUserId in postedUserIds)
            {
                interviewUser = filteredDbUsers.Where(x => x.UserId == postedUserId).FirstOrDefault();
                if (interviewUser == null)
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
            }

            foreach (InterviewUser dbInterviewUser in filteredDbUsers)
            {
                if (!postedUserIds.Contains(dbInterviewUser.UserId))
                    // Delete
                    await _dal.DeleteEntity(dbInterviewUser);
            }

        }

        private async Task ResolveCandidateUser(Guid? postedUserId, List<InterviewUser> dbInterviewUsers, RoleUserTypes roleUserType, Guid interviewId)
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

        //private InterviewStates GetInterviewState(VmInterviewerUserIds vmInterviewUserIds)
        //{

        //    InterviewStates? result = InterviewStates.PendingCommitteeMembers;

        //    if (vmInterviewUserIds.InterviewerUserId != null && vmInterviewUserIds.InterviewerLeadUserId != null)
        //    {
        //        if (vmInterviewUserIds.CandidateUserId == null)
        //            result = InterviewStates.AvailableForCandidate;
        //        else
        //            result = InterviewStates.Booked;
        //    }

        //    return (InterviewStates)result;

        //}

        private async Task SendInterviewEmailToCandiate(VmInterview vmInterview)
        {

            Process process = await _dal.GetEntity<Process>((Guid)_state.ProcessId) as Process;
            EmailTemplate emailTemplate = await GetEmailTemplateForInterviewEmailToCandiate();
            List<Schedule> schedules = await _dal.GetSchedulesByProcessId((Guid)_state.ProcessId);
            RoleUser roleUser = await _dal.GetEntity<RoleUser>((Guid)vmInterview.VmInterviewerUserIds.CandidateUserId) as RoleUser;
            ExternalUser externalUser = await _dal.GetEntity<ExternalUser>((Guid)roleUser.UserId) as ExternalUser;
            TokenResponse tokenResponse = await _tokenManager.GetToken();
            string email;
            string callbackUrl;

            if ((bool)roleUser.IsExternal)
            {
                email = roleUser.ExternalUserEmail;
                callbackUrl = GetCallbackUrl(process.Id, externalUser.Id);
            }
            else
            {
                GraphUser graphUser = await _usersManager.GetUserInfoAsync(roleUser.UserId.ToString(), tokenResponse.access_token);
                email = graphUser.mail;
                callbackUrl = GetCallbackUrl(process.Id, null);
            }

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
                List<EmailRecipent> toRecipients = _emailsManager.GetEmailRecipients(email);

                string body = emailTemplate.EmailBody
                    .Replace("{0}", noProcess)
                    .Replace("{1}", groupNiv)
                    .Replace("{2}", startDate)
                    .Replace("{3}", startTime)
                    .Replace("{4}", startOral)
                    .Replace("{5}", location)
                    .Replace("{6}", contactName)
                    .Replace("{7}", contactNumber)
                    .Replace("{callbackUrl}", callbackUrl);

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