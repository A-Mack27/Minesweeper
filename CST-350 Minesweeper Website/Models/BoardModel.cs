namespace CST_350_Minesweeper_Website.Models
{
    public class BoardModel
    {
        // Square board
        public int Size { get; set; }
        // Array of cell objects
        public CellModel[,] Grid { get; set; }
        public decimal Difficulty { get; set; }
        public int InitialBombCount { get; set; }
        public int FlagCount { get; set; }
        public int Score {  get; set; }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="s"></param>
        /// <param name="d"></param>
        public BoardModel(int s, int d)
        {
            Difficulty = d;
            Size = s;
            Grid = new CellModel[Size, Size];
            // Fill the grid with Cell objects
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Grid[i, j] = new CellModel(i, j);
                }
            }
        }
    }
}
