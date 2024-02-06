using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using GoC.WebTemplate.Components.Entities;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
//using Interview.UI.Models.Groups;
using Interview.UI.Models.Roles;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Graph;
using Interview.UI.Services.Mock.Identity;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace Interview.UI.Controllers
{
    
    public class RolesController : BaseController
    {

        #region Declarations

        private readonly IMapper _mapper;
        private readonly IState _state;
        private readonly IStringLocalizer<RolesController> _localizer;
        private readonly IToken _tokenManager;
        private readonly IUsers _usersManager;

        #endregion

        #region Constructors

        public RolesController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IState state, IStringLocalizer<RolesController> localizer, 
            IOptions<JusticeOptions> justiceOptions, IStringLocalizer<BaseController> baseLocalizer, IToken tokenManager, IUsers graphManager) 
            : base(modelAccessor, justiceOptions, dal, baseLocalizer)
        {
            
            _mapper = mapper;
            _state = state;
            _localizer = localizer;
            _tokenManager = tokenManager;
            _usersManager = graphManager;

        }

        #endregion

        #region Index Methods

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            VmIndex result = new VmIndex();

            // // Handle equities (this will be handled by the role the logged in user is in)
            if (User.IsInRole(RoleTypes.Admin.ToString()))
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

            if (ModelState.IsValid) // Handles simple validation (just posted model)
                await IndexHandleModelState(vmIndex); // Handles Complex validation (checks db)

            if (ModelState.IsValid)
            {

                // Handle RolesUser
                Guid roleUserId;
                RoleUser roleUser = await GetRoleUser(vmIndex);
                roleUserId = await _dal.AddEntity<RoleUser>(roleUser);

                // Handle equities (this will be handled by the role the logged in user is in)
                if (User.IsInRole(RoleTypes.Admin.ToString()))
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

        private async Task IndexHandleModelState(VmIndex vmIndex)
        {

            List<RoleUser> roleUsers = null;
            switch (vmIndex.UserType)
            {
                case UserTypes.Internal:                        // Ensure RoleUser hasn't been added to process
                    roleUsers = await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
                    if (roleUsers.Any(x => x.UserId == (Guid)vmIndex.InternalId))
                        ModelState.AddModelError("InternalName", _localizer["UserAlreadyInRole"].Value);
                    break;
                case UserTypes.ExistingExternal:                // Ensure RoleUser hasn't been added to process
                    roleUsers = await _dal.GetRoleUsersByProcessId((Guid)_state.ProcessId);
                    if (roleUsers.Any(x => x.UserId == (Guid)vmIndex.ExistingExternalId))
                        ModelState.AddModelError("ExistingExternalId", _localizer["UserAlreadyInRole"].Value);
                    break;

                case UserTypes.NewExternal:                     // Ensure new External User doesn't exist
                    roleUsers = await _dal.GetExternalRoleUsersByEmail(vmIndex.NewExternalEmail);
                    if (roleUsers.Any())
                        ModelState.AddModelError("NewExternalEmail", _localizer["ExternalUserAlreadyExists"].Value);
                    break;
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
            ViewBag.ShowEquities = User.IsInRole(RoleTypes.Admin.ToString());

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

        private async Task<RoleUser> GetRoleUser(VmIndex vmIndex)
        {

            // finish this...

            RoleUser result = new RoleUser()
            {
                ProcessId = (Guid)_state.ProcessId,
                LanguageType = vmIndex.LanguageType == null ? null : (LanguageTypes)vmIndex.LanguageType,
                RoleUserType = (RoleUserTypes)vmIndex.RoleUserType,
                IsExternal = (UserTypes)vmIndex.UserType != UserTypes.Internal,
                DateInserted = DateTime.Now
            };
            Guid? id = null;
            string userFirstName = null;
            string userLastName = null;

            if (vmIndex.UserType == UserTypes.Internal)
            {
                EntraUser entraUser = await GetEntraUser((Guid)vmIndex.InternalId);
                id = entraUser.id;
                userFirstName = entraUser.givenName;
                userLastName = entraUser.surname;
            }
            else if (vmIndex.UserType == UserTypes.NewExternal)
            {

            }
            else if (vmIndex.UserType == UserTypes.ExistingExternal)
            {

            }

            result.UserId = (Guid)id;
            result.UserFirstname = userFirstName;
            result.UserLastname = userLastName;

            return result;

        }

        private async Task<EntraUser> GetEntraUser(Guid id)
        {

            EntraUser result = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();

            result = await _usersManager.GetUserInfoAsync(id.ToString(), tokenResponse.access_token);

            return result;

        }

        #endregion

        #region LegendPartial Methods

        [HttpGet]
        public async Task<IActionResult> EmailAlreadySent()
        {

            return null;

        }

        [HttpGet]
        public async Task<IActionResult> EmailAllExternalCandidates()
        {

            return null;

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
            dbRoleUser.RoleUserType = (RoleUserTypes)vmRoleUser.RoleUserType;
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

        [HttpGet]
        public async Task<IActionResult> EmailExternalUser(Guid roleUserId)
        {

            return null;

        }

        #endregion

    }

}
