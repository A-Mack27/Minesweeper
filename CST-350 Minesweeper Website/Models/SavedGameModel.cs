namespace CST_350_Minesweeper_Website.Models
{
    public class SavedGameModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DateSaved { get; set; }
        public string SaveState { get; set; }

        public SavedGameModel(int id, int userId, DateTime dateSaved, string saveState)
        {
            Id = id;
            UserId = userId;
            DateSaved = dateSaved;
            SaveState = saveState;
        }
    }
}
