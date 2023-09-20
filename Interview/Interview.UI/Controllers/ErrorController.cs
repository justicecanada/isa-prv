using AutoMapper;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class ErrorController : GoC.WebTemplate.CoreMVC.Controllers.WebTemplateBaseController
    {
        public ErrorController(GoC.WebTemplate.Components.Core.Services.IModelAccessor modelAccessor)
            : base(modelAccessor)
        {

        }

        public IActionResult Index(string exceptionId)
        {

            ViewBag.ExceptionId = exceptionId;

            return View();

        }

    }

}
