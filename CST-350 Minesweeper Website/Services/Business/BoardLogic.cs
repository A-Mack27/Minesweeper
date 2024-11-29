using CST_350_Minesweeper_Website.Models;
using System.Drawing;

namespace CST_350_Minesweeper_Website.Services.Business
{
    public class BoardLogic
    {
        // -------------------------------------------- GENERATE BOMBS METHOD -------------------------------------------- //
        /// <summary>
        /// Generates the bombs at random spots on the board based on difficulty
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="startCol"></param>
        public int GenerateBombs(BoardModel board, int startRow, int startCol)
        {
            // Instantiate Random class
            Random rand = new Random();
            CellModel startCell = board.Grid[startRow, startCol];
            // Calculate the total number of bombs necessary
            int bombCount = decimal.ToInt32(Convert.ToDecimal(board.Size) * Convert.ToDecimal(board.Size) * (board.Difficulty / 100));
            board.InitialBombCount = bombCount;
            // Repeats until all the bombs are placed
            while (bombCount > 0)
            {
                // Generate random numbers for row and column
                int row = rand.Next(0, board.Size);
                int col = rand.Next(0, board.Size);
                CellModel randCell = board.Grid[row, col];

                // The second condition is here to prevent a loss right after the first cell is chosen
                if (randCell.IsLive != true && !randCell.Equals(startCell))
                {
                    randCell.IsLive = true;
                    bombCount--;
                }
            }
            // Calculate the number of live neighbors
            CalculateLiveNeighbors(board);
            // If the starting cell has any live neighbors, use recursion to try again
            if (startCell.LiveNeighbors > 0)
            {
                // Reset all the cells
                foreach (CellModel cell in board.Grid)
                {
                    cell.IsLive = false;
                    cell.LiveNeighbors = 0;
                }
                // Generate the board again
                GenerateBombs(board, startRow, startCol);
            }
            // When it makes a valid board, return the initial bomb count
            return board.InitialBombCount;
        }
        // ----------------------------------------- END OF GENERATE BOMBS METHOD ----------------------------------------- //

        // ---------------------------------------------- UPDATE BOARD METHOD --------------------------------------------- //
        /// <summary>
        /// Updates the board with the new reveal/flag conditions
        /// </summary>
        /// <param name="revealedRow"></param>
        /// <param name="revealedCol"></param>
        /// <param name="flagCell"></param>
        /// <returns></returns>
        public (bool, bool, int, int) UpdateBoard(BoardModel board, int revealedRow, int revealedCol, bool flagCell, bool quickSweep)
        {
            board.FlagCount = 0;
            int remainingCells = 0, bombCount = 0;
            bool multipleCellsUpdated = false;
            // Object placeholder variable
            CellModel selectedCell = board.Grid[revealedRow, revealedCol];
            if (quickSweep && selectedCell.IsRevealed && !flagCell) { QuickSweep(board, revealedRow, revealedCol); multipleCellsUpdated = true; }
            // If the cell isn't flagged, flag it (if it isn't revealed) and if it is, unflag it
            else if (flagCell == true && !selectedCell.IsRevealed) selectedCell.IsFlagged = !selectedCell.IsFlagged;
            // If the live neighbors is 0, start the recursion process
            else if (board.Grid[revealedRow, revealedCol].LiveNeighbors == 0) { FloodFill(board, revealedRow, revealedCol); multipleCellsUpdated = true; }
            // If the cell ins't flagged, reveal it
            else if (!selectedCell.IsFlagged) selectedCell.IsRevealed = true;

            // Scans each cell to get the count for flags, bombs, and remaining cells
            foreach (CellModel cell in board.Grid)
            {
                if (cell.IsRevealed == true && cell.IsLive == true) return (true, multipleCellsUpdated, -1, bombCount);
                if (cell.IsFlagged) board.FlagCount++;
                if (cell.IsLive) bombCount++;
                if (!cell.IsRevealed) remainingCells++;
            }
            // Returns different things based on the condition of the board
            if (remainingCells - bombCount == 0) return (true, multipleCellsUpdated, 0, 0);
            return (false, multipleCellsUpdated, remainingCells, bombCount - board.FlagCount);
        }
        // ------------------------------------------- END OF UPDATE BOARD METHOD ------------------------------------------ //

