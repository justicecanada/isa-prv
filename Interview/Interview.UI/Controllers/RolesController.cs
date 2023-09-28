using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.Roles;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Mock.Identity;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;

namespace Interview.UI.Controllers
{
    
    public class RolesController : BaseController
    {

        #region Declarations

        private readonly DalSql _dal;
        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IStringLocalizer<RolesController> _localizer;

        private readonly MockIdentityContext _mockIdentityContext;

        #endregion

        #region Constructors

        public RolesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, MockIdentityContext mockIdentityContext,
            IStringLocalizer<RolesController> localizer) : base(modelAccessor)
        {
            _dal = dal;
            _mapper = mapper;
            _state = state;
            _localizer = localizer;

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

            var mockUser = await GetMockUser(vmIndex);
            
            if (ModelState.IsValid)
            {

                UserSetting userSetting = new UserSetting()
                {
                    ContestId = (Guid)_state.ContestId,
                    UserLanguageId = vmIndex.UserLanguageId,
                    RoleId = (Guid)vmIndex.RoleId,
                    UserId = (Guid)mockUser.Id,
                    UserFirstname = mockUser.FirstName,
                    UserLastname = mockUser.LastName,
                    IsExternal = (UserTypes)vmIndex.UserType != UserTypes.Internal,
                    DateInserted = DateTime.Now
                };

                await _dal.AddEntity(userSetting);

                return RedirectToAction("Index");

            }
            else
            {

                await SetIndexViewBag();

                return View(vmIndex);

            }

        }

        [HttpGet]
        public async Task<JsonResult> LookupInteralUser(string query)
        {

            List<MockUser> result = null;

            if (!string.IsNullOrEmpty(query))
                result = await _mockIdentityContext.MockUsers.Where(x => ((x.FirstName.ToLower().StartsWith(query.ToLower()) || x.LastName.ToLower().StartsWith(query.ToLower()))
                    && x.UserType == UserTypes.Internal)).ToListAsync();

            return new JsonResult(new { result = true, results = result })
            {
                StatusCode = 200
            };

        }

        private async Task SetIndexViewBag()
        {

            VmContest vmContest;            
            var roles = await _dal.GetAllRoles();
            var vmRoles = _mapper.Map(roles, typeof(List<Role>), typeof(List<VmRole>));
            var userLanguages = await _dal.GetAllUserLanguages();
            var vmUserLanguages = _mapper.Map(userLanguages, typeof(List<UserLanguage>), typeof(List<VmUserLanguage>));
            var userSettings = await _dal.GetUserSettingsByContestId((Guid)_state.ContestId);
            var vmUserSettings = _mapper.Map(userSettings, typeof(List<UserSetting>), typeof(List<VmUserSetting>));
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
            ViewBag.VmUserSettings = vmUserSettings;
            ViewBag.MockExistingExternalUsers = mockExistingExternalUsers;

        }

        private async Task<MockUser> GetMockUser(VmIndex vmIndex)
        {

            MockUser result = null;

            switch (vmIndex.UserType)
            {

                case UserTypes.Internal:

                    result = await _mockIdentityContext.MockUsers.Where(x => (x.Id == vmIndex.InternalId && 
                        x.UserType == UserTypes.Internal)).FirstOrDefaultAsync();
                    if (result == null)
                        ModelState.AddModelError("InternalName", _localizer["InternalUserDoesNotExist"]);

                    break;

                case UserTypes.ExistingExternal:

                    result = await _mockIdentityContext.MockUsers.Where(x => (x.Id == vmIndex.ExistingExternalId &&
                        x.UserType == UserTypes.ExistingExternal)).FirstOrDefaultAsync();

                    break;

                case UserTypes.NewExternal:

                    result = new MockUser()
                    {
                        UserName = vmIndex.NewExternalEmail,
                        FirstName = vmIndex.NewExternalFirstName,
                        LastName = vmIndex.NewExternalLastName,
                        Email = vmIndex.NewExternalEmail,
                        UserType = UserTypes.NewExternal
                    };

                    _mockIdentityContext.Entry(result).State = EntityState.Added;
                    await _mockIdentityContext.SaveChangesAsync();

                    break;

            }

            return result;

        }

        #endregion

        #region TablePartial Methods

        [HttpGet]
        public IActionResult UpdateUserSettings(Guid userSettingsId)
        {

            TempData["UserSettingIdToUpdate"] = userSettingsId;

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserSettings(VmUserSetting vmUserSetting)
        {

            var dbUserSetting = await _dal.GetEntity<UserSetting>((Guid)vmUserSetting.Id) as UserSetting;

            dbUserSetting.RoleId = vmUserSetting.RoleId;
            dbUserSetting.UserLanguageId = vmUserSetting.UserLanguageId;
            await _dal.UpdateEntity(dbUserSetting);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> DeleteUserSetting(Guid userSettingsId)
        {

            await _dal.DeleteEntity<UserSetting>(userSettingsId);

            return RedirectToAction("Index");

        }

        #endregion

    }

}
