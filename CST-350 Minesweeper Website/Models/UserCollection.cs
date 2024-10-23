
namespace CST_350_Minesweeper_Website.Models
{
    public class UserCollection : IUserManager
    {
        public static List<UserModel> _users;

        public int AddUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public int CheckCredentials(string username, string password)
        {
            throw new NotImplementedException();
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
