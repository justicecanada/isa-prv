namespace Interview.UI.Models.Emails
{
    
    public class VmIndex
    {

        public VmIndex()
        {
            EmailTemplates = new List<VmEmailTemplate>();
        }

        public List<VmEmailTemplate> EmailTemplates { get; set; }

    }

}
