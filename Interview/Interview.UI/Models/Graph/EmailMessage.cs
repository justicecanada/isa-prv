namespace Interview.UI.Models.Graph
{
    
    public class EmailMessage
    {

        public EmailMessage()
        {
            ccRecipients = new List<EmailRecipent>();
        }

        public string subject { get; set; }
        public EmailBody body { get; set; }
        public List<EmailRecipent> toRecipients { get; set; }
        public List<EmailRecipent> ccRecipients { get; set; }

    }

}
