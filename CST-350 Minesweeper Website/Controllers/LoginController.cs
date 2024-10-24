using CST_350_Minesweeper_Website.Filters;
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

        /// <summary>
        /// Logs the user out of the website
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return View("Index");
        }

        /// <summary>
        /// Processes the inputted login information and sends the user to the corresponding page
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public IActionResult ProcessLogin(string username, string password)
        {
            int result = -1;
            string userJson = "";

            UserCollection users = new UserCollection();

            UserModel userData = new() { Id = 1, Username = username, PasswordHash = password };

            if (users.CheckCredentials(username, password).Id != 0)
            {
                userJson = ServiceStack.Text.JsonSerializer.SerializeToString(userData);

                HttpContext.Session.SetString("User", userJson);

                return View("LoginSuccess", userData);
            }
            return View("LoginFailure", new LoginViewModel());
        }
    }
}