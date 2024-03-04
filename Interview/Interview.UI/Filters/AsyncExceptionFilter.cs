using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;

namespace Interview.UI.Filters
{

    public class AsyncExceptionFilter : IAsyncExceptionFilter
    {

        private readonly ILogger<AsyncExceptionFilter> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AsyncExceptionFilter(ILogger<AsyncExceptionFilter> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {

            RedirectToActionResult result;
            Exception exception = context.Exception;

            // Log exception
            string exceptionId = Guid.NewGuid().ToString().Substring(0, 8);
            string userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            bool isAjaxRequest = GetIsAjaxRequest(context.HttpContext.Request);

            // Log exception
            var msgObj = GetExceptionDetails(exception, exceptionId, userName);
            var msg = JsonConvert.SerializeObject(msgObj);           

            _logger.LogError(exception, msg);

            // Handle response
            if (isAjaxRequest)
                result = new RedirectToActionResult("IndexModal", "Error", new { area = "", exceptionId = exceptionId });
            else
                result = new RedirectToActionResult("Index", "Error", new { area = "", exceptionId = exceptionId });

            context.Result = result;
            context.ExceptionHandled = true;

            return;

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
