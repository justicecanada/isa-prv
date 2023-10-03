using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Models.AppSettings;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Interview.UI.Controllers
{
    
    public class ErrorController : BaseController
    {
            
        public ErrorController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper, IOptions<JusticeOptions> justiceOptions) 
            : base(modelAccessor, justiceOptions, dal)
        {

        }

        public IActionResult Index(string exceptionId)
        {

            ViewBag.ExceptionId = exceptionId;

            return View();

        }

    }

}
