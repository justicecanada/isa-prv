using Microsoft.AspNetCore.Mvc;

namespace Interview.UI.Controllers
{
    
    public class ErrorController : Controller
    {

        public IActionResult Index(string exceptionId)
        {

            ViewBag.ExceptionId = exceptionId;

            return View();

        }

    }

}
