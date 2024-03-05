using Interview.UI.Models.Graph;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Interview.UI.Services.Graph
{
    
    public class UserManager : IUsers
    {

        #region Declarations

        private readonly HttpClient _client;
        private readonly string _host = "https://graph.microsoft.com";

        #endregion

        #region Constructors

        public UserManager(HttpClient client)
        {

            _client = client;
            _client.BaseAddress = new Uri(_host);

        }

        #endregion

        #region Public Methods

        public async Task<GraphUser> GetUserInfoAsync(string userPrincipalName, string token)
        {

            GraphUser result = null;
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_host}/v1.0/users/{userPrincipalName}"))
            {
                Headers =
                {
                    //{ "Content-Type", "application/json" },
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" }
                }
            };
            HttpResponseMessage response = _client.SendAsync(request).Result;

            result = await response.Content.ReadFromJsonAsync<GraphUser>();

            return result;

        }

        public async Task<SearchUsersResponse> SearchInternalUsersAsync(string query, string token)
        {

            // https://learn.microsoft.com/en-us/graph/api/user-list?view=graph-rest-1.0&tabs=http#example-4-use-filter-and-top-to-get-one-user-with-a-display-name-that-starts-with-a-including-a-count-of-returned-objects

            SearchUsersResponse result = null;
            object badRequest = null;
            string baseUrl = $"{_host}/v1.0/users?";
            string filterKey = "$filter=";
            string nameClause = $"startswith(givenName, '{query}') or startswith(surname, '{query}')";
            string enabledClause = "accountEnabled eq true";
            string memberClause = "userType eq 'member'";
            string emailClause = "endswith(userPrincipalName,'@justice.gc.ca') OR endswith(userPrincipalName,'.osi-bis.ca') OR " +
                "endswith(userPrincipalName,'.lcc-cdc.gc.ca') OR endswith(userPrincipalName,'.interlocuteur-special-interlocutor.ca') OR endswith(userPrincipalName,'.ombudsman.gc.ca')";
            string filterSuffix = "&$top=10&$count=true";

            string fullFilter = $"{baseUrl}{filterKey}({nameClause}) and ({enabledClause}) and ({memberClause}) and ({emailClause}){filterSuffix}";

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(fullFilter))
            {
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" },
                    { "ConsistencyLevel", "eventual" }
                }
            };
            HttpResponseMessage response = _client.SendAsync(request).Result;

            if (response.StatusCode == HttpStatusCode.OK)
                result = await response.Content.ReadFromJsonAsync<SearchUsersResponse>();
            else
                badRequest = await response.Content.ReadFromJsonAsync<object>();

            return result;

        }

        #endregion

    }

}
