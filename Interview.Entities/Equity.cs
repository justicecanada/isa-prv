using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class Equity : EntityBase
    {

        public Guid UserSettingsId { get; set; }
        public string? NameFR { get; set; }
        public string? NameEN { get; set; }
        public string? ViewFR { get; set; }
        public string? ViewEN { get; set; }

    }

}
