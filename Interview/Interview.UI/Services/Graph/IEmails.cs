using Interview.UI.Models.Graph;

namespace Interview.UI.Services.Graph
{
    
    public interface IEmails
    {

        Task<HttpResponseMessage> SendEmailAsync(EmailEnvelope emailEnvelope, string token);

    }

}
