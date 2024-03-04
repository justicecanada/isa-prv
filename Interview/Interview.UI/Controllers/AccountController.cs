using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Graph;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Interview.UI.Controllers
{

    [Authorize(Roles = Roles.Admin)]
    public class AccountController : BaseController
    {

        #region Declarations

        private readonly IToken _tokenManager;
        private readonly IUsers _usersManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AccountController> _localizer;

        #endregion

        #region Constructors

        public AccountController(IModelAccessor modelAccessor, DalSql dal, IToken tokenManager, IUsers userManager, 
            IStringLocalizer<BaseController> baseLocalizer, IWebHostEnvironment hostEnvironment,
            IMapper mapper, IStringLocalizer<AccountController> localizer)
            : base(modelAccessor, dal, baseLocalizer)
        {

            _tokenManager = tokenManager;
            _usersManager = userManager;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _localizer = localizer;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        }

        #endregion

        #region Public Login Methods

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {

            IActionResult result = null;

            if (_hostEnvironment.IsDevelopment())
                result = new RedirectToActionResult("Index", "Default", null);
            else
                result = new RedirectResult("/.auth/login/aad?post_login_redirect_uri=/Default/Index");

            return result;

        }

        #endregion

        #region Public Manage User Roles Methods

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {

            RegisterManageUserRolesClientResources();
            HandleCommonPageMethods();

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(VmInternalUser vmInternalUser)
        {

            TokenResponse tokenResponse = await _tokenManager.GetToken();
            GraphUser graphUser = await _usersManager.GetUserInfoAsync(vmInternalUser.EntraId.ToString(), tokenResponse.access_token);

            if (ModelState.IsValid)
            {

                InternalUser internaluser = _mapper.Map<InternalUser>(vmInternalUser);

                if (vmInternalUser.Id == Guid.Empty)
                    await _dal.AddEntity<InternalUser>(internaluser);
                else
                    await _dal.UpdateEntity(internaluser);

                RegisterManageUserRolesClientResources();
                HandleCommonPageMethods();
                Notify(string.Format(_localizer["UserAddedToRole"].Value, graphUser.givenName, graphUser.surname, vmInternalUser.RoleType), "success");

                return View();

            }
            else
            {

                ViewBag.GraphUser = graphUser;

                RegisterManageUserRolesClientResources();
                HandleCommonPageMethods();

                return View(vmInternalUser);
            }

        }

        [HttpGet]
        public async Task<JsonResult> SearchInteralUsers(string query)
        {

            SearchUsersResponse result = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();

            result = await _usersManager.SearchInternalUsersAsync(query, tokenResponse.access_token);

            return new JsonResult(new { result = true, results = result.value })
            {
                StatusCode = 200
            };

        }
 
        [HttpGet]
        public async Task<PartialViewResult> UserDetailsPartial(string userPrincipalName)
        {

            VmInternalUser result = null;
            InternalUser internalUser = null;
            GraphUser graphUser = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();

            graphUser = await _usersManager.GetUserInfoAsync(userPrincipalName, tokenResponse.access_token);
            ViewBag.GraphUser = graphUser;
            internalUser = await _dal.GetInternalUserByEntraId(graphUser.id);

            if (internalUser == null)
                internalUser = new InternalUser() { EntraId = graphUser.id };

            result = _mapper.Map<VmInternalUser>(internalUser);

            return PartialView(result);

        }

        private void RegisterManageUserRolesClientResources()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css'>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Account/SearchUsers.js?v={BuildId} \"></script>");

        }

        #endregion

        #region Raise Exception Methods

        [HttpGet]
        public IActionResult RaiseException()
        {

            throw new Exception("Wups!");

        }

        #endregion

        #region Private Common Methods

        private async Task<GraphUser> GetGraphUser()
        {

            // Need to add Authorization Bearer (token) request header:
            // https://learn.microsoft.com/en-us/graph/api/user-get?view=graph-rest-1.0&tabs=http#example-2-signed-in-user-request
            // Links regarding Container Apps Easy Auth and tokens:
            //   1. https://johnnyreilly.com/azure-container-apps-easy-auth-and-dotnet-authentication
            //   2. https://github.com/microsoft/azure-container-apps/issues/995#issuecomment-1820496130
            //   3. https://github.com/microsoft/azure-container-apps/issues/479#issuecomment-1817523559

            // Get Token
            GraphUser result = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();
            string userPrincipalName = User.Identity.Name;

            result = await _usersManager.GetUserInfoAsync(userPrincipalName, tokenResponse.access_token);

            return result;

        }

        #endregion

    }

}
