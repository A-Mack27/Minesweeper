
using CST_350_Minesweeper_Website.Interfaces;
using CST_350_Minesweeper_Website.Services.DataAccess;

namespace CST_350_Minesweeper_Website.Models
{
    public class UserCollection : IUserManager
    {
        // A list to hold all users
        public static List<UserModel> _users = new List<UserModel>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserCollection()
        {
            UserDAO dataAccess = new UserDAO();
            _users = dataAccess.GetAllUsers();
        }

        /// <summary>
        /// Method that adds a user to the database from the information on the RegisterViewModel
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int AddUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the username and password entered are correct
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>The user (assuming the credentials are correct</returns>
        public UserModel CheckCredentials(string username, string password)
        {
            UserModel user = new UserModel();

            UserDAO dataAccess = new UserDAO();
            // Takes data down to the data access layer
            user = dataAccess.CheckCredentials(username, password);

            // Sends data up to the presentation
            return user;
        }

        /// <summary>
        /// Deletes a user from the database
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void DeleteUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all of the users in the database
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<UserModel> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a specific user based on the desired ID number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUserById(int id)
        {
            return _users.Find(x => x.Id == id);
        }

        /// <summary>
        /// Updates the information of a user
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(UserModel user)
        {
            // Declare and initialize
            int userId = -1;
            // Find the matching user with GetUserById
            UserModel findUser = GetUserById(user.Id);
            // If the result isn't null, update the user
            if (findUser != null)
            {
                userId = _users.IndexOf(findUser);
                _users[userId] = user;
            }
        }

        /// <summary>
        /// Verifies that the submitted password is correct
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool VerifyPassword(string password, UserModel user)
        {
            if (user.PasswordHash == password) return true;
            return false;
        }
    }
}
