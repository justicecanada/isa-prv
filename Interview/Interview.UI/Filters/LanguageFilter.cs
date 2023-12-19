using Azure;
using GoC.WebTemplate.CoreMVC.Controllers;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Interview.UI.Filters
{

    public class LanguageFilter : IActionFilter
    {
        
        private const string _langSwitchQueryStringKey = "GoCTemplateCulture";
        //private const string _englishCulture = "en-CA";
        //private const string _frenchCulture = "fr-CA";

        public void OnActionExecuted(ActionExecutedContext context)
        {

            //if (context.HttpContext.Request.Query.ContainsKey(_langSwitchQueryStringKey))
            //{

            //    string cultureName = context.HttpContext.Request.Query[_langSwitchQueryStringKey];
            //    RequestCulture requestCulture = new RequestCulture(cultureName);
            //    string cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

            //    CultureInfo.CurrentCulture = new CultureInfo(cultureName);
            //    CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
            //    context.HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, cookieValue);

            //}

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            if (context.HttpContext.Request.Query.ContainsKey(_langSwitchQueryStringKey))
            {

                // Handle .Net Core Concerns                
                string cultureName = context.HttpContext.Request.Query[_langSwitchQueryStringKey];
                RequestCulture requestCulture = new RequestCulture(cultureName);
                string cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                CultureInfo.CurrentCulture = new CultureInfo(cultureName);
                CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
                context.HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, cookieValue);

            }

        }

    }

}
