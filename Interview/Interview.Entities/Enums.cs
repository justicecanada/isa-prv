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

    public enum RoleUserTypes
    {
        HR = 1,
        Interviewer = 2,
        Lead = 3,
        Assistant = 4,
        Candidate = 5,
        Admin = 6
    }

    public enum LanguageTypes
    {
        English = 1,
        French = 2,
        Bilingual = 3
    }

    public enum EmailTypes
    {
        CandidateRegisteredTimeSlot = 1,
        CandidateAddedByHR = 2,
        CandidateInterviewReminder = 3,
        CandidateExternal = 4
    }

}
