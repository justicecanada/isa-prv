using AutoMapper;
using AutoMapper.Execution;
using Interview.UI.Models;
using Interview.Entities;
using Interview.UI.Models.Default;

namespace Interview.UI.Services.Automapper
{

    public class IntToTimeSpanConverter : IValueConverter<int?, TimeSpan?>
    {
        
        public TimeSpan? Convert(int? sourceMember, ResolutionContext context)
        {

            TimeSpan? result = null;

            if (sourceMember != null)
                result = TimeSpan.FromMinutes((int)sourceMember);

            return result;

        }

    }

    public class TimeSpanToIntConverter : IValueConverter<TimeSpan?, int?>
    {
        
        public int? Convert(TimeSpan? sourceMember, ResolutionContext context)
        {

            int? result = null;

            if (sourceMember != null)
                result = (int)sourceMember.Value.TotalMinutes;

            return result;

        }

    }

    public class DateTimeTimeSpanTimeZoneToDateTimeOffsetForInterview : IValueResolver<VmInterview, Interview.Entities.Interview, DateTimeOffset>
    {
        DateTimeOffset IValueResolver<VmInterview, Entities.Interview, DateTimeOffset>.Resolve(VmInterview source, Entities.Interview destination, DateTimeOffset destMember, ResolutionContext context)
        {

            DateTimeOffset result;

            //if (source.VmStartDate != null)
            //{
                TimeSpan startTime = source.VmStartTime == null ? TimeSpan.Parse("00:00:00") : (TimeSpan)source.VmStartTime;
                string input = $"{((DateTime)source.VmStartDate.Date).ToString("MM/dd/yyyy")} {startTime.ToString()}";

                result = DateTimeOffset.Parse(input);
            //}

            return result;


        }
    }

    public class DateTimeOffsetToDateTimeForInterview : IValueResolver<Interview.Entities.Interview, VmInterview, DateTime>
    {
        
        public DateTime Resolve(Entities.Interview source, VmInterview destination, DateTime destMember, ResolutionContext context)
        {

            DateTime result = source.StartDate.Date;

            return result;

        }

    }

    public class DateTimeOffSetToTimeSpanForInterview : IValueResolver<Interview.Entities.Interview, VmInterview, TimeSpan>
    {
        
        public TimeSpan Resolve(Entities.Interview source, VmInterview destination, TimeSpan destMember, ResolutionContext context)
        {

            TimeSpan result = source.StartDate.TimeOfDay;

            return result;

        }

    }

    public class DateTimeTimeSpanTimeZoneToDateTimeOffsetForInterviewModal : IValueResolver<VmInterviewModal, Interview.Entities.Interview, DateTimeOffset>
    {
        DateTimeOffset IValueResolver<VmInterviewModal, Entities.Interview, DateTimeOffset>.Resolve(VmInterviewModal source, Entities.Interview destination, DateTimeOffset destMember, ResolutionContext context)
        {

            DateTimeOffset result;

            //if (source.VmStartDate != null)
            //{
            TimeSpan startTime = source.VmStartTime == null ? TimeSpan.Parse("00:00:00") : (TimeSpan)source.VmStartTime;
            string input = $"{((DateTime)source.VmStartDate.Date).ToString("MM/dd/yyyy")} {startTime.ToString()}";

            result = DateTimeOffset.Parse(input);
            //}

            return result;


        }
    }

    public class DateTimeOffsetToDateTimeForInterviewModal : IValueResolver<Interview.Entities.Interview, VmInterviewModal, DateTime>
    {

        public DateTime Resolve(Entities.Interview source, VmInterviewModal destination, DateTime destMember, ResolutionContext context)
        {

            DateTime result = source.StartDate.Date;

            return result;

        }

    }

    public class DateTimeOffSetToTimeSpanForInterviewForInterviewModal : IValueResolver<Interview.Entities.Interview, VmInterviewModal, TimeSpan>
    {

        public TimeSpan Resolve(Entities.Interview source, VmInterviewModal destination, TimeSpan destMember, ResolutionContext context)
        {

            TimeSpan result = source.StartDate.TimeOfDay;

            return result;

        }

    }

}
