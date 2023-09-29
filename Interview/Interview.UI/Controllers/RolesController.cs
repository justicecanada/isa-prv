﻿using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.Components.Entities;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Roles;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Mock.Identity;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<JusticeOptions> _justiceOptions;

        #endregion

        #region Constructors

        public RolesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IStringLocalizer<RolesController> localizer, 
            IOptions<JusticeOptions> justiceOptions) : base(modelAccessor)
        {
            
            _dal = dal;
            _mapper = mapper;
            _state = state;
            _localizer = localizer;
            _justiceOptions = justiceOptions;

            //IndexRegisterClientResources();

        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();

            // // Handle equities (this will be handled by the role the logged in user is in)
            if (_justiceOptions.Value.ShowEquitiesOnRoles)
            {
                var equities = await _dal.GetAllEquities();
                List<VmEquity> vmEquities = (List<VmEquity>)_mapper.Map(equities, typeof(List<Equity>), typeof(List<VmEquity>));
                result.Equities = vmEquities;
            }

            await IndexSetViewBag();
            IndexRegisterClientResources();

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VmIndex vmIndex)
        {

            var mockUser = await GetMockUser(vmIndex);
            
            if (ModelState.IsValid)
            {

                // Handle UserSetting
                Guid userSettingId;
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
                userSettingId = await _dal.AddEntity(userSetting);

                // Handle equities (this will be handled by the role the logged in user is in)
                if (_justiceOptions.Value.ShowEquitiesOnRoles)
                {                   
                    foreach (var equity in vmIndex.Equities.Where(x => x.IsSelected).ToList())
                    {
                        var userSettingEquity = new UserSettingEquity()
                        {
                            UserSettingId = userSettingId,
                            EquityId = (Guid)equity.Id
                        };
                        await _dal.AddEntity(userSettingEquity);
                    }
                }

                return RedirectToAction("Index");

            }
            else
            {

                await IndexSetViewBag();
                IndexRegisterClientResources();

                return View(vmIndex);

            }

        }

        [HttpGet]
        public async Task<JsonResult> LookupInteralUser(string query)
        {

            List<MockUser> result = null;

            if (!string.IsNullOrEmpty(query))
                result = await _dal.LookupInteralMockUser(query);

            return new JsonResult(new { result = true, results = result })
            {
                StatusCode = 200
            };

        }

        private async Task IndexSetViewBag()
        {

            // Contest
            var contest = _state.ContestId == null ? new Contest() : await _dal.GetEntity<Contest>((Guid)_state.ContestId, true);
            var vmContest = _mapper.Map<VmContest>(contest);
            ViewBag.VmContest = vmContest;

            // UserSettings
            // TODO: Get UserSettings and include Equities
            var userSettings = _state.ContestId == null ? new List<UserSetting>() : await _dal.GetUserSettingsByContestId((Guid)_state.ContestId);
            var vmUserSettings = _mapper.Map(userSettings, typeof(List<UserSetting>), typeof(List<VmUserSetting>));
            ViewBag.VmUserSettings = vmUserSettings;

            var roles = await _dal.GetAllRoles();
            var vmRoles = _mapper.Map(roles, typeof(List<Role>), typeof(List<VmRole>));
            ViewBag.VmRoles = vmRoles;

            // UserLanguages
            var userLanguages = await _dal.GetAllUserLanguages();
            var vmUserLanguages = _mapper.Map(userLanguages, typeof(List<UserLanguage>), typeof(List<VmUserLanguage>));
            ViewBag.VmUserLanguages = vmUserLanguages;

            // Show Equities
            ViewBag.ShowEquities = _justiceOptions.Value.ShowEquitiesOnRoles;

            // MockUsers
            var mockExistingExternalUsers = await _dal.GetListExistingExternalMockUser();
            ViewBag.MockExistingExternalUsers = mockExistingExternalUsers;

        }

        private void IndexRegisterClientResources()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css'>");
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-datatables/datatables.min.css'>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/roles/index.js?v={AssemblyVersion}'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-datatables/datatables.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/roles/tablepartial.js?v={AssemblyVersion}'></script>");

        }

        private async Task<MockUser> GetMockUser(VmIndex vmIndex)
        {

            MockUser result = null;

            switch (vmIndex.UserType)
            {

                case UserTypes.Internal:

                    result = await _dal.GetMockUserByIdAndType((Guid)vmIndex.InternalId, UserTypes.Internal);
                    if (result == null)
                        ModelState.AddModelError("InternalName", _localizer["InternalUserDoesNotExist"]);

                    break;

                case UserTypes.ExistingExternal:

                    result = await _dal.GetMockUserByIdAndType((Guid)vmIndex.ExistingExternalId, UserTypes.ExistingExternal);

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

                    await _dal.AddMockUser(result);

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
