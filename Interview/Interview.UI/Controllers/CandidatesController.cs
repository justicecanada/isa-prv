using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Services.DAL;
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

        #endregion

        #region Constructors

        public CandidatesController(IModelAccessor modelAccessor, DalSql dal, IStringLocalizer<BaseController> baseLocalizer, IMapper mapper, 
            IState state)
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
            _state = state;
        }

        #endregion

        #region Public Internal Methods

        [Authorize]
        public async Task<IActionResult> Internal(Guid processId)
        {

            await SetInternalViewBag(processId);

            return View();

        }

        private async Task SetInternalViewBag(Guid processId)
        {

            Process process = await _dal.GetEntity<Process>(processId) as Process;
            List<Interview.Entities.Interview> interviews = await _dal.GetInterViewsByProcessId(processId);
            List<VmInterview> vmInterviews = _mapper.Map<List<VmInterview>>(interviews);

            ViewBag.ProccessStartDate = process.StartDate;
            ViewBag.ProccessEndDate = process.EndDate;
            ViewBag.VmInterviews = vmInterviews;

        }

        #endregion

        #region External Methods

        [HttpGet]
        public async Task<IActionResult> External(Guid processId, Guid externalCandidateId)
        {

            _state.ProcessId = processId;
            await SetExternalViewBag(processId, externalCandidateId);

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> SelectExternalInterview(Guid interviewId, Guid externalCandidateId)
        {

            await SetExternalViewBag((Guid)_state.ProcessId, externalCandidateId);

            return View("External");

        }

        private async Task SetExternalViewBag(Guid processId, Guid externalCandidateId)
        {

            Process process = await _dal.GetEntity<Process>(processId, true) as Process;
            List<Interview.Entities.Interview> interviews = await _dal.GetInterViewsByProcessId(processId);
            List<Entities.Interview> availableInterviews = interviews.Where(x => x.Status == InterviewStates.AvailableForCandidate).ToList();
            List<VmInterview> vmAvailableInterviews = _mapper.Map<List<VmInterview>>(availableInterviews);
            RoleUser candidateRoleUser = process.RoleUsers.Where(x => x.RoleUserType == RoleUserTypes.Candidate && x.UserId == externalCandidateId).First();
            var equities = await _dal.GetRoleUserEquitiesByRoleUserId(candidateRoleUser.Id);
            
            ViewBag.ProccessStartDate = process.StartDate;
            ViewBag.ProccessEndDate = process.EndDate;
            ViewBag.VmInterviews = vmAvailableInterviews;
            ViewBag.ExternalCandidateId = externalCandidateId;

        }

        #endregion

    }

}
