using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Interview.UI.Services.Graph
{
    
    public class TokenManagerWithCertificate : IToken
    {

        #region Declarations

        private readonly HttpClient _client;
        private readonly IOptions<TokenOptions> _tokenOptions;
        private readonly IConfiguration _config;
        private readonly ILogger<TokenManagerWithSecret> _logger;
        private readonly IMemoryCache _cache;
        private readonly X509Certificate2 _certificate;

        private const string _host = "https://login.microsoftonline.com";
        private const string _grantType = "client_credentials";
        private const string _scope = "https://graph.microsoft.com/.default";
        private const string _cacheKey = "TOKEN_CACHE_KEY";

        #endregion

        #region Constructors

        public TokenManagerWithCertificate(IConfiguration config, IOptions<TokenOptions> tokenOptions, ILogger<TokenManagerWithSecret> logger,
            IMemoryCache cache)
        {

            _config = config;
            _tokenOptions = tokenOptions;
            _logger = logger;
            _cache = cache;

            // https://stackoverflow.com/questions/40014047/add-client-certificate-to-net-core-httpclient
            //HttpClientHandler handler = new HttpClientHandler();
            //handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //handler.SslProtocols = SslProtocols.Tls12;
            //handler.ClientCertificates.Add(new X509Certificate2("certificate.crt"));


            _certificate = new X509Certificate2("certificate.crt");

            _client = new HttpClient();

        }

        #endregion

        #region Public Methods

        public async Task<TokenResponse> GetToken()
        {

            TokenResponse result = GetFromCach();

            if (result == null)
            {
                result = await RefreshToken();
                AddToCache(result);
            }

            return result;

        }

        #endregion

        #region Private Caching Token Methods

        private TokenResponse GetFromCach()
        {

            TokenResponse result = _cache.Get<TokenResponse>(_cacheKey);

            return result;

        }

        private void AddToCache(TokenResponse tokenResponse)
        {

            // token.expires_in defaults to 1 hour
            DateTime expirationTime = DateTime.Now.Add(TimeSpan.FromSeconds(tokenResponse.expires_in)).AddMinutes(-1);
            CancellationChangeToken expirationToken = new CancellationChangeToken(
                new CancellationTokenSource(TimeSpan.FromSeconds(tokenResponse.expires_in)).Token);
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
                 .SetPriority(CacheItemPriority.Normal)
                 .SetAbsoluteExpiration(expirationTime)
                 .AddExpirationToken(expirationToken)
                 .RegisterPostEvictionCallback(EvictionCallback, this);

            _cache.Set(_cacheKey, tokenResponse, options);

        }

        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {

        }

        #endregion

        #region Private Get Token Methods

        private async Task<TokenResponse> RefreshToken()
        {

            // Get Token https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-client-creds-grant-flow#second-case-access-token-request-with-a-certificate
            // User Secrets: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&source=recommendations&tabs=windows

            TokenResponse result = null;
            var clientAssertion = GetSignedClientAssertion(_certificate, _tokenOptions.Value.ClientId);
            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
              { "client_id", _tokenOptions.Value.ClientId },

              // Used for Secret
              //{ "client_secret", _config["microsoft-provider-authentication-secret"] },

              // Used for Cert
              { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
              { "client_assertion", clientAssertion },

              { "grant_type", _grantType },
              { "scope", _scope },
            });
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_host}{_tokenOptions.Value.Uri}"))
            {
                Content = content
            };

            //LogCredentials();

            _client.BaseAddress = new Uri(_host);
            HttpResponseMessage response = _client.SendAsync(request).Result;

            result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            return result;

        }

        private string GetSignedClientAssertion(X509Certificate2 certificate, string clientId)
        {

            // https://learn.microsoft.com/en-us/entra/msal/dotnet/acquiring-tokens/web-apps-apis/confidential-client-assertions#crafting-the-assertion

            // no need to add exp, nbf as JsonWebTokenHandler will add them by default.
            string result = null;
            var claims = new Dictionary<string, object>()
            {
                { "aud", "https://login.microsoftonline.com/44c0b27b-bb8b-4284-829c-8ad96d3b40e5/oauth2/v2.0/token" },
                { "iss", clientId },
                { "jti", Guid.NewGuid().ToString() },
                { "sub", clientId }
            };

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claims,
                SigningCredentials = new X509SigningCredentials(certificate)
            };

            var handler = new JsonWebTokenHandler();
            try
            {
                result = handler.CreateToken(securityTokenDescriptor);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return result;

        }


        private void LogCredentials()
        {

            var creds = new
            {
                client_id = _tokenOptions.Value.ClientId,
                client_secret = _config["app-registration-client-secret"],
                grant_type = _grantType,
                scope = _scope
            };
            var credsJson = JsonConvert.SerializeObject(creds);

            _logger.LogInformation($"GetTokenWithBody creds = {credsJson}");

        }

        #endregion

    }

}
