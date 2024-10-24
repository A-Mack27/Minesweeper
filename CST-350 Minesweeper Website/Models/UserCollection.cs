
using CST_350_Minesweeper_Website.Services.DataAccess;

namespace CST_350_Minesweeper_Website.Models
{
    public class UserCollection : IUserManager
    {
        public static List<UserModel> _users;

        public UserCollection()
        {
            _users = new List<UserModel>();
        }

        public int AddUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public UserModel CheckCredentials(string username, string password)
        {
            UserModel user = new UserModel();

            UserDAO dataAccess = new UserDAO();
            // Takes data down to the data access layer
            user = dataAccess.CheckCredentials(username, password);

            // Sends data up to the presentation
            return user;
        }

        public void DeleteUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public List<UserModel> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public UserModel GetUserById(int id)
        {
            return _users.Find(x => x.Id == id);
        }

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
