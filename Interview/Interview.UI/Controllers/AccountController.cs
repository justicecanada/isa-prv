﻿using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Interview.UI.Services.DAL;
using Interview.UI.Services.Graph;
using Interview.UI.Services.State;
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

        #endregion

        #region Constructors

        public AccountController(IModelAccessor modelAccessor, DalSql dal, IOptions<JusticeOptions> justiceOptions,
            IToken tokenManager, IUsers graphManager, IStringLocalizer<BaseController> baseLocalizer, IWebHostEnvironment hostEnvironment)
            : base(modelAccessor, justiceOptions, dal, baseLocalizer)
        {

            _tokenManager = tokenManager;
            _usersManager = graphManager;
            _hostEnvironment = hostEnvironment;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        }

        #endregion

        #region Public Login Methods

        [HttpGet]
        public IActionResult Login()
        {

            IActionResult result = null;

            if (_hostEnvironment.IsDevelopment())
                result = new RedirectToActionResult("Index", "Default", null);
            else
                result = new RedirectResult("/.auth/login/aad?post_login_redirect_uri=/Default/Index");

            return result;

        }

        #endregion

        #region Public Details Methods

        [HttpGet]
        public async Task<IActionResult> Details()
        {

            // Need to add Authorization Bearer (token) request header:
            // https://learn.microsoft.com/en-us/graph/api/user-get?view=graph-rest-1.0&tabs=http#example-2-signed-in-user-request
            // Links regarding Container Apps Easy Auth and tokens:
            //   1. https://johnnyreilly.com/azure-container-apps-easy-auth-and-dotnet-authentication
            //   2. https://github.com/microsoft/azure-container-apps/issues/995#issuecomment-1820496130
            //   3. https://github.com/microsoft/azure-container-apps/issues/479#issuecomment-1817523559

            // Get Token
            TokenResponse tokenResponse = await _tokenManager.GetToken();
            string userPrincipalName = User.Identity.Name;
            EntraUser entraUser = await _usersManager.GetUserInfoAsync(userPrincipalName, tokenResponse.access_token);

            ViewBag.EntraUser = entraUser;

            return View();

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

    }

}
