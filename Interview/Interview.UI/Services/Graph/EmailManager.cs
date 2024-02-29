using Interview.Entities;
using Interview.UI.Models;
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
        private readonly IOptions<EmailManagerOptions> _emailManagerOptions;

        private const string _host = "https://graph.microsoft.com";

        #endregion

        #region Constructors

        public EmailManager(HttpClient client, IOptions<TokenOptions> tokenOptions, IOptions<EmailManagerOptions> emailManagerOptions)
        {

            _client = client;
            _client.BaseAddress = new Uri(_host);

            _tokenOptions = tokenOptions;
            _emailManagerOptions = emailManagerOptions;

        }

        #endregion

        #region Public Interface Methods

        public async Task<HttpResponseMessage> SendEmailAsync(EmailEnvelope emailEnvelope, string token, string userName)
        {

            // https://learn.microsoft.com/en-us/graph/api/user-sendmail?view=graph-rest-1.0&tabs=http#example-1-send-a-new-email-using-json-format

            HttpResponseMessage result = null;

            if (_emailManagerOptions.Value.OnlySendToLoggedInUser)
            {
                var loggedInRecipient = emailEnvelope.message.toRecipients.Where(x => x.emailAddress.address.ToLower() == userName.ToLower()).FirstOrDefault();
                if (loggedInRecipient == null)
                    return result;
                else
                {
                    emailEnvelope.message.toRecipients.Clear();
                    emailEnvelope.message.toRecipients.Add(loggedInRecipient);
                }
            }

            var serializedEnvelope = JsonConvert.SerializeObject(emailEnvelope);
            var content = new StringContent(serializedEnvelope, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_host}/v1.0/users/{userName}/sendMail"))
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

        public List<EmailRecipent> GetEmailRecipients(string recipients)
        {

            List<EmailRecipent> result = new List<EmailRecipent>();
            string[] addresses = string.IsNullOrEmpty(recipients) ? new string[0] : recipients.Split(',');

            foreach (string address in addresses)
            {
                EmailRecipent recipent = new EmailRecipent();
                recipent.emailAddress.address = address;
                result.Add(recipent);
            }

            return result;

        }

        public EmailEnvelope GetEmailEnvelopeForCandidateAddedByHR(EmailTemplate emailTemplate, Process process, VmInterview vmInterview, string email, string callbackUrl)
        {

            EmailEnvelope result = null;

            if (emailTemplate != null && process.Schedules.Count != 0)
            {

                string noProcess = process.NoProcessus;
                string groupNiv = process.GroupNiv;
                string startDate = vmInterview.VmStartDate.ToLongDateString();
                string startTime = vmInterview.VmStartDate.ToLongTimeString();
                string startOral = "Figure this out";
                string location = $"{vmInterview.Location} {vmInterview.Room}";
                string contactName = string.IsNullOrEmpty(vmInterview.ContactName) ? process.ContactName : vmInterview.ContactName;
                string contactNumber = string.IsNullOrEmpty(vmInterview.ContactNumber) ? process.ContactNumber : vmInterview.ContactNumber;
                List<EmailRecipent> toRecipients = GetEmailRecipients(email);

                string body = emailTemplate.EmailBody
                    .Replace("{0}", noProcess)
                    .Replace("{1}", groupNiv)
                    .Replace("{2}", startDate)
                    .Replace("{3}", startTime)
                    .Replace("{4}", startOral)
                    .Replace("{5}", location)
                    .Replace("{6}", contactName)
                    .Replace("{7}", contactNumber)
                    .Replace("{callbackUrl}", callbackUrl);

                result = new EmailEnvelope()
                {
                    message = new EmailMessage()
                    {
                        subject = emailTemplate.EmailSubject,
                        body = new EmailBody()
                        {
                            contentType = "HTML",
                            content = body
                        },
                        toRecipients = toRecipients,
                        //ccRecipients = GetEmailRecipients(emailTemplate.CcRecipients),
                    },
                    saveToSentItems = "false"
                };

            }

            return result;

        }

        public EmailEnvelope GetEmailEnvelopeForCandidateRegisteredTimeSlot(EmailTemplate emailTemplate, Process process, VmInterview vmInterview, string email, string callbackUrl)
        {

            EmailEnvelope result = null;

            if (emailTemplate != null && process.Schedules.Count != 0)
            {

                string noProcess = process.NoProcessus;
                string groupNiv = process.GroupNiv;
                string startDate = vmInterview.VmStartDate.ToLongDateString();
                string startTime = vmInterview.VmStartDate.ToLongTimeString();
                string startOral = "Figure this out";
                string location = $"{vmInterview.Location} {vmInterview.Room}";
                string contactName = string.IsNullOrEmpty(vmInterview.ContactName) ? process.ContactName : vmInterview.ContactName;
                string contactNumber = string.IsNullOrEmpty(vmInterview.ContactNumber) ? process.ContactNumber : vmInterview.ContactNumber;
                List<EmailRecipent> toRecipients = GetEmailRecipients(email);

                string body = emailTemplate.EmailBody
                    .Replace("{0}", noProcess)
                    .Replace("{1}", groupNiv)
                    .Replace("{2}", startDate)
                    .Replace("{3}", startTime)
                    .Replace("{4}", startOral)
                    .Replace("{5}", location)
                    .Replace("{6}", contactName)
                    .Replace("{7}", contactNumber)
                    .Replace("{callbackUrl}", callbackUrl);

                result = new EmailEnvelope()
                {
                    message = new EmailMessage()
                    {
                        subject = emailTemplate.EmailSubject,
                        body = new EmailBody()
                        {
                            contentType = "HTML",
                            content = body
                        },
                        toRecipients = toRecipients,
                        //ccRecipients = GetEmailRecipients(emailTemplate.CcRecipients),
                    },
                    saveToSentItems = "false"
                };

            }

            return result;

        }

        #endregion

    }

}
