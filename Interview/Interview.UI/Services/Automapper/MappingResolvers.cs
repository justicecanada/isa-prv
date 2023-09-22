using AutoMapper;

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

}
