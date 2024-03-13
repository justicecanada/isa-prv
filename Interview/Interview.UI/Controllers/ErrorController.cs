using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    
    public class ErrorController : BaseController
    {
            
        public ErrorController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IStringLocalizer<BaseController> baseLocalizer,
            IOptions<SessionTimeoutOptions> sessionTimeoutOptions) 
            : base(modelAccessor, dal, baseLocalizer, sessionTimeoutOptions)
        {

        }

        public IActionResult Index(string exceptionId)
        {

            ViewBag.ExceptionId = exceptionId;

            return View();

        }

        public PartialViewResult IndexModal(string exceptionId)
        {

            ViewBag.ExceptionId = exceptionId;

            return PartialView();

        }

    }

}
