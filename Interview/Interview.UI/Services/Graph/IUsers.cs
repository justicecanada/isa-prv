using Interview.UI.Models.Graph;

namespace Interview.UI.Services.Graph
{
    
    public interface IUsers
    {

        Task<GraphUser> GetUserInfoAsync(string userPrincipalName, string token);

        Task<SearchUsersResponse> SearchInternalUsersAsync(string query, string token);

        Task<SearchUsersResponse> GetDisabledAccountsAsync(string token);

        Task<SearchUsersResponse> GetBadEmailsAsync(string token);

        Task<SearchUsersResponse> GetDirSyncEnabledAsync(string token);

        Task<SearchUsersResponse> GetMemberUserTypesAsync(string token);

        Task<SearchUsersResponse> GetNoSlashesInEmailAsync(string token);

    }

}
