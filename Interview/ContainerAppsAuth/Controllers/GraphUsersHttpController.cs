using ContainerAppsAuth.Models;
using ContainerAppsAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Security.Claims;

namespace ContainerAppsAuth.Controllers
{
    
    public class GraphUsersHttpController : Controller
    {

        #region Declarations

        private readonly TokenManager _tokenManager;
        private readonly GraphManager _graphManager;

        #endregion

        #region Constructors

        public GraphUsersHttpController(TokenManager tokenManager, GraphManager graphManager)
        {

            _tokenManager = tokenManager;
            _graphManager = graphManager;

        }

        #endregion

        #region Public Index

        [Authorize]
        public async Task<IActionResult> Index()
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
            // ToDo: Get token for use to call with Graph

            // Get User
            //string userId = ((ClaimsIdentity)User.Identity).Claims.Where(x => x.Type == "aud").First().Value;
            //string userId = ((ClaimsIdentity)User.Identity).Claims.Where(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").First().Value;
            string userPrincipalName = User.Identity.Name;
            EntraUser entraUser = await _graphManager.GetUserInfo(userPrincipalName, tokenResponse.access_token);
            ViewBag.EntraUser = JsonConvert.SerializeObject(entraUser, Formatting.Indented);

            return View();

        }

        #endregion

        #region Search Users

        public async Task<IActionResult> SearchUsers()
        {

            return View();

        }

        [HttpGet]
        public async Task<JsonResult> SearchInteralUsers(string query)
        {

            SearchUsersResponse result = null;
            TokenResponse tokenResponse = await _tokenManager.GetTokenWithBody();

            result = await _graphManager.SearchInternalUsers(query, tokenResponse.access_token);

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
            TokenResponse tokenResponse = await _tokenManager.GetTokenWithBody();

            entraUser = await _graphManager.GetUserInfo(userPrincipalName, tokenResponse.access_token);
            result = JsonConvert.SerializeObject(entraUser, Formatting.Indented);

            return new JsonResult(new { result = true, results = result })
            {
                StatusCode = 200
            };

        }

        #endregion

    }

}
