using CST_350_Minesweeper_Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace CST_350_Minesweeper_Website.Controllers
{
    public class UserController : Controller
    {
        public static UserCollection users = new UserCollection();
        public IActionResult Index()
        {
            return View();
        }
    }
}
