namespace CST_350_Minesweeper_Website.Services.Business
{
    public class Cell
    {
        // Getters and setters for attributes
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public int LiveNeighbors { get; set; }
        public bool IsLive { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }

        // Constructor
        public Cell(int r, int c)
        {
            RowNumber = r;
            ColumnNumber = c;
            IsLive = false;
            LiveNeighbors = 0;
            IsRevealed = false;
            IsFlagged = false;
        }
    }
}
