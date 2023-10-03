using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Interview.UI.Controllers
{
    public class DefaultController : BaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IOptions<JusticeOptions> _justiceOptions;

        #endregion

        #region Constructors

        public DefaultController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IOptions<JusticeOptions> justiceOptions) : base(modelAccessor)
        {
            _dal = dal;
            _mapper = mapper;
            _state = state;
            _justiceOptions = justiceOptions;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<Contest> contests = await _dal.GetAllContestsWithUserSettingsAndRoles();
            Guid? contestId = null;

            // Look to Session for ContestId
            if (_state.ContestId != null)
                contestId = _state.ContestId;
            // Look to first item in list if _state.ContestId isn't set by user
            else if (contests.Any())
            {
                contestId = contests.First().Id;
                _state.ContestId = contestId;
            }

            ViewBag.Contests = contests;
            ViewBag.ContestId = contestId;

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> SwitchContest(Guid contestId)
        {

            _state.ContestId = contestId;

            return RedirectToAction("Index");

        }

        #endregion

    }
}
