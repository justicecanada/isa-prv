using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Dashboard;
using Interview.UI.Models.Stats;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Stats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    
    public class DashboardController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IStats _statsManager;

        #endregion

        #region Constructors

        public DashboardController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IStringLocalizer<BaseController> baseLocalizer, 
            IStats statsManager) 
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
            _statsManager = statsManager;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index(string processIdToFilter = null)
        {

            VmFilter result = new VmFilter();
            Guid? processId = processIdToFilter == null ? null : new Guid(processIdToFilter);

            result.ProcessId = processId;
            await SetIndexFilterViewBag(processId);
            await SetIndexResultsViewBag(result);

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(VmFilter vmFilter)
        {

            await SetIndexFilterViewBag(vmFilter.ProcessId);
            await SetIndexResultsViewBag(vmFilter);

            return View("Index", vmFilter);

        }

        private async Task SetIndexFilterViewBag(Guid? processId)
        {

            List<Entities.Process> processesForDropdown = await GetProcessesForLoggedInUser();
            List<Entities.Process> processesForStats = null;

            ViewBag.ProcessesForDropdown = processesForDropdown;
            ViewBag.ProcessId = processId;

        }

        private async Task SetIndexResultsViewBag(VmFilter vmFilter)
        {

            List<Entities.Process> processResults = null;

            if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.System.ToString()))
                processResults = await _dal.GetAllProcessesForStats(vmFilter.ProcessId);
            else if (User.IsInRole(RoleTypes.Owner.ToString()))
                processResults = await _dal.GetProcessesForGroupOwnerForStats(EntraId, vmFilter.ProcessId);
            else
                processResults = await _dal.GetProcessesForRoleUserForStats(EntraId, vmFilter.ProcessId);
            processResults.OrderByDescending(x => x.CreatedDate);

            // Equities
            List<Equity> equities = await _dal.GetAllEquities();
            ViewBag.Equities = equities;

            // Stats
            string cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
            VmInterviewStats vmInterviewStats = _statsManager.GetInterviewStats(processResults);
            ViewBag.InterviewStats = vmInterviewStats;

        }

        #endregion

    }

}
