using CST_350_Minesweeper_Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace CST_350_Minesweeper_Website.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProcessLogin(string username, string password)
        {
            int result = -1;
            string userJson = "";

            UserCollection users = new UserCollection();

            UserModel userData = new() { Id = 1, Username = username, PasswordHash = password };

            if (users.CheckCredentials(username, password) > 0)
            {
                userJson = ServiceStack.Text.JsonSerializer.SerializeToString(userData);

                HttpContext.Session.SetString("User", userJson);

                return View("LoginSuccess", userData);
            }
            return View("LoginFailure", userData);
        }
    }
}