        // ----------------------------------------------- FLOOD FILL METHOD ----------------------------------------------- //
        /// <summary>
        /// Flood fill recursion method for empty cells
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void FloodFill(BoardModel board, int row, int col)
        {
            // If statements staggered to prevent an out of bounds error
            if (row < 0 || row > board.Size - 1 || col < 0 || col > board.Size - 1) return;
            else if (board.Grid[row, col].IsFlagged == true || board.Grid[row, col].IsRevealed) return;
            else if (board.Grid[row, col].LiveNeighbors > 0)
            {
                // Reveals cells on the edges that have live neighbors
                board.Grid[row, col].IsRevealed = true;
                return;
            }

            // Reveals the cell
            board.Grid[row, col].IsRevealed = true;
            // Recursive in all 8 directions
            FloodFill(board, row - 1, col);
            FloodFill(board, row - 1, col + 1);
            FloodFill(board, row, col + 1);
            FloodFill(board, row + 1, col + 1);
            FloodFill(board, row + 1, col);
            FloodFill(board, row + 1, col - 1);
            FloodFill(board, row, col - 1);
            FloodFill(board, row - 1, col - 1);
        }
        // ------------------------------------------- END OF FLOOD FILL METHOD -------------------------------------------- //

        // ---------------------------------------- CALCULATE LIVE NEIGHBORS METHOD ---------------------------------------- //
        /// <summary>
        /// Determines the number of live cells in a 3x3 area around each cell
        /// </summary>
        /// <param name="currentCell"></param>
        public void CalculateLiveNeighbors(BoardModel board)
        {
            // Nested loops to run for every cell in the grid
            for (var i = 0; i < board.Size; i++)
            {
                for (var j = 0; j < board.Size; j++)
                {
                    // Sets a variable equal to the current cell
                    var currentCell = board.Grid[i, j];
                    // Resets the count
                    int count = 0;
                    // If the cell is a bomb, skip this cell
                    if (currentCell.IsLive == true) continue;

                    // Nested loops that make the 3x3 grid around the cell
                    for (int r = -1; r <= 1; r++)
                    {
                        for (int c = -1; c <= 1; c++)
                        {
                            // Skips the current cell at (0,0 index)
                            if (r == 0 && c == 0) continue;

                            // Indexes of the row and column for the neighbor
                            int newRow = currentCell.RowNumber + r;
                            int newCol = currentCell.ColumnNumber + c;

                            // If the indexes are in bounds and the cell is live, increase the count
                            if (newRow >= 0 && newRow < board.Size && newCol >= 0 && newCol < board.Size && board.Grid[newRow, newCol].IsLive)
                            {
                                count++;
                            }
                        }
                    }
                    // Set the number of Neighbors for the sell to the count variable
                    currentCell.LiveNeighbors = count;
                }
            }
        }
        // ------------------------------------- END OF CALCULATE LIVE NEIGHBORS METHOD ------------------------------------- //

        // ----------------------------------------------- QUICKSWEEP METHOD ------------------------------------------------ //
        /// <summary>
        /// Method to quickly scan all the adjacent squares
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void QuickSweep(BoardModel board, int row, int col)
        {
            CellModel currentCell = board.Grid[row, col];

            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    // Skips the current cell at (0,0 index)
                    if (r == 0 && c == 0) continue;

                    int newRow = currentCell.RowNumber + r;
                    int newCol = currentCell.ColumnNumber + c;

                    // Reveal all the adjacent squares
                    if (newRow >= 0 && newRow < board.Size && newCol >= 0 && newCol < board.Size && !board.Grid[newRow, newCol].IsFlagged)
                    {
                        if (board.Grid[newRow, newCol].LiveNeighbors == 0) FloodFill(board, newRow, newCol);
                        board.Grid[newRow, newCol].IsRevealed = true;
                    }
                }
            }
        }
        // -------------------------------------------- END OF QUICKSWEEP METHOD --------------------------------------------- //

        // ---------------------------------------------- CALCULATE SCORE METHOD --------------------------------------------- //
        /// <summary>
        /// Calculate the score of the game
        /// </summary>
        /// <returns></returns>
        public int CalculateScore(TimeSpan elapsedTime, BoardModel board)
        {
            int baseScore = 10000;
            double sizeMulti = (double)board.Size / 10;
            double diffMulti = (double)board.Difficulty / 10;
            double score = (baseScore * sizeMulti * diffMulti) / (elapsedTime.TotalSeconds + 1);
            return (int)score;
        }
        // ------------------------------------------- END OF CALCULATE SCORE METHOD ----------------------------------------- // 

        // -------------------------------------------------- WIPE BOARD METHOD ---------------------------------------------- //
        /// <summary>
        /// Wipe the board by revealling all cells and unflagging flags
        /// </summary>
        /// <param name="board"></param>
        public void WipeBoard(BoardModel board)
        {
            foreach (CellModel cell in board.Grid)
            {
                if (cell.IsLive) cell.IsFlagged = false;
                else cell.IsRevealed = true;
            }
        }
        // ----------------------------------------------- END OF WIPE BOARD METHOD ------------------------------------------ //
    }
}