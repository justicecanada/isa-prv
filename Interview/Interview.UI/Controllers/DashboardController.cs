using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Dashboard;
using Interview.UI.Models.JqueryDataTables;
using Interview.UI.Models.Stats;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Stats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
            //await SetIndexResultsViewBag(result);
            RegisterIndexClientResources();

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(VmFilter vmFilter)
        {

            await SetIndexFilterViewBag(vmFilter.ProcessId);
            //await SetIndexResultsViewBag(vmFilter);
            RegisterIndexClientResources();

            return View("Index", vmFilter);

        }

        private async Task SetIndexFilterViewBag(Guid? processId)
        {

            List<Entities.Process> processesForDropdown = await GetProcessesForLoggedInUser();
            List<Entities.Process> processesForStats = null;

            ViewBag.ProcessesForDropdown = processesForDropdown;
            ViewBag.ProcessId = processId;

        }

        private void RegisterIndexClientResources()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-DataTables/datatables.min.css'>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-DataTables/datatables.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Dashboard/Index.js?v={BuildId}'></script>");

        }

        #endregion

        #region Results Table Methods

        [HttpPost]
        public async Task<JsonResult> GetResults([FromBody]DtParameters dtParameters)
        {

            DtResult<VmDashboardItem> result = null;            
            VmFilter vmFilter = JsonConvert.DeserializeObject<VmFilter>(dtParameters.formfilter);
            List<VmDashboardItem> dashboardItems = await GetDashboardItems(vmFilter);

            result = new DtResult<VmDashboardItem>
            {
                Draw = dtParameters.draw,
                RecordsTotal = dashboardItems.Count,        // This needs to be the full non paged result set.
                RecordsFiltered = dashboardItems.Count,     // This needs to be the amount of filtered records.
                Data = dashboardItems
                    .Skip(dtParameters.start)
                    .Take(dtParameters.length)
                    .ToList()
            };

            return Json(result);

        }

        private async Task<List<VmDashboardItem>> GetDashboardItems(VmFilter vmFilter)
        {

            List<VmDashboardItem> result = null;
            List<Entities.Process> processResults = null;

            if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.System.ToString()))
                processResults = await _dal.GetAllProcessesForDashboard(vmFilter.ProcessId, vmFilter.StartDate, vmFilter.EndDate);
            else if (User.IsInRole(RoleTypes.Owner.ToString()))
                processResults = await _dal.GetProcessesForGroupOwnerForDashboard(EntraId, vmFilter.ProcessId, vmFilter.StartDate, vmFilter.EndDate);
            else
                processResults = await _dal.GetProcessesForRoleUserForDashboard(EntraId, vmFilter.ProcessId, vmFilter.StartDate, vmFilter.EndDate);
            processResults.OrderByDescending(x => x.CreatedDate);

            //// Equities
            List<Equity> equities = await _dal.GetAllEquities();
            //ViewBag.Equities = equities;

            //// Stats
            string cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
            //VmInterviewCounts vmInterviewCounts = _statsManager.GetInterviewCounts(processResults);
            //ViewBag.InterviewCounts = vmInterviewCounts;

            // Dashboard Items
            result = _statsManager.GetDashboardItems(processResults, equities, cultureName,
                vmFilter.PeriodOfTimeType == null ? VmPeriodOfTimeTypes.Daily : (VmPeriodOfTimeTypes)vmFilter.PeriodOfTimeType);

            return result;

        }

        #endregion

    }

}
