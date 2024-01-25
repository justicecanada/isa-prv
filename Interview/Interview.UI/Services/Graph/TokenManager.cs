using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Interview.UI.Services.Graph
{

    public class TokenManager : IToken
    {

        #region Declarations

        private readonly HttpClient _client;
        private readonly string _host = "https://login.microsoftonline.com";
        private readonly string _uri;
        private readonly string _clientId;
        private readonly string _grantType = "client_credentials";
        private readonly string _scope = "https://graph.microsoft.com/.default";
        private readonly string _clientSecret;
        private readonly ILogger<TokenManager> _logger;

        #endregion

        #region Constructors

        public TokenManager(HttpClient client, IConfiguration config, IOptions<TokenOptions> tokenOptions, ILogger<TokenManager> logger)
        {

            _client = client;
            _client.BaseAddress = new Uri(_host);

            _uri = tokenOptions.Value.Uri;
            _clientId = tokenOptions.Value.ClientId;
            // User Secrets: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&source=recommendations&tabs=windows
            _clientSecret = config["app-registration-client-secret"];

            _logger = logger;

        }

        //public TokenManager()
        //{

        //    // https://stackoverflow.com/questions/40014047/add-client-certificate-to-net-core-httpclient

        //    HttpClientHandler handler = new HttpClientHandler();
        //    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        //    handler.SslProtocols = SslProtocols.Tls12;
        //    handler.ClientCertificates.Add(new X509Certificate2("certificate.crt"));

        //    _client = new HttpClient(handler);
        //    _client.BaseAddress = new Uri(_host);

        //}

        #endregion

        #region Public Methods

        public async Task<TokenResponse> GetToken()
        {

            TokenResponse result = await RefreshToken();

            return result;

        }

        #endregion

        #region Private Caching Token Methods



        #endregion

        #region Private Get Token Methods

        private async Task<TokenResponse> RefreshToken()
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

            //LogCredentials();

            HttpResponseMessage response = _client.SendAsync(request).Result;

            result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            return result;

        }

        private void LogCredentials()
        {

            var creds = new
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                grant_type = _grantType,
                scope = _scope
            };
            var credsJson = JsonConvert.SerializeObject(creds);

            _logger.LogInformation($"GetTokenWithBody creds = {credsJson}");

        }

        #endregion

    }

}
