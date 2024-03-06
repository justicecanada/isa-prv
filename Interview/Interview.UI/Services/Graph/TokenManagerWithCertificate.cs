﻿using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
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
            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
              { "client_id", _tokenOptions.Value.ClientId },
              //{ "client_secret", _config["microsoft-provider-authentication-secret"] },
              { "grant_type", _grantType },
              { "scope", _scope },
            });
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_host}{_tokenOptions.Value.Uri}"))
            {
                Content = content
            };
            request.Headers.Add("X-ARR-ClientCert", _certificate.GetRawCertDataString());

            //LogCredentials();

            _client.BaseAddress = new Uri(_host);
            HttpResponseMessage response = _client.SendAsync(request).Result;

            result = await response.Content.ReadFromJsonAsync<TokenResponse>();

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
