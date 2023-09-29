using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Services.DAL;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
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

        #endregion

        #region Constructors

        public DefaultController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state) : base(modelAccessor)
        {
            _dal = dal;
            _mapper = mapper;
            _state = state;
        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<Contest> contests = await _dal.GetAllContestsWithUserSettingsAndRoles();
            List<VmContest> vmContests = (List<VmContest>)(contests == null ? new List<VmContest>() : _mapper.Map(contests, typeof(List<Contest>), typeof(List<VmContest>)));
            Guid? contestId = null;

            // Look to Session for ContestId
            if (_state.ContestId != null)
                contestId = _state.ContestId;
            // Look to first item in list if _state.ContestId isn't set by user
            else if (vmContests.Any())
            {
                contestId = vmContests.First().Id;
                _state.ContestId = contestId;
            }

            ViewBag.VmContests = vmContests;
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
