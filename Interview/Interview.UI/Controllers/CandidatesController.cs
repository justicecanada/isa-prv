using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.Graph;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Graph;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using Process = Interview.Entities.Process;

namespace Interview.UI.Controllers
{
    
    public class CandidatesController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IToken _tokenManager;
        private readonly IUsers _usersManager;
        private readonly IEmails _emailsManager;

        #endregion

        #region Constructors

        public CandidatesController(IModelAccessor modelAccessor, DalSql dal, IStringLocalizer<BaseController> baseLocalizer, IMapper mapper, 
            IState state, IToken tokenManager, IUsers userManager, IEmails emailsManager)
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
            _state = state;
            _tokenManager = tokenManager;
            _usersManager = userManager;
            _emailsManager = emailsManager;
        }

        #endregion

        #region Interviews Methods

        [HttpGet]
        public async Task<IActionResult> Interviews(Guid processId, Guid externalCandidateId)
        {

            _state.ProcessId = processId;
            await SetInterviewsViewBag(processId, externalCandidateId);

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> SelectInterview(Guid interviewId, Guid externalCandidateId)
        {

            Entities.Interview interview = await _dal.GetEntity<Entities.Interview>(interviewId) as Entities.Interview;
            Process process = await _dal.GetEntity<Process>(interview.ProcessId, true) as Process;
            RoleUser candidateRoleUser = process.RoleUsers.Where(x => (x.RoleUserType == RoleUserTypes.Candidate && x.UserId == externalCandidateId)).First();
            InterviewUser newInterviewUser = new InterviewUser()
            {
                UserId = candidateRoleUser.Id,
                RoleUserType = RoleUserTypes.Candidate,
                InterviewId = interviewId
            };

            // Handle Interview
            interview.Status = InterviewStates.Booked;
            await _dal.UpdateEntity(interview);

            // Handle Interview User
            await _dal.AddEntity<InterviewUser>(newInterviewUser);

            // Email Candidate
            await HandleCandidateEmail(process, interview, candidateRoleUser);

            return RedirectToAction("InterviewBooked", "Candidates", new { id = interviewId });

        }

        private async Task SetInterviewsViewBag(Guid processId, Guid externalCandidateId)
        {

            Process process = await _dal.GetEntity<Process>(processId, true) as Process;
            List<Interview.Entities.Interview> interviews = await _dal.GetInterViewsByProcessId(processId);
            List<Entities.Interview> availableInterviews = interviews.Where(x => x.Status == InterviewStates.AvailableForCandidate).ToList();
            List<VmInterview> vmAvailableInterviews = _mapper.Map<List<VmInterview>>(availableInterviews);
            RoleUser candidateRoleUser = process.RoleUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate && x.UserId == externalCandidateId).First();
            List<RoleUserEquity> roleUserEquities = await _dal.GetRoleUserEquitiesByRoleUserId(candidateRoleUser.Id);
            
            ViewBag.ProccessStartDate = process.StartDate;
            ViewBag.ProccessEndDate = process.EndDate;
            ViewBag.VmInterviews = vmAvailableInterviews;
            ViewBag.ExternalCandidateId = externalCandidateId;
            ViewBag.RoleUserEquities = roleUserEquities;

        }

        private async Task HandleCandidateEmail(Process process, Entities.Interview interview, RoleUser candidateRoleUser)
        {

            List<EmailTemplate> emailTemplates = await _dal.GetEmailTemplatesByProcessId((Guid)_state.ProcessId);
            EmailTemplate emailTemplate = null;
            EmailEnvelope emailEnvelope = null;
            VmInterview vmInterview = _mapper.Map<VmInterview>(interview);
            TokenResponse tokenResponse = await _tokenManager.GetToken();
            HttpResponseMessage responseMessage = null;

            if (User.IsInRole(RoleTypes.Admin.ToString()))
            {
                emailTemplate = emailTemplates.Where(x => x.EmailType == EmailTypes.CandidateAddedByHR).FirstOrDefault();
                emailEnvelope = _emailsManager.GetEmailEnvelopeForCandidateAddedByHR(emailTemplate, process, vmInterview, candidateRoleUser.ExternalUserEmail);
            }
            else
            {
                emailTemplate = emailTemplates.Where(x => x.EmailType == EmailTypes.CandidateRegisteredTimeSlot).FirstOrDefault();
                emailEnvelope = _emailsManager.GetEmailEnvelopeForCandidateRegisteredTimeSlot(emailTemplate, process, vmInterview, candidateRoleUser.ExternalUserEmail);
            }
            responseMessage = await _emailsManager.SendEmailAsync(emailEnvelope, tokenResponse.access_token, User.Identity.Name);
 
        }

        #endregion

        #region Interview Booked

        [HttpGet]
        public async Task<IActionResult> InterviewBooked(Guid id)
        {

            Entities.Interview interview = await _dal.GetEntity<Entities.Interview>(id, true) as Entities.Interview;
            VmInterview vmInterview = _mapper.Map<VmInterview>(interview);
            Process process = await _dal.GetEntity<Process>(interview.ProcessId, true) as Process;
            RoleUser candidateRoleUser = process.RoleUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate).First();
            List<RoleUserEquity> roleUserEquities = await _dal.GetRoleUserEquitiesByRoleUserId(candidateRoleUser.Id);

            ViewBag.VmInterview = vmInterview;
            ViewBag.RoleUserEquities = roleUserEquities;

            return View();

        }

        #endregion

        #region Common Methodes

        private async Task<GraphUser> GetGraphUser()
        {

            // Need to add Authorization Bearer (token) request header:
            // https://learn.microsoft.com/en-us/graph/api/user-get?view=graph-rest-1.0&tabs=http#example-2-signed-in-user-request
            // Links regarding Container Apps Easy Auth and tokens:
            //   1. https://johnnyreilly.com/azure-container-apps-easy-auth-and-dotnet-authentication
            //   2. https://github.com/microsoft/azure-container-apps/issues/995#issuecomment-1820496130
            //   3. https://github.com/microsoft/azure-container-apps/issues/479#issuecomment-1817523559

            // Get Token
            GraphUser result = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();
            string userPrincipalName = User.Identity.Name;

            result = await _usersManager.GetUserInfoAsync(userPrincipalName, tokenResponse.access_token);

            return result;

        }

        #endregion

    }

}
