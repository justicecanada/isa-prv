using AutoMapper;
using Interview.Entities;
using Interview.UI.Models;
using System.Xml;

namespace Interview.UI.Services.Automapper
{
    
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {

            CreateMap<Contest, VmContest>()
                .ForMember(d => d.InterviewDuration, opt => opt.ConvertUsing(new TimeSpanToIntConverter()));
            CreateMap<VmContest, Contest>()
                .ForMember(d => d.InterviewDuration, opt => opt.ConvertUsing(new IntToTimeSpanConverter()));

            CreateMap<UserSetting, VmUserSetting>();
            CreateMap<VmUserSetting, UserSetting>();

            CreateMap<Role, VmRole>();
            CreateMap<VmRole, Role>();

            CreateMap<Schedule, VmSchedule>();
            CreateMap<VmSchedule, Schedule>();

            CreateMap<ScheduleType, VmScheduleType>();
            CreateMap<VmScheduleType, ScheduleType>();

            CreateMap<UserLanguage, VmUserLanguage>();
            CreateMap<VmUserLanguage, UserLanguage>();

            CreateMap<UserSetting, VmUserSetting>();
            CreateMap<VmUserSetting, UserSetting>();

            CreateMap<Equity, VmEquity>();
            CreateMap<VmEquity, Equity>();

        }

    }

}
