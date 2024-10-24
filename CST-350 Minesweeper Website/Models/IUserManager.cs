namespace CST_350_Minesweeper_Website.Models
{
    /// <summary>
    /// IUserManager interface to serve as the template for the classes it's referenced in
    /// </summary>
    public interface IUserManager
    {
        public List<UserModel> GetAllUsers();
        public UserModel GetUserById(int id);
        public int AddUser(UserModel user);
        public void DeleteUser(UserModel user);
        public void UpdateUser(UserModel user);
        public UserModel CheckCredentials(string username, string password);
    }
}
