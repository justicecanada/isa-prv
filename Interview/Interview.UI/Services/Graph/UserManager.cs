using Interview.UI.Models.Graph;
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
            //var justiceFilter = "(user.accountEnabled -eq true) and (user.dirSyncEnabled -eq true) and (user.userType -eq \"member\") and (user.userPrincipalName -match \"^[^./]+\\.[^./]+@(?>justice\\.gc\\.ca|osi-bis\\.ca|lcc-cdc\\.gc\\.ca|interlocuteur-special-interlocutor\\.ca|ombudsman\\.gc\\.ca)$\") and (user.mail -notContains \"/\")";
            var justiceFilter = "userPrincipalName -match \"^[^./]+\\.[^./]+@(?>justice\\.gc\\.ca|osi-bis\\.ca|lcc-cdc\\.gc\\.ca|interlocuteur-special-interlocutor\\.ca|ombudsman\\.gc\\.ca)$";

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_host}/v1.0/users?$filter=startswith(givenName, '{query}') or startswith(surname, '{query}')&$top=10 & {justiceFilter}"))
            {
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" }
                }
            };
            HttpResponseMessage response = _client.SendAsync(request).Result;

            if (response.StatusCode == HttpStatusCode.OK)
                result = await response.Content.ReadFromJsonAsync<SearchUsersResponse>();
            else
                badRequest = await response.Content.ReadFromJsonAsync<object>();

            return result;

        }

        public async Task<SearchUsersResponse> GetDisabledAccounts(string token)
        {

            //https://learn.microsoft.com/en-us/graph/api/user-list?view=graph-rest-1.0&tabs=http#example-4-use-filter-and-top-to-get-one-user-with-a-display-name-that-starts-with-a-including-a-count-of-returned-objects

            SearchUsersResponse result = null;
            object badRequest = null;
            string filter = "$filter=";

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_host}/v1.0/users?{filter}"))
            {
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" }
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
