using AutoMapper;
using Interview.Entities;
using Interview.UI.Models;

namespace Interview.UI.Services.Automapper
{
    
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {

            CreateMap<Contest, VmContest>();
            CreateMap<VmContest, Contest>();

            CreateMap<UserSetting, VmUserSetting>();
            CreateMap<VmUserSetting, UserSetting>();

            CreateMap<Role, VmRole>();
            CreateMap<VmRole, Role>();

            CreateMap<Schedule, VmSchedule>();
            CreateMap<VmSchedule, Schedule>();

            CreateMap<ScheduleType, VmScheduleType>();
            CreateMap<VmScheduleType, ScheduleType>();


        }

    }

}
