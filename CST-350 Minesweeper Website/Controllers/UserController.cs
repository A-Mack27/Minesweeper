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

        /// <summary>
        /// Controller to be called to process the user login
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        public IActionResult ProcessLogin(LoginViewModel loginViewModel)
        {
            // Check for a match
            UserModel result = users.CheckCredentials(loginViewModel.Username, loginViewModel.Password);
            // 
            if (result.Id != 0)
            {
                UserModel user = users.GetUserById(result.Id);
                return View("LoginSuccess", user);
            }
            return View("LoginFailure");
        }
    }
}
