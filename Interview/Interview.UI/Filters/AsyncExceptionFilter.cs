using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

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

            var msgObj = new { message = exception.Message, exceptionId = exceptionId, userName = userName, stacktrace = exception.StackTrace };
            var msg = JsonConvert.SerializeObject(msgObj);

            _logger.LogError(exception, msg);

            result = new RedirectToActionResult("IndexModal", "Error", new { area = "", exceptionId = exceptionId });
            context.Result = result;

            context.ExceptionHandled = true;

            return;

        }

    }

}
