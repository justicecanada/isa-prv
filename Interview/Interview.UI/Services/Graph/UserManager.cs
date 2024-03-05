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

        #region Exploratory Filter Clauses

        public async Task<SearchUsersResponse> GetEnabledAccountsAsync(string token)
        {

            // (user.accountEnabled -eq true) 

            SearchUsersResponse result = null;
            string filterClause = "accountEnabled eq true";
            HttpResponseMessage response = await QueryGraph(filterClause, token);

            result = await GetSearchUserResponse(response, filterClause);

            return result;

        }

        public async Task<SearchUsersResponse> GetBadEmailsAsync(string token)
        {

            // (user.userPrincipalName -match \"^[^./]+\\.[^./]+@(?>justice\\.gc\\.ca|osi-bis\\.ca|lcc-cdc\\.gc\\.ca|interlocuteur-special-interlocutor\\.ca|ombudsman\\.gc\\.ca)$\")

            SearchUsersResponse result = null;
            object badRequest = null;
            string filterClause = "endswith(userPrincipalName,'@justice.gc.ca') OR endswith(userPrincipalName,'.osi-bis.ca') OR " + 
                "endswith(userPrincipalName,'.lcc-cdc.gc.ca') OR endswith(userPrincipalName,'.interlocuteur-special-interlocutor.ca') OR endswith(userPrincipalName,'.ombudsman.gc.ca')";
            HttpResponseMessage response = await QueryGraph(filterClause, token);

            result = await GetSearchUserResponse(response, filterClause);

            return result;

        }

        public async Task<SearchUsersResponse> GetDirSyncEnabledAsync(string token)
        {

            // and (user.dirSyncEnabled -eq true) 

            SearchUsersResponse result = null;
            object badRequest = null;
            string filterClause = "dirSyncEnabled eq true";
            HttpResponseMessage response = await QueryGraph(filterClause, token);

            result = await GetSearchUserResponse(response, filterClause);

            return result;

        }

        public async Task<SearchUsersResponse> GetMemberUserTypesAsync(string token)
        {

            // and (user.dirSyncEnabled -eq true) 

            SearchUsersResponse result = null;
            object badRequest = null;
            string filterClause = "userType eq 'member'";
            HttpResponseMessage response = await QueryGraph(filterClause, token);

            result = await GetSearchUserResponse(response, filterClause);

            return result;

        }

        public async Task<SearchUsersResponse> GetNoSlashesInEmailAsync(string token)
        {

            // and (user.mail -notContains \"/\")

            SearchUsersResponse result = null;
            object badRequest = null;
            string filterClause = "userType eq 'member'";
            HttpResponseMessage response = await QueryGraph(filterClause, token);

            result = await GetSearchUserResponse(response, filterClause);

            return result;

        }

        #endregion

        #region Private Methods

        private async Task<HttpResponseMessage> QueryGraph(string filterClause, string token)
        {

            HttpResponseMessage result = null;
            string graphUserUrl = $"{_host}/v1.0/users?";
            string filterPrefix = "$filter=";
            string filterSuffix = "&$top=10&$count=true";
            string filter = $"{filterPrefix} {filterClause} {filterSuffix}";
            Uri uri = new Uri($"{graphUserUrl}{filter}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri)
            {
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" },
                    { "ConsistencyLevel", "eventual" }
                }
            };

            result = _client.SendAsync(request).Result;

            return result;

        }

        private async Task<SearchUsersResponse> GetSearchUserResponse(HttpResponseMessage response, string filterClause)
        {

            SearchUsersResponse result = null;
 
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = await response.Content.ReadFromJsonAsync<SearchUsersResponse>();
            }
            else
            {
                result = new SearchUsersResponse();
                result.badRequest = response.Content.ReadFromJsonAsync<object>();
            }
            result.statusCode = response.StatusCode;
            result.filterClause = filterClause;

            return result;

        }

        #endregion

    }

}
