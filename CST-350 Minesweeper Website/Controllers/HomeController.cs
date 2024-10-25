using CST_350_Minesweeper_Website.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        // Might move this to a GameController class in the future
        // Add this new action for StartGame
        public IActionResult StartGame()
        {
            // Check if the user session is active
            if (HttpContext.Session.GetString("User") == null)
            {
                // If no session, redirect to login page
                return RedirectToAction("Index", "Login");
            }

            // Otherwise, return the StartGame view
            return View();
        }
    }
}
