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

            CreateMap<Schedule, VmSchedule>();
            CreateMap<VmSchedule, Schedule>();

            CreateMap<UserSetting, VmUserSetting>();
            CreateMap<VmUserSetting, UserSetting>();

            CreateMap<Equity, VmEquity>();
            CreateMap<VmEquity, Equity>();

            CreateMap<UserSettingEquity, VmUserSettingEquity>();
            CreateMap<VmUserSettingEquity, UserSettingEquity>();

            CreateMap<Interview.Entities.Interview, VmInterview>();
            CreateMap<VmInterview, Interview.Entities.Interview>();


            #region ~/Groups

            CreateMap<Contest, Interview.UI.Models.Groups.VmContest>();
            CreateMap<Interview.UI.Models.Groups.VmContest, Contest>();

            CreateMap<ContestGroup, Interview.UI.Models.Groups.VmContestGroup>();
            CreateMap<Interview.UI.Models.Groups.VmContestGroup, ContestGroup>();

            CreateMap<Group, Interview.UI.Models.Groups.VmGroup>();
            CreateMap<Interview.UI.Models.Groups.VmGroup, Group>();

            CreateMap<GroupOwner, Interview.UI.Models.Groups.VmGroupOwner>();
            CreateMap<Interview.UI.Models.Groups.VmGroupOwner, GroupOwner>();

            #endregion

        }

    }

}
