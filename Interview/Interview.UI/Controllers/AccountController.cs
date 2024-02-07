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

    public class AccountController : BaseController
    {

        #region Declarations

        private readonly IToken _tokenManager;
        private readonly IUsers _usersManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public AccountController(IModelAccessor modelAccessor, DalSql dal, IOptions<JusticeOptions> justiceOptions,
            IToken tokenManager, IUsers userManager, IStringLocalizer<BaseController> baseLocalizer, IWebHostEnvironment hostEnvironment,
            IMapper mapper)
            : base(modelAccessor, justiceOptions, dal, baseLocalizer)
        {

            _tokenManager = tokenManager;
            _usersManager = userManager;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        }

        #endregion

        #region Public Login Methods

        [HttpGet]
        public async Task<IActionResult> Login()
        {

            IActionResult result = null;

            if (_hostEnvironment.IsDevelopment())
            {
                result = new RedirectToActionResult("Index", "Default", null);
            }
            else
                result = new RedirectResult("/.auth/login/aad?post_login_redirect_uri=/Default/Index");

            return result;

        }

        #endregion

        #region Public Details Methods

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details()
        {

            VmInternalUser result = null;
            EntraUser entraUser = await GetEntraUser();
            InternalUser internalUser = await _dal.GetInternalUserByEntraId(EntraId);

            ViewBag.EntraUser = entraUser;
            result = internalUser == null ? new VmInternalUser() : _mapper.Map<VmInternalUser>(internalUser);

            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(VmInternalUser vmInternalUser)
        {

            if (ModelState.IsValid)
            {

                InternalUser internaluser = _mapper.Map<InternalUser>(vmInternalUser);

                internaluser.EntraId = EntraId;
                if (vmInternalUser.Id == null)
                    await _dal.AddEntity<InternalUser>(internaluser);
                else
                    await _dal.UpdateEntity(internaluser);

                return RedirectToAction("Details");

            }
            else
            {
                EntraUser entraUser = await GetEntraUser();

                ViewBag.EntraUser = entraUser;

                return View("Details", vmInternalUser);
            }

        }

        #endregion

        #region Public Search Users Methods

        [HttpGet]
        public async Task<IActionResult> SearchUsers()
        {

            // css
            WebTemplateModel.HTMLHeaderElements.Add($"<link rel='stylesheet' href='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.css'>");

            // js
            WebTemplateModel.HTMLBodyElements.Add($"<script src='/lib/jquery-ui-1.13.2.custom/jquery-ui.min.js'></script>");
            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Account/SearchUsers.js?v={BuildId} \"></script>");

            return View();

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
        public async Task<JsonResult> GetUserDetails(string userPrincipalName)
        {

            string result = null;
            EntraUser entraUser = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();

            entraUser = await _usersManager.GetUserInfoAsync(userPrincipalName, tokenResponse.access_token);
            result = JsonConvert.SerializeObject(entraUser, Formatting.Indented);

            return new JsonResult(new { result = true, results = result })
            {
                StatusCode = 200
            };

        }

        #endregion

        #region Private Common Methods

        private async Task<EntraUser> GetEntraUser()
        {

            // Need to add Authorization Bearer (token) request header:
            // https://learn.microsoft.com/en-us/graph/api/user-get?view=graph-rest-1.0&tabs=http#example-2-signed-in-user-request
            // Links regarding Container Apps Easy Auth and tokens:
            //   1. https://johnnyreilly.com/azure-container-apps-easy-auth-and-dotnet-authentication
            //   2. https://github.com/microsoft/azure-container-apps/issues/995#issuecomment-1820496130
            //   3. https://github.com/microsoft/azure-container-apps/issues/479#issuecomment-1817523559

            // Get Token
            EntraUser result = null;
            TokenResponse tokenResponse = await _tokenManager.GetToken();
            string userPrincipalName = User.Identity.Name;

            result = await _usersManager.GetUserInfoAsync(userPrincipalName, tokenResponse.access_token);
            ViewBag.EntraUser = result;

            return result;

        }

        #endregion

    }

}
