using ContainerAppsAuth.Models;
using Microsoft.Graph.Models;
using System;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ContainerAppsAuth.Services
{
    
    public class GraphManager
    {

        #region Declarations

        private readonly HttpClient _client;
        private readonly string _host = "https://graph.microsoft.com";

        #endregion

        #region Constructors

        public GraphManager(HttpClient client)
        {

            _client = client;
            _client.BaseAddress = new Uri(_host);

        }

        //public GraphManager()
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

        public async Task<EntraUser> GetUserInfo(string userPrincipalName, string token)
        {

            EntraUser result = null;
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_host}/v1.0/users/{userPrincipalName}"))
            {
                Headers =
                {
                    //{ "Content-Type", "application/json" },
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" }
                }
            };
            HttpResponseMessage response = _client.SendAsync(request).Result;

            result = await response.Content.ReadFromJsonAsync<EntraUser>();

            return result;

        }

        public async Task<SearchUsersResponse> SearchInternalUsers(string query, string token)
        {

            // https://learn.microsoft.com/en-us/graph/api/user-list?view=graph-rest-1.0&tabs=http#example-4-use-filter-and-top-to-get-one-user-with-a-display-name-that-starts-with-a-including-a-count-of-returned-objects

            SearchUsersResponse result = null;
            object badRequest = null;
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_host}/v1.0/users?$filter=startswith(displayname, '{query}')&$top=10"))
            {
                Headers =
                {
                    //{ "Content-Type", "application/json" },
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" }
                }
            };
            HttpResponseMessage response = _client.SendAsync(request).Result;

            if (response.StatusCode== HttpStatusCode.OK)
                result = await response.Content.ReadFromJsonAsync<SearchUsersResponse>();
            else
                badRequest = await response.Content.ReadFromJsonAsync<object>();

            return result;

        }

        #endregion

    }

}
