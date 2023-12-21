using Interview.UI.Models.Auth;
using System.Net;
using System.Net.Http.Headers;

namespace Interview.UI.Services.DAL
{
    
    public class DalAuth : IDalAuth
    {

        private readonly string _baseAddress;
        private readonly HttpClient _client;

        public DalAuth(IHttpContextAccessor contextAccessor, HttpClient client)
        {

            _baseAddress = $"{contextAccessor.HttpContext.Request.Scheme}://{contextAccessor.HttpContext.Request.Host.Value}";

            _client = client;
            _client.BaseAddress = new Uri(_baseAddress);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        }

        public async Task<GetAuthToken> GetAuthToken()
        {

            GetAuthToken result = new GetAuthToken();
            var response = await _client.GetAsync(".auth/me");

            result.RequestMessage = response.RequestMessage.ToString();
            result.StatusCode = response.StatusCode.ToString();

            if (response.StatusCode == HttpStatusCode.OK)
                result.Response = await response.Content.ReadAsStringAsync();

            return result;

        }

    }

}
