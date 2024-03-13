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
using System;
using System.Text.Json;

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
            HandleCommonPageMethods();

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(VmFilter vmFilter)
        {

            await SetIndexFilterViewBag(vmFilter.ProcessId);
            //await SetIndexResultsViewBag(vmFilter);
            RegisterIndexClientResources();
            HandleCommonPageMethods();

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

            if (System.Globalization.CultureInfo.CurrentCulture.Name == Constants.EnglishCulture)
                WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Resources.en.js?v={BuildId}'></script>");
            else
                WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Resources.fr.js?v={BuildId}'></script>");

        }

        #endregion

        #region Results Table Methods

        [HttpPost]
        public async Task<string> GetResults([FromBody]DtParameters dtParameters)
        {
           
            VmFilter vmFilter = JsonConvert.DeserializeObject<VmFilter>(dtParameters.formfilter);
            string cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
            List<Process> processes = await GetProcesses(vmFilter, cultureName);
            List<Equity> equities = await _dal.GetAllEquities();
            List<VmDashboardItem> dashboardItems = _statsManager.GetDashboardItems(processes, equities, cultureName,
                vmFilter.PeriodOfTimeType == null ? VmPeriodOfTimeTypes.Daily : (VmPeriodOfTimeTypes)vmFilter.PeriodOfTimeType);
            VmInterviewCounts vmInterviewCounts = _statsManager.GetInterviewCounts(processes);
            string seralizedInterviewCounts = System.Text.Json.JsonSerializer.Serialize(vmInterviewCounts);

            DtResult<VmDashboardItem> dtResult = new DtResult<VmDashboardItem>
            {
                Draw = dtParameters.draw,
                RecordsTotal = dashboardItems.Count,
                RecordsFiltered = dashboardItems.Count,
                Data = dashboardItems
                    .Skip(dtParameters.start)
                    .Take(dtParameters.length)
                    .ToList(),
                PartialView = seralizedInterviewCounts
            };
            JsonSerializerOptions settings = new JsonSerializerOptions() { PropertyNamingPolicy = new CustomLessThanDesirableNamingPolicy() };
            string result = System.Text.Json.JsonSerializer.Serialize(dtResult, settings);

            return result;

        }

        private async Task<List<Entities.Process>> GetProcesses(VmFilter vmFilter, string cultureName)
        {

            List<Entities.Process> result = null;
            DateTime startDate = vmFilter.StartDate == null ? DateTime.Now.AddYears(-1) : (DateTime)vmFilter.StartDate;
            DateTime endDate = vmFilter.EndDate == null ? DateTime.Now.AddYears(1) : (DateTime)vmFilter.EndDate;

            if (User.IsInRole(RoleTypes.Admin.ToString()) || User.IsInRole(RoleTypes.System.ToString()))
                result = await _dal.GetAllProcessesForDashboard(vmFilter.ProcessId, startDate, endDate);
            else if (User.IsInRole(RoleTypes.Owner.ToString()))
                result = await _dal.GetProcessesForGroupOwnerForDashboard(EntraId, vmFilter.ProcessId, startDate, endDate);
            else
                result = await _dal.GetProcessesForRoleUserForDashboard(EntraId, vmFilter.ProcessId, startDate, endDate);
            result.OrderByDescending(x => x.CreatedDate);

            return result;

        }

        #endregion

    }

    public class CustomLessThanDesirableNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {

            if (name == "Draw")
                return "draw";
            if (name == "RecordsTotal")
                return "recordsTotal";
            if (name == "RecordsFiltered")
                return "recordsFiltered";

            if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
                return name;

            return name.ToLower();
        }
    }

}
