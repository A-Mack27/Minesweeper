using System.ComponentModel.DataAnnotations;

namespace CST_350_Register_and_Login_App.Models
{
    public class LoginViewModel
    {
        // Class level properties
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}