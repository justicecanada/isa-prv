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
            VmContest vmContest = vmContests.FirstOrDefault();

            _state.ContestId = vmContest == null ? null : vmContest.Id;

            ViewBag.VmContests = vmContests;

            return View();

        }

        #endregion

    }
}
