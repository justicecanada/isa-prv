using Interview.UI.Models.AppSettings;
using Interview.UI.Models.Graph;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Formats.Asn1.AsnWriter;

namespace Interview.UI.Services.Graph
{
    
    public class EmailManager : IEmails
    {

        #region Declarations

        private readonly HttpClient _client;
        private readonly IOptions<TokenOptions> _tokenOptions;

        private const string _host = "https://graph.microsoft.com";

        #endregion

        #region Constructors

        public EmailManager(HttpClient client, IOptions<TokenOptions> tokenOptions)
        {

            _client = client;
            _client.BaseAddress = new Uri(_host);

            _tokenOptions = tokenOptions;

        }

        #endregion

        #region Public Interface Methods

        public async Task<HttpResponseMessage> SendEmailAsync(EmailEnvelope emailEnvelope, string token, string userName)
        {

            // https://learn.microsoft.com/en-us/graph/api/user-sendmail?view=graph-rest-1.0&tabs=http#example-1-send-a-new-email-using-json-format

            HttpResponseMessage result = null;
            var serializedEnvelope = JsonConvert.SerializeObject(emailEnvelope);
            var content = new StringContent(serializedEnvelope, Encoding.UTF8, "application/json");
            //var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_host}/v1.0/me/sendMail"))
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_host}/v1.0/users/{userName}/sendMail"))
            {
                Content = content,
                Headers =
                {
                    //{ "Content-type",  "application/json" },              // Throws exception
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" }
                }
            };

            result = await _client.SendAsync(request);

            return result;

        }

        #endregion

    }

}
