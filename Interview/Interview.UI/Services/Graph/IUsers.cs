using Interview.UI.Models.Graph;

namespace Interview.UI.Services.Graph
{
    
    public interface IUsers
    {

        Task<EntraUser> GetUserInfo(string userPrincipalName, string token);

        Task<SearchUsersResponse> SearchInternalUsers(string query, string token);

    }

}
