using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class EmailType : EntityBase
    {

        public string? NameFR { get; set; }
        public string? NameEN { get; set; }
        public string? DescFR { get; set; }
        public string? DescEN { get; set; }

        public List<EmailTemplate> EmailTemplates { get; set; }

    }

}
