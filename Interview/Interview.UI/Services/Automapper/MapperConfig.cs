using AutoMapper;
using Interview.Entities;
using Interview.UI.Models;
using System.Globalization;
using System;
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

            CreateMap<RoleUser, VmRoleUser>();
            CreateMap<VmRoleUser, RoleUser>();

            CreateMap<Schedule, VmSchedule>();
            CreateMap<VmSchedule, Schedule>();

            CreateMap<RoleUser, VmRoleUser>();
            CreateMap<VmRoleUser, RoleUser>();

            CreateMap<Equity, VmEquity>();
            CreateMap<VmEquity, Equity>();

            CreateMap<RoleUserEquity, VmRoleUserEquity>();
            CreateMap<VmRoleUserEquity, RoleUserEquity>();

            CreateMap<Interview.Entities.Interview, VmInterview>()
                .ForMember(x => x.VmStartDate, opt => opt.MapFrom<DateTimeOffsetToDateTime>())
                .ForMember(x => x.VmStartTime, opt => opt.MapFrom<DateTimeOffSetToTimeSpan>());
            CreateMap<VmInterview, Interview.Entities.Interview>()
                .ForMember(x => x.StartDate, opt => opt.MapFrom<DateTimeTimeSpanTimeZoneToDateTimeOffset>());

            CreateMap<InterviewUser, VmInterview>();
            CreateMap<VmInterview, VmInterviewUser>();

            CreateMap<EmailTemplate, VmEmailTemplate>();
            CreateMap<VmEmailTemplate, EmailTemplate>();

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
