using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{

    public enum ScheduleTypes
    {
        Candidate = 1,
        Members = 2,
        Marking = 3
    }

    public enum UserTypes
    {
        Internal = 0,
        ExistingExternal = 1,
        NewExternal = 2
    }

    public enum RoleTypes
    {
        HR = 1,
        INTERVIEWER = 2,
        LEAD = 3,
        ASSISTANT = 4,
        CANDIDATE = 5,
        ADMIN = 6
    }

}
