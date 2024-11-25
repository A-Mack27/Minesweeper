namespace CST_350_Minesweeper_Website.Services.Business
{
    public class Board
    {
        public int Size { get; set; }
        public Cell[,] Grid { get; set; }
        public decimal Difficulty { get; set; }
        public int InitialBombCount { get; set; }

        public Board(int s, int d)
        {
            Difficulty = d;
            Size = s;
            Grid = new Cell[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Grid[i, j] = new Cell(i, j);
                }
            }
        }

        public int GenerateBombs(int startRow, int startCol)
        {
            Random rand = new Random();
            Cell startCell = Grid[startRow, startCol];
            int bombCount = decimal.ToInt32(Size * Size * (Difficulty / 100));
            InitialBombCount = bombCount;

            while (bombCount > 0)
            {
                int row = rand.Next(0, Size);
                int col = rand.Next(0, Size);
                Cell randCell = Grid[row, col];

                if (!randCell.IsLive && !randCell.Equals(startCell))
                {
                    randCell.IsLive = true;
                    bombCount--;
                }
            }
            CalculateLiveNeighbors();

            if (startCell.LiveNeighbors > 0)
            {
                foreach (Cell cell in Grid)
                {
                    cell.IsLive = false;
                    cell.LiveNeighbors = 0;
                }
                GenerateBombs(startRow, startCol);
            }
            return InitialBombCount;
        }

        public void CalculateLiveNeighbors()
        {
            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var currentCell = Grid[i, j];
                    int count = 0;

                    if (currentCell.IsLive) continue;

                    for (int r = -1; r <= 1; r++)
                    {
                        for (int c = -1; c <= 1; c++)
                        {
                            if (r == 0 && c == 0) continue;

                            int newRow = currentCell.RowNumber + r;
                            int newCol = currentCell.ColumnNumber + c;

                            if (newRow >= 0 && newRow < Size && newCol >= 0 && newCol < Size && Grid[newRow, newCol].IsLive)
                            {
                                count++;
                            }
                        }
                    }
                    currentCell.LiveNeighbors = count;
                }
            }
        }

        public (bool, int, int) UpdateBoard(int revealedRow, int revealedCol, bool flagCell, bool quickSweep)
        {
            int remainingCells = 0, flagCount = 0, bombCount = 0;
            Cell selectedCell = Grid[revealedRow, revealedCol];

            if (!selectedCell.IsRevealed && !flagCell)
            {
                if (selectedCell.LiveNeighbors == 0)
                {
                    FloodFill(revealedRow, revealedCol);
                }
                else
                {
                    selectedCell.IsRevealed = true;
                }
            }

            foreach (Cell cell in Grid)
            {
                if (!cell.IsRevealed) remainingCells++;
                if (cell.IsLive && cell.IsRevealed) return (true, -1, bombCount);
            }

            return remainingCells == InitialBombCount ? (true, 0, bombCount) : (false, remainingCells, bombCount);
        }

        public void FloodFill(int row, int col)
        {
            if (row < 0 || row >= Size || col < 0 || col >= Size) return; // Prevent out-of-bounds errors
            if (Grid[row, col].IsFlagged || Grid[row, col].IsRevealed) return; // Skip flagged or already revealed cells
            if (Grid[row, col].LiveNeighbors > 0)
            {
                Grid[row, col].IsRevealed = true; // Reveal cells with live neighbors
                return;
            }

            // Reveal the cell
            Grid[row, col].IsRevealed = true;

            // Recursively reveal adjacent cells
            FloodFill(row - 1, col);
            FloodFill(row + 1, col);
            FloodFill(row, col - 1);
            FloodFill(row, col + 1);
            FloodFill(row - 1, col - 1);
            FloodFill(row - 1, col + 1);
            FloodFill(row + 1, col - 1);
            FloodFill(row + 1, col + 1);
        }

    }
}
