using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Interview.UI.Filters
{
    
    public class ExceptionFilter : IExceptionFilter
    {

        private readonly ILogger<ExceptionFilter> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExceptionFilter(ILogger<ExceptionFilter> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnException(ExceptionContext context)
        {

            Exception exception = context.Exception;
            bool isAjaxRequest = GetIsAjaxRequest(context.HttpContext.Request);
            string exceptionId = Guid.NewGuid().ToString().Substring(0, 8);
            string userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            var msgObj = GetExceptionDetails(exception, exceptionId, userName);
            var msg = JsonConvert.SerializeObject(msgObj);

            _logger.LogError(exception, msg);

            var result = new RedirectToActionResult("Index", "Error", new { area = "", exceptionId = exceptionId });
            context.Result = result;

        }

        private bool GetIsAjaxRequest(HttpRequest request)
        {

            bool result = false;

            if (request.Headers["X-Requested-With"] == "XMLHttpRequest")
                result = true;

            return result;

        }

        private object GetExceptionDetails(Exception exception, string exceptionId, string userName)
        {

            object result = new
            {
                message = exception.Message,
                exceptionId = exceptionId,
                userName = userName,
                stacktrace = exception.StackTrace,
                innerException = exception.InnerException == null ? null : GetExceptionDetails(exception.InnerException, exceptionId, userName)
            };

            return result;

        }

    }

}
