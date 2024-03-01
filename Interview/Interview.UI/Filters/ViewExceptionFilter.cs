using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.IO.Pipelines;
using System.Text;

namespace Interview.UI.Filters
{
    
    public class ViewExceptionFilter : ResultFilterAttribute
    {

        private readonly ILogger<ViewExceptionFilter> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewExceptionFilter(ILogger<ViewExceptionFilter> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async void OnResultExecuted(ResultExecutedContext context)
        {
            
            if (context.Exception != null)
            {

                Exception exception = context.Exception;
                string exceptionId = Guid.NewGuid().ToString().Substring(0, 8);
                string userName = _httpContextAccessor.HttpContext.User.Identity.Name;

                var msgObj = new { message = exception.Message, exceptionId = exceptionId, userName = userName, stacktrace = exception.StackTrace, exceptionHandler = "ViewExceptionFilter" };
                var msg = JsonConvert.SerializeObject(msgObj);

                _logger.LogError(exception, msg);

            }
            base.OnResultExecuted(context);

        }

    }

}
