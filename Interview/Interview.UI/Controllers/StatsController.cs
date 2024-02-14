using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Stats;
using Interview.UI.Services.DAL;
using Interview.UI.Services.State;
using Interview.UI.Services.Stats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    
    public class StatsController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IStats _statsManager;

        #endregion

        #region Constructors

        public StatsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IStringLocalizer<BaseController> baseLocalizer, 
            IState state, IStats statsManager) 
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
            _state = state;
            _statsManager = statsManager;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index(string processIdToFilter = null)
        {

            VmIndex result = new VmIndex();
            Guid? processId = processIdToFilter == null ? null : new Guid(processIdToFilter);

            result.ProcessId = processId;
            await SetIndexViewBag(processId);

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchProcess(string processId)
        {

            return RedirectToAction("Index", "Stats", new { processIdToFilter = processId } ) ;

        }

        private async Task SetIndexViewBag(Guid? processId)
        {

            // Processes
            List<Entities.Process> processesForDropdown = await GetProcessesForLoggedInUser();
            List<Entities.Process> processesForStats = null;

            ViewBag.ProcessesForDropdown = processesForDropdown;
            ViewBag.ProcessId = processId;

            if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.System.ToString()))
                processesForStats = await _dal.GetAllProcessesForStats(processId);
            else if (User.IsInRole(RoleTypes.Owner.ToString()))
                processesForStats = await _dal.GetProcessesForGroupOwnerForStats(EntraId, processId);
            else
                processesForStats = await _dal.GetProcessesForRoleUserForStats(EntraId, processId);
            processesForStats.OrderByDescending(x => x.CreatedDate);

            // Equities
            List<Equity> equities = await _dal.GetAllEquities();
            ViewBag.Equities = equities;

            // Stats
            string cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
            VmInterviewStats vmInterviewStats = _statsManager.GetInterviewStats(processesForStats);
            ViewBag.InterviewStats = vmInterviewStats;
            List<VmEquityStat> candidateEquityStats = _statsManager.GetCandiateEquityStats(processesForStats, equities, cultureName);
            ViewBag.CandidateEquityStats = candidateEquityStats;

        }

        #endregion

    }

}
