using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{
    
    public class Group : EntityBase
    {

        public Group()
        {
            ProcessGroups = new List<ProcessGroup>();
            Processes = new List<Process>();
            GroupOwners = new List<GroupOwner>();
        }

        public Guid ProcessId { get; set; }
        public string? NameFr { get; set; }
        public string? NameEn { get; set; }
        public bool IsDeleted { get; set; }
 
        public List<ProcessGroup> ProcessGroups { get; set; }
        public List<Process> Processes { get; set; }
        public List<GroupOwner> GroupOwners { get; set; }   

    }

}
