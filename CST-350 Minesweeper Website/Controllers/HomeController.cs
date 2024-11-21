using CST_350_Minesweeper_Website.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;

namespace CST_350_Minesweeper_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // If the default theme isn't set, set it here
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("FormColor")))
            {
                SetDefaultTheme();
            }
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

        public void SetDefaultTheme()
        {
            HttpContext.Session.SetString("FormColor", "rgb(255, 255, 255)");  // Default background color
            HttpContext.Session.SetString("VisitedColor1", "rgb(219, 219, 219)");  // Default visited color 1
            HttpContext.Session.SetString("VisitedColor2", "rgb(200, 200, 200)");  // Default visited color 2
            HttpContext.Session.SetString("UnvisitedColor1", "rgb(131, 131, 131)");  // Default unvisited color 1
            HttpContext.Session.SetString("UnvisitedColor2", "rgb(115, 115, 115)");  // Default unvisited color 2
            HttpContext.Session.SetString("TextColor", "rgb(10, 10, 10)");  // Default text color
        }
    }
}
