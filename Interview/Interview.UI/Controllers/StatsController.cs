using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Stats;
using Interview.UI.Services.DAL;
using Interview.UI.Services.State;
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

        #endregion

        #region Constructors

        public StatsController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IStringLocalizer<BaseController> baseLocalizer, 
            IState state) 
            : base(modelAccessor, dal, baseLocalizer)
        {
            _mapper = mapper;
            _state = state;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index(string processIdToFilter = null)
        {

            VmIndex result = new VmIndex();
            Guid processId = processIdToFilter == null ? (Guid)_state.ProcessId : new Guid(processIdToFilter);

            result.ProcessId = processId;
            await SetIndexViewBag();

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SwitchProcess(string processId)
        {

            return RedirectToAction("Index", "Stats", new { processIdToFilter = processId } ) ;

        }

        private async Task SetIndexViewBag()
        {

            List<Entities.Process> processes = await GetProcessesForLoggedInUser();
            ViewBag.Processes = processes;

            ViewBag.ProcessId = (Guid)_state.ProcessId;

        }

        #endregion

    }

}
