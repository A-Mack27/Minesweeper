using CST_350_Minesweeper_Website.Models;
using CST_350_Register_and_Login_App.Models;
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
            int result = -1;
            // Check for a match
            result = users.CheckCredentials(loginViewModel.Username, loginViewModel.Password);
            // 
            if (result == 0)
            {
                UserModel user = users.GetUserById(result);
                return View("LoginSuccess", user);
            }
            return View("LoginFailure");
        }
    }
}
