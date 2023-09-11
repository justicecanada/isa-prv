using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interview.UI.Tests
{
    public class TestBase
    {

        protected object GetEntity<t>()
        {

            object result = Activator.CreateInstance<t>();
            PropertyInfo[] properties = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            // Set primitive types
            foreach (PropertyInfo property in properties)
            {

                if (property.PropertyType.FullName.ToLower() == "system.string")
                    property.SetValue(result, Guid.NewGuid().ToString().Substring(0, 3));
                else if (property.PropertyType.FullName.ToLower().Contains("system.int"))
                    property.SetValue(result, GetRandomNumber(property.PropertyType.FullName.ToLower()));
                else if (property.PropertyType.FullName.ToLower().Contains("system.datetimeoffset"))
                    property.SetValue(result, DateTimeOffset.Now);
                else if (property.PropertyType.FullName.ToLower().Contains("system.datetime"))
                    property.SetValue(result, DateTime.Now);
            }

            return result;

        }

        protected object GetRandomNumber(string fullName)
        {

            object result = null;
            byte[] bytes = new byte[100];
            Random random = new Random();

            if (fullName.Contains("int16"))
                result = Convert.ToInt16(random.Next(0, 1000));
            else if (fullName.Contains("int32"))
                result = Convert.ToInt32(random.Next(0, 1000));
            else if (fullName.Contains("int64"))
                result = Convert.ToInt32(random.Next(0, 1000));
            else if (fullName.Contains("double"))
                result = Convert.ToDouble(string.Format("{0}.{1}", random.Next(0, 1000), random.Next(0, 100)));
            else
                throw new NotImplementedException("Exception in GetRandomNumber: type not handled");

            return result;

        }

    }

}
