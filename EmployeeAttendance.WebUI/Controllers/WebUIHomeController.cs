using EmployeeAttendance.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeAttendance.WebUI.Controllers
{
    public class WebUIHomeController : Controller
    {
        private readonly ILogger<WebUIHomeController> _logger;

        public WebUIHomeController(ILogger<WebUIHomeController> logger)
        {
            _logger = logger;
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
