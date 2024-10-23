namespace CST_350_Minesweeper_Website.Models
{
    // Class for our group view model
    public class GroupViewModel
    {
        public bool IsSelected { get; set; }
        public string GroupName { get; set; }
    }

    public class RegisterViewModel
    {
        // Properties for our entry screen
        public string Username { get; set; }
        public string Password { get; set; }
        public List<GroupViewModel> Groups { get; set; }

        public RegisterViewModel()
        {
            // Declare and initialize
            Username = "";
            Password = "";
            // Create the selection we want for checkboxes
            Groups = new List<GroupViewModel>
            {
                new GroupViewModel { GroupName = "Admin", IsSelected = false},
                new GroupViewModel { GroupName = "Users", IsSelected = false},
                new GroupViewModel { GroupName = "Students", IsSelected = false}
            };
        }
    }
}
