using Interview.Entities;
using Interview.UI.Models;
using Interview.UI.Models.Graph;

namespace Interview.UI.Services.Graph
{
    
    public interface IEmails
    {

        Task<HttpResponseMessage> SendEmailAsync(EmailEnvelope emailEnvelope, string token, string userName);

        List<EmailRecipent> GetEmailRecipients(string recipients);

        EmailEnvelope GetEmailEnvelopeForCandidateAddedByHR(EmailTemplate emailTemplate, Process process, VmInterview vmInterview, string email);

        EmailEnvelope GetEmailEnvelopeForCandidateRegisteredTimeSlot(EmailTemplate emailTemplate, Process process, VmInterview vmInterview, string email);

    }

}
