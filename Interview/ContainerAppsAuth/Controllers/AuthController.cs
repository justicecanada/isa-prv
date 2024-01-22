using ContainerAppsAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

namespace ContainerAppsAuth.Controllers
{
    public class AuthController : Controller
    {

        #region Declarations

        // https://johnnyreilly.com/azure-container-apps-easy-auth-and-dotnet-authentication

        private readonly ILogger<AuthController> _logger;
        private readonly string _enviornmentName;

        #endregion

        #region Constructors

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
        }

        #endregion

        #region Public Index

        public async Task<IActionResult> Index()
        {

            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.UserIdentity = JsonConvert.SerializeObject(User.Identity, Formatting.Indented);
            //ViewBag.SerializedHeaders = JsonConvert.SerializeObject(Request.Headers.Where(x => x.Key.ToUpper().StartsWith("X-MS-CLIENT-PRINCIPAL")), Formatting.Indented);
            ViewBag.SerializedHeaders = JsonConvert.SerializeObject(Request.Headers, Formatting.Indented);

            return View();

        }

        #endregion

        #region Public No Authentication

        public IActionResult NoAuthentication()
        {

            return View();

        }

        #endregion

        #region Require Authentication

        [Authorize]
        public IActionResult RequireAuthentication()
        {

            return View();

        }

        #endregion

    }

}