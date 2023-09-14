using Microsoft.AspNetCore.Mvc.Filters;
using Interview.UI;
using System.Reflection;

namespace Interview.UI.Filters
{

    public class AssemblyVersionFilter : IActionFilter
    {

        

        private string AssemblyVersion
        {
            get {

                string result;

                if (Config.Application.ContainsKey(Constants.AssemblyVersionKey))
                    result =  Config.Application[Constants.AssemblyVersionKey];
                else
                {
                    
                    AssemblyInformationalVersionAttribute infoVersion = 
                        (AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly()
                        .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).FirstOrDefault();
                    result =  infoVersion.InformationalVersion;

                    Config.Application[Constants.AssemblyVersionKey] = result;

                }

                return result;

            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) 
        {

            string assemblyVersion = AssemblyVersion;

            ((Microsoft.AspNetCore.Mvc.Controller)filterContext.Controller).ViewData[Constants.AssemblyVersionKey] = assemblyVersion;

        }

        public void OnActionExecuted(ActionExecutedContext filterContext) 
        { 
        
        
        
        }

    }

}
