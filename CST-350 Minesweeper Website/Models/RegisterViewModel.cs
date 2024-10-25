using System.ComponentModel.DataAnnotations;

namespace CST_350_Minesweeper_Website.Models
{
    public class RegisterViewModel
    {
        // Class level properties
        public int Id { get; set; }
        public string Username { get; set; }
        [Display(Name = "Password")] // Changes the display name of the attribute
        public string PasswordHash { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public RegisterViewModel()
        {
            // Declare and initialize
            Username = "";
            PasswordHash = "";
            FirstName = "";
            LastName= "";
            Sex = "";
            Age = 0;
            Email = "";
        }
    }
}
