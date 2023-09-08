using Interview.UI.Models;
using Interview.UI.Services.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Interview.UI.Controllers
{
    public class HomeController : Controller
    {

        private readonly IDal _dal;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IDal dal)
        {
            _logger = logger;
            _dal = dal;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}