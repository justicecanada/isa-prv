using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.Roles;
using Interview.UI.Services.DAL;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class RolesController : BaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;
        private readonly IState _state;

        #endregion

        #region Constructors

        public RolesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state) : base(modelAccessor)
        {
            _dal = dal;
            _mapper = mapper;
            _state = state;
        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();
            var contest = await _dal.GetEntity<Contest>((Guid)_state.ContestId, true);
            var vmContest = _mapper.Map<VmContest>(contest);

            ViewBag.VmContest = vmContest;
            
            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VmIndex vmIndex)
        {
            
            //if (ModelState.IsValid)
            //{
            //    return null;
            //}
            //else
            //{

                var contest = await _dal.GetEntity<Contest>((Guid)_state.ContestId, true);
                var vmContest = _mapper.Map<VmContest>(contest);

                ViewBag.VmContest = vmContest;

                return View(vmIndex);

            //}

        }

        #endregion

    }

}
