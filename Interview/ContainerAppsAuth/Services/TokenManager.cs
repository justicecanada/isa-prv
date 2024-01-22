using ContainerAppsAuth.Models;
using Newtonsoft.Json;
using System.Net;

namespace ContainerAppsAuth.Services
{

    public class TokenManager
    {

        #region Declarations

        private readonly HttpClient _client;
        private readonly string _host = "https://login.microsoftonline.com";
        private readonly string _uri = "/44c0b27b-bb8b-4284-829c-8ad96d3b40e5/oauth2/v2.0/token";
        private readonly string _clientId = "44a90db8-4fca-4c3f-be35-3f228f145ab0";
        private readonly string _grantType = "client_credentials";
        private readonly string _scope = "https://graph.microsoft.com/.default";
        private readonly string _clientSecret;

        #endregion

        #region Constructors

        public TokenManager(HttpClient client, IConfiguration config)
        {
            
            _client = client;
            _client.BaseAddress = new Uri(_host);

            // User Secrets: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&source=recommendations&tabs=windows
            _clientSecret = config["microsoft-provider-authentication-secret"];

        }

        #endregion

        #region Public Methods

        public async Task<TokenResponse> GetTokenWithBody()
        {

            // https://learn.microsoft.com/en-us/graph/auth-v2-service?tabs=http#token-request

            TokenResponse result = null;
            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
              { "client_id", _clientId },
              { "client_secret", _clientSecret },
              { "grant_type", _grantType },
              { "scope", _scope },
            });
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_host}{_uri}"))
            {
                Content = content
            };
            HttpResponseMessage response = _client.SendAsync(request).Result;

            result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            return result;

        }

        #endregion

    }

}
