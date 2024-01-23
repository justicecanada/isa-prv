using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Interview.UI.Filters
{
    
    public class ExceptionFilter : IExceptionFilter
    {

        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {

            Exception exception = context.Exception;
            bool isAjaxRequest = GetIsAjaxRequest(context.HttpContext.Request);
            string exceptionId = Guid.NewGuid().ToString().Substring(0, 8);

            var msgObj = new { message = exception.Message, exceptionId = exceptionId, stacktrace = exception.StackTrace };
            var msg = JsonConvert.SerializeObject(msgObj);

            _logger.LogError(exception, msg);

            if (isAjaxRequest)
            {
                context.HttpContext.Response.StatusCode = 500;
            }
            else
            {
                //context.HttpContext.Response.StatusCode = 200;                        
                //context.HttpContext.Response.Redirect(string.Format("/Error/Index?exceptionId={0}", exceptionId));

                var result = new RedirectToActionResult("Index", "Error", new { area = "", exceptionId = exceptionId });
                context.Result = result;

            }

        }

        private bool GetIsAjaxRequest(HttpRequest request)
        {

            bool result = false;

            if (request.Headers["X-Requested-With"] == "XMLHttpRequest")
                result = true;

            return result;

        }


    }

}
