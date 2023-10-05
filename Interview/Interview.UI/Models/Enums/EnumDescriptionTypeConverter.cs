using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Interview.UI.Models.Enums
{
    
    public class EnumDescriptionTypeConverter : EnumConverter
    {

        //https://www.youtube.com/watch?v=T1mhORJCDsY

        public EnumDescriptionTypeConverter(Type type) : base(type)
        {

        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                        return (attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].Description)) ? attributes[0].Description : value.ToString();
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);

        }

    }

}
