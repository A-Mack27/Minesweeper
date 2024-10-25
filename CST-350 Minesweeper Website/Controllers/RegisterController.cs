using CST_350_Minesweeper_Website.Models;
using CST_350_Minesweeper_Website.Services.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CST_350_Minesweeper_Website.Controllers
{
    public class RegisterController : Controller
    {
        /// <summary>
        /// Show the registration form
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Take the inputs from the register page and processes them
        /// </summary>
        /// <returns></returns>
        public IActionResult ProcessRegister(RegisterViewModel registerViewModel)
        {
            // Create a new user model to hold the new user
            UserModel user = new UserModel();

            // Set all the attributes of the new user to the form entries
            user.Username = registerViewModel.Username;
            user.SetPassword(registerViewModel.PasswordHash);
            user.FirstName = registerViewModel.FirstName;
            user.LastName = registerViewModel.LastName;
            user.Age = registerViewModel.Age;
            user.Sex = registerViewModel.Sex;
            user.Email = registerViewModel.Email;
            // Add the user to the list
            UserCollection._users.Add(user);
            // Send the user back to the index

            // Save the user to the database using UserDAO
            UserDAO userDAO = new UserDAO();
            int result = userDAO.AddUser(user);

            if (result > 0)
            {
                // If user registration is successful, redirect to login page
                return View("RegisterSuccess");
            }
            else
            {
                // If there was an error, show an error message
                return View("RegisterFailure");
            }
        }
    }
}
