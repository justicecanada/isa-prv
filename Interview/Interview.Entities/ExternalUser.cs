using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class ExternalUser : EntityBase
    {

        public string? GivenName { get; set; }

        public string? SurName { get; set; }

        public string? Email { get; set; }

    }

}
