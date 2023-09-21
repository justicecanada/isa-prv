using AutoMapper;

namespace Interview.UI.Services.Automapper
{

    public class IntToTimeSpanConverter : IValueConverter<string?, TimeSpan?>
    {
        
        public TimeSpan? Convert(string? sourceMember, ResolutionContext context)
        {

            TimeSpan? result = null;

            if (!string.IsNullOrEmpty(sourceMember))
            {
                var nonNullSourceMember = sourceMember.ToString();
                result = TimeSpan.FromMinutes(System.Convert.ToInt32(nonNullSourceMember));
            }

            return result;

        }

    }

    public class TimeSpanToIntConverter : IValueConverter<TimeSpan?, string?>
    {
        public string? Convert(TimeSpan? sourceMember, ResolutionContext context)
        {

            string? result = null;

            if (sourceMember != null)
                result = sourceMember.Value.TotalMinutes.ToString();

            return result;

        }
    }

}
