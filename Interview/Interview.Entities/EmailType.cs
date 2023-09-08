using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class EmailType : EntityBase
    {

        public Guid EmailTemplateId { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set; }
        public string? DescFR { get; set; }
        public string? DescEN { get; set; }

    }

}
