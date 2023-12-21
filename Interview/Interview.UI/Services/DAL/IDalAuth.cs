using Interview.UI.Models.Auth;

namespace Interview.UI.Services.DAL
{
    
    public interface IDalAuth
    {

        Task<GetAuthToken> GetAuthToken();

    }

}
