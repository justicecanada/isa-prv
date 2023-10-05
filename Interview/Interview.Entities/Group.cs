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
            ContestGroups = new List<ContestGroup>();
            Contests = new List<Contest>();
            GroupOwners = new List<GroupOwner>();
        }

        public Guid ContestId { get; set; }
        public string? NameFr { get; set; }
        public string? NameEn { get; set; }
        public bool IsDeleted { get; set; }
 
        public List<ContestGroup> ContestGroups { get; set; }
        public List<Contest> Contests { get; set; }
        public List<GroupOwner> GroupOwners { get; set; }   

    }

}
