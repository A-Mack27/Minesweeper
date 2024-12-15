using CST_350_Minesweeper_Website.Filters;
using CST_350_Minesweeper_Website.Models;
using CST_350_Minesweeper_Website.Services.DataAccess;
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
            // Remove all session variables and return them to the home page
            HttpContext.Session.Clear();

            HttpContext.Session.SetString("FormColor", "rgb(255, 255, 255)");  // Default background color
            HttpContext.Session.SetString("VisitedColor1", "rgb(219, 219, 219)");  // Default visited color 1
            HttpContext.Session.SetString("VisitedColor2", "rgb(200, 200, 200)");  // Default visited color 2
            HttpContext.Session.SetString("UnvisitedColor1", "rgb(131, 131, 131)");  // Default unvisited color 1
            HttpContext.Session.SetString("UnvisitedColor2", "rgb(115, 115, 115)");  // Default unvisited color 2
            HttpContext.Session.SetString("TextColor", "rgb(10, 10, 10)");  // Default text color

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
            string userJson = "";
            // Here's a new instance of UserCollection to manage the users
            UserCollection users = new UserCollection();
            //  Create a new user to represent the data entered during the login attempt
            UserModel userData = users.CheckCredentials(username, password);
            // If the ID isn't 0, the login was successful
            if (userData.Id != 0)
            {
                // Serialize and store the user data with the key 'user' to be referenced later
                userJson = ServiceStack.Text.JsonSerializer.SerializeToString(userData);
                HttpContext.Session.SetString("User", userJson);
                return View("LoginSuccess", userData); // Return the success view
            }
            return View("LoginFailure", new LoginViewModel()); // Return the failure view
        }
    }
}