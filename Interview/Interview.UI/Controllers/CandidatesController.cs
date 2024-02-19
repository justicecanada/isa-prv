using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Interview.UI.Controllers
{
    
    public class CandidatesController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public CandidatesController(IModelAccessor modelAccessor, DalSql dal, IStringLocalizer<BaseController> baseLocalizer, IMapper mapper)
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
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

        public async Task<IActionResult> External(Guid processId, Guid externalCandidateId)
        {

            await SetExternalViewBag(processId, externalCandidateId);

            return View();

        }

        private async Task SetExternalViewBag(Guid processId, Guid externalCandidateId)
        {

            Process process = await _dal.GetEntity<Process>(processId) as Process;
            List<Interview.Entities.Interview> interviews = await _dal.GetInterViewsByProcessId(processId);
            List<VmInterview> vmInterviews = _mapper.Map<List<VmInterview>>(interviews);

            ViewBag.ProccessStartDate = process.StartDate;
            ViewBag.ProccessEndDate = process.EndDate;
            ViewBag.VmInterviews = vmInterviews;
            ViewBag.ExternalCandidateId = externalCandidateId;

        }

        #endregion

    }

}
