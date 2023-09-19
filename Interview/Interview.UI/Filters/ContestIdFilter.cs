using Microsoft.AspNetCore.Mvc.Filters;

namespace Interview.UI.Filters
{
    
    public class ContestIdFilter : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string contestId = filterContext.HttpContext.Request.Query["contestId"].ToString();

            ((Microsoft.AspNetCore.Mvc.Controller)filterContext.Controller).ViewData["contestId"] = contestId;

        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {



        }

    }

}
