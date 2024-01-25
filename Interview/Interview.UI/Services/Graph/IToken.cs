using Interview.UI.Models.Graph;

namespace Interview.UI.Services.Graph
{
    
    public interface IToken
    {

        Task<TokenResponse> GetToken();

    }

}
