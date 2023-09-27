using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.Roles;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Mock.Identity;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Interview.UI.Controllers
{
    
    public class RolesController : BaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly MockIdentityContext _mockIdentityContext;

        #endregion

        #region Constructors

        public RolesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, MockIdentityContext mockIdentityContext) : base(modelAccessor)
        {
            _dal = dal;
            _mapper = mapper;
            _state = state;
            _mockIdentityContext = mockIdentityContext;
        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();

            await SetIndexViewBag();
            
            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VmIndex vmIndex)
        {

            if (ModelState.IsValid)
            {
                return null;
            }
            else
            {

                await SetIndexViewBag();

                return View(vmIndex);

            }

        }

        private async Task SetIndexViewBag()
        {

            VmContest vmContest;            
            var roles = await _dal.GetAllRoles();
            var vmRoles = _mapper.Map(roles, typeof(List<Role>), typeof(List<VmRole>));
            var userLanguages = await _dal.GetAllUserLanguages();
            var vmUserLanguages = _mapper.Map(userLanguages, typeof(List<UserLanguage>), typeof(List<VmUserLanguage>));
            var mockExistingExternalUsers = await _mockIdentityContext.MockUsers.Where(x => x.UserType == UserTypes.ExistingExternal).ToListAsync();

            if (_state.ContestId == null)
                vmContest = new VmContest();
            else
            {
                var contest = await _dal.GetEntity<Contest>((Guid)_state.ContestId, true);
                vmContest = _mapper.Map<VmContest>(contest);
            }

            ViewBag.VmContest = vmContest;
            ViewBag.VmRoles = vmRoles;
            ViewBag.VmUserLanguages = vmUserLanguages;
            ViewBag.MockExistingExternalUsers = mockExistingExternalUsers;

        }

        #endregion

    }

}
