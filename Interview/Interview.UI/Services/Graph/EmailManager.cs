using Interview.UI.Models.Graph;
using Newtonsoft.Json;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Formats.Asn1.AsnWriter;

namespace Interview.UI.Services.Graph
{
    
    public class EmailManager : IEmails
    {

        #region Declarations

        private readonly HttpClient _client;
        private readonly string _host = "https://graph.microsoft.com";

        #endregion

        #region Constructors

        public EmailManager(HttpClient client)
        {

            _client = client;
            _client.BaseAddress = new Uri(_host);

        }

        #endregion

        #region Public Interface Methods

        public async Task<HttpResponseMessage> SendEmailAsync(EmailEnvelope emailEnvelope, string token)
        {

            HttpResponseMessage result = null;
            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
              { "message", JsonConvert.SerializeObject(emailEnvelope) }
            });
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_host}/v1.0/me/sendMail"))
            {
                Content = content,
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {token}" }
                }
            };

            result = await _client.SendAsync(request);

            return result;

        }

        #endregion

    }

}
