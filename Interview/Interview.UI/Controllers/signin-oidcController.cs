using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Text.Json;

namespace Interview.UI.Controllers
{

    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    public class signin_oidcController : BaseController
    {

        #region Declarations

        private readonly IDownstreamWebApi _downstreamWebApi;

        #endregion

        #region Constructors

        public signin_oidcController(IModelAccessor modelAccessor, IOptions<JusticeOptions> justiceOptions, IOptions<SessionTimeout> sessionTimeoutOptions, DalSql dal,
            IDownstreamWebApi downstreamWebApi)
            : base(modelAccessor, justiceOptions, sessionTimeoutOptions, dal)
        {
            _downstreamWebApi = downstreamWebApi;
        }

        #endregion

        public async Task<IActionResult> Index()
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

	}
}
