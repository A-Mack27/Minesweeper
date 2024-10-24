namespace CST_350_Minesweeper_Website.Models
{
    public class UserModel
    {
        // Class level properties
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// Sets the user's password
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password)
        {
            PasswordHash = password;
        }
    }
}
