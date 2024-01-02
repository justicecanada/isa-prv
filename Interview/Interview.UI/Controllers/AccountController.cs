using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Interview.UI.Services.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Text.Json;

namespace Interview.UI.Controllers
{

	[AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
	public class AccountController : BaseController
    {

		#region Declarations

		private readonly IDownstreamWebApi _downstreamWebApi;

		#endregion

		#region Constructors

		public AccountController(IModelAccessor modelAccessor, IOptions<JusticeOptions> justiceOptions, IOptions<SessionTimeout> sessionTimeoutOptions, DalSql dal,
			IDownstreamWebApi downstreamWebApi) 
            : base(modelAccessor, justiceOptions, sessionTimeoutOptions, dal)
        {
			_downstreamWebApi = downstreamWebApi;
		}

        #endregion

        #region SignIn

        [HttpGet]
        public async Task<IActionResult> SignedIn()
        {

			using var response = await _downstreamWebApi.CallWebApiForUserAsync("DownstreamApi").ConfigureAwait(false);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var apiResult = await response.Content.ReadFromJsonAsync<JsonDocument>().ConfigureAwait(false);
				ViewData["ApiResult"] = JsonSerializer.Serialize(apiResult, new JsonSerializerOptions { WriteIndented = true });
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {error}");
			}

			return View();

        }

        #endregion

        #region SignOut

        [HttpGet]
        public RedirectToActionResult SignOut()
        {

            RedirectToActionResult result = null;

            // Handle where to redirect
            if (User.Identity.IsAuthenticated)
                result = new RedirectToActionResult("SignIn", "Account", null);
            else
                result = new RedirectToActionResult("SignedOut", "Account", null);

            // Handle Session
            HttpContext.Session.Clear();

            return result;

        }

        [HttpGet]
        public IActionResult SignedOut()
        {

            return View();

        }

        #endregion

    }

}
