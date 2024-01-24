using AutoMapper;
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
        private readonly IGraph _graphManager;

        #endregion

        #region Constructors

        public AccountController(IModelAccessor modelAccessor, DalSql dal, IOptions<JusticeOptions> justiceOptions,
            IToken tokenManager, IGraph graphManager, IStringLocalizer<BaseController> baseLocalizer)
            : base(modelAccessor, justiceOptions, dal, baseLocalizer)
        {

            _tokenManager = tokenManager;
            _graphManager = graphManager;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        }

        #endregion

        #region Public Index Methods

        [HttpGet]
        public IActionResult Index()
        {

            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.UserIdentity = JsonConvert.SerializeObject(User.Identity, Formatting.Indented);
            ViewBag.SerializedHeaders = JsonConvert.SerializeObject(Request.Headers.Where(x => x.Key.ToUpper().StartsWith("X-MS-CLIENT-PRINCIPAL")), Formatting.Indented);

            return View();

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
            TokenResponse tokenResponse = await _tokenManager.GetTokenWithBody();
            ViewBag.TokenResponse = JsonConvert.SerializeObject(tokenResponse, Formatting.Indented);

            // Get User
            string userPrincipalName = User.Identity.Name;
            EntraUser entraUser = await _graphManager.GetUserInfo(userPrincipalName, tokenResponse.access_token);
            ViewBag.EntraUser = JsonConvert.SerializeObject(entraUser, Formatting.Indented);

            return View();

        }

        #endregion

        #region Public Search Users Methods

        public async Task<IActionResult> SearchUsers()
        {

            WebTemplateModel.HTMLBodyElements.Add($"<script src=\"/js/Sccount/SearchUsers.js?v={BuildId} \"></script>");

            return View();

        }

        #endregion

    }

}
