using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
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

        #endregion

        #region Constructors

        public AccountController(IModelAccessor modelAccessor, DalSql dal, IOptions<JusticeOptions> justiceOptions,
            IToken tokenManager)
            : base(modelAccessor, justiceOptions, dal)
        {

            _tokenManager = tokenManager;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        }

        #endregion

        #region Public Index Methods

        public IActionResult Index()
        {

            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.UserIdentity = JsonConvert.SerializeObject(User.Identity, Formatting.Indented);
            ViewBag.SerializedHeaders = JsonConvert.SerializeObject(Request.Headers.Where(x => x.Key.ToUpper().StartsWith("X-MS-CLIENT-PRINCIPAL")), Formatting.Indented);

            return View();

        }

        #endregion

    }
    
}
