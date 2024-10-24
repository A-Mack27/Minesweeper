using System.ComponentModel.DataAnnotations;

namespace CST_350_Minesweeper_Website.Models
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