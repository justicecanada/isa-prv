﻿using Interview.UI.Models.Graph;

namespace Interview.UI.Services.Graph
{
    
    public interface IUsers
    {

        Task<GraphUser> GetUserInfoAsync(string userPrincipalName, string token);

        Task<SearchUsersResponse> SearchInternalUsersAsync(string query, string token);

        Task<SearchUsersResponse> GetDisabledAccounts(string token);

        Task<SearchUsersResponse> GetBadEmails(string token);

        Task<SearchUsersResponse> GetDirSyncEnabled(string token);

    }

}
