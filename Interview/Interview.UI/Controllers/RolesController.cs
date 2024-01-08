using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.Components.Entities;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
//using Interview.UI.Models.Groups;
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

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IStringLocalizer<RolesController> _localizer;

        #endregion

        #region Constructors

        public RolesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IStringLocalizer<RolesController> localizer, 
            IOptions<JusticeOptions> justiceOptions) : base(modelAccessor, justiceOptions, dal)
        {
            
            _mapper = mapper;
            _state = state;
            _localizer = localizer;

            //IndexRegisterClientResources();

        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();

            // // Handle equities (this will be handled by the role the logged in user is in)
            if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin))
            {
                var equities = await _dal.GetAllEquities();
                List<VmEquity> vmEquities = (List<VmEquity>)_mapper.Map(equities, typeof(List<Equity>), typeof(List<VmEquity>));
                result.Equities = vmEquities;
            }

            await IndexSetViewBag();
            IndexRegisterClientResources();

            if (TempData["RoleUserIdToUpdate"] != null)
            {
                RoleUser roleUserToEdit = ((List<RoleUser>)ViewBag.RoleUsers).Where(x => x.Id == (Guid)TempData["RoleUserIdToUpdate"]).First();
                result.RoleUserToEdit = (VmRoleUser)_mapper.Map(roleUserToEdit, typeof(RoleUser), typeof(VmRoleUser));
            }

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VmIndex vmIndex)
        {

            var mockUser = await GetMockUser(vmIndex);
            
            if (ModelState.IsValid)
            {

                // Handle RolesUser
                Guid roleUserId;
                RoleUser roleUser = new RoleUser()
                {
                    ProcessId = (Guid)_state.ProcessId,
                    LanguageType = vmIndex.LanguageType == null ? null : (LanguageTypes)vmIndex.LanguageType,
                    RoleType = (RoleTypes)vmIndex.RoleType,
                    UserId = (Guid)mockUser.Id,
                    UserFirstname = mockUser.FirstName,
                    UserLastname = mockUser.LastName,
                    IsExternal = (UserTypes)vmIndex.UserType != UserTypes.Internal,
                    DateInserted = DateTime.Now
                };
                roleUserId = await _dal.AddEntity<RoleUser>(roleUser);

                // Handle equities (this will be handled by the role the logged in user is in)
                if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin))
                {                   
                    foreach (var equity in vmIndex.Equities.Where(x => x.IsSelected).ToList())
                    {
                        var roleUserEquity = new RoleUserEquity()
                        {
                            RoleUserId = roleUserId,
                            EquityId = (Guid)equity.Id
                        };
                        await _dal.AddEntity<RoleUserEquity>(roleUserEquity);
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

        private async Task IndexSetViewBag()
        {

            // Process
            var process = _state.ProcessId == null ? new Process() : await _dal.GetEntity<Process>((Guid)_state.ProcessId, true);
            ViewBag.Process = process;

            // RoleUsers
            var roleUsers = _state.ProcessId == null ? new List<RoleUser>() : await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
            ViewBag.RoleUsers = roleUsers;

            // Show Equities
            ViewBag.ShowEquities = IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin);

            // MockUsers
            var mockExistingExternalUsers = await _dal.GetListExistingExternalMockUser();
            ViewBag.MockExistingExternalUsers = mockExistingExternalUsers;

        }

        private void IndexRegisterClientResources()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css'>");
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-DataTables/datatables.min.css'>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Roles/Index.js?v={BuildId}'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-DataTables/datatables.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/js/Roles/TablePartial.js?v={BuildId}'></script>");

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
        public IActionResult UpdateRoleUser(Guid roleUserId)
        {

            TempData["RoleUserIdToUpdate"] = roleUserId;

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoleUser(VmRoleUser vmRoleUser)
        {

            var dbRoleUser = await _dal.GetEntity<RoleUser>((Guid)vmRoleUser.Id) as RoleUser;

            // Handle RoleUser
            dbRoleUser.RoleType = (RoleTypes)vmRoleUser.RoleType;
            dbRoleUser.LanguageType = vmRoleUser.LanguageType == null ? null : (LanguageTypes)vmRoleUser.LanguageType;
            await _dal.UpdateEntity(dbRoleUser);

            // Handle Equities
            if (IsLoggedInMockUserInRole(MockLoggedInUserRoles.Admin))
            {
                var dbRoleUserEquities = await _dal.GetRoleUserEquitiesByRoleUserId((Guid)vmRoleUser.Id);
                var postedEquities = vmRoleUser.Equities.Where(x => x.IsSelected).ToList();

                // Delete Equities
                foreach (RoleUserEquity dbRoleUserEquity in dbRoleUserEquities)
                    if (!postedEquities.Any(x => x.Id == dbRoleUserEquity.EquityId))
                        await _dal.DeleteEntity<RoleUserEquity>(dbRoleUserEquity.Id);

                // Add Equities
                foreach (VmEquity vmEquity in postedEquities)
                {
                    if (!dbRoleUserEquities.Any(x => x.EquityId == vmEquity.Id))
                    {
                        RoleUserEquity roleUserEquity = new RoleUserEquity()
                        {
                            EquityId = (Guid)vmEquity.Id,
                            RoleUserId = (Guid)vmRoleUser.Id
                        };
                        await _dal.AddEntity<RoleUserEquity>(roleUserEquity);
                    }             
                }

            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> DeleteRoleUser(Guid roleUserId)
        {

            await _dal.DeleteEntity<RoleUser>(roleUserId);

            return RedirectToAction("Index");

        }

        #endregion

    }

}
