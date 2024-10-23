namespace CST_350_Minesweeper_Website.Models
{
    public class UserModel
    {
        // Class level properties
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public string Groups { get; set; }

        /// <summary>
        /// Sets the password
        /// </summary>
        /// <param name="password"></param>
        /// <summary>
        /// Sets the password and generates a salt
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password)
        {
            PasswordHash = password;
        }
    }
}
