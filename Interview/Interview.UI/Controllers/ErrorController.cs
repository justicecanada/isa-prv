using AutoMapper;
using GoC.WebTemplate.Components.Core.Services;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class ErrorController : BaseController
    {
        public ErrorController(IModelAccessor modelAccessor, DalSql dal, IMapper mapper) : base(modelAccessor)
        {

        }

        public IActionResult Index(string exceptionId)
        {

            ViewBag.ExceptionId = exceptionId;

            return View();

        }

    }

}
