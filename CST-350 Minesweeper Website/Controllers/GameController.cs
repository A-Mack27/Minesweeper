using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CST_350_Minesweeper_Website.Services.Business;

public class GameController : Controller
{
    private static bool gameStarted;
    private static bool gameIsOver;
    private static int cellsLeft;
    private static int bombCount;

    // -------------------------- START: INDEX VIEW --------------------------
    // Displays the main page for the game
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("User") == null)
        {
            return RedirectToAction("Index", "Login"); // Redirect if not logged in
        }
        return View();
    }
    // -------------------------- END: INDEX VIEW --------------------------

    // -------------------------- START: BOARD VIEW --------------------------
    // Displays the Minesweeper game board
    public IActionResult Board()
    {
        return View("Board", GetBoard());
    }
    // -------------------------- END: BOARD VIEW --------------------------

    // -------------------------- START: RESUME ACTION --------------------------
    // Resumes the game if a saved board exists
    public IActionResult Resume()
    {
        if (HttpContext.Session.GetString("Board") != null)
        {
            return RedirectToAction("Index", "Theme");
        }
        return RedirectToAction("Board", "Game");
    }
    // -------------------------- END: RESUME ACTION --------------------------

    // -------------------------- START: INITIALIZE GAME --------------------------
    // Initializes the game board with the selected size and difficulty
    [HttpPost]
    public IActionResult Initialize(string boardSize, string difficulty)
    {
        int boardSizeInt = boardSize.ToLower() switch
        {
            "small" => 10,
            "medium" => 15,
            "large" => 20,
            _ => 10 // Default to small if no valid input
        };

        int difficultyInt = difficulty.ToLower() switch
        {
            "easy" => 10,
            "medium" => 15,
            "hard" => 20,
            _ => 10 // Default to easy if no valid input
        };

        // Create a new board
        Board board = new Board(boardSizeInt, difficultyInt);
        gameStarted = false;
        gameIsOver = false;
        HttpContext.Session.SetString("GameStarted", "false");
        HttpContext.Session.SetString("StartTime", DateTime.Now.ToString());
        SaveBoard(board);

        return RedirectToAction("Board");
    }
    // -------------------------- END: INITIALIZE GAME --------------------------

    // -------------------------- START: START GAME --------------------------
    // Resets and restarts the game
    public IActionResult Start()
    {
        HttpContext.Session.Remove("Board"); // Remove the existing board
        HttpContext.Session.SetString("GameStarted", "false");
        return RedirectToAction("Index", "Theme");
    }
    // -------------------------- END: START GAME --------------------------

    // -------------------------- START: CONFIGURE GAME --------------------------
    // Placeholder for configuring the game settings
    public IActionResult Configure()
    {
        return View();
    }
    // -------------------------- END: CONFIGURE GAME --------------------------

    // -------------------------- START: WIN VIEW --------------------------
    // Displays the win message with the player's score
    public IActionResult Win()
    {
        int score = CalculateScore(); // Calculate the final score
        HttpContext.Session.Remove("Board"); // Clear the board from session
        return View(score);
    }
    // -------------------------- END: WIN VIEW --------------------------

    // -------------------------- START: LOSS VIEW --------------------------
    // Displays the game over message
    public IActionResult Loss()
    {
        return View();
    }
    // -------------------------- END: LOSS VIEW --------------------------

    // -------------------------- START: CALCULATE SCORE --------------------------
    // Calculates the player's score based on game settings and time elapsed
    private int CalculateScore()
    {
        Board board = GetBoard();
        TimeSpan elapsedTime = DateTime.Now - DateTime.Parse(HttpContext.Session.GetString("StartTime") ?? DateTime.MinValue.ToString());
        int baseScore = 10000;
        double sizeMulti = (double)board.Size / 10;
        double diffMulti = (double)board.Difficulty / 10;
        double score = (baseScore * sizeMulti * diffMulti) / (elapsedTime.TotalSeconds + 1);
        return (int)score;
    }
    // -------------------------- END: CALCULATE SCORE --------------------------

    // -------------------------- START: REVEAL CELL ACTION --------------------------
    // Reveals a cell when clicked
    [HttpPost]
    public IActionResult RevealCell(string cellLocation)
    {
        var parts = cellLocation.Split(',');
        int row = Convert.ToInt32(parts[0]);
        int col = Convert.ToInt32(parts[1]);

        Board board = GetBoard();

        if (!gameStarted)
        {
            board.GenerateBombs(row, col); // Generate bombs after the first click
            gameStarted = true;
            HttpContext.Session.SetString("GameStarted", "true");
        }

        Cell cell = board.Grid[row, col];
        if (!cell.IsRevealed && !cell.IsFlagged)
        {
            cell.IsRevealed = true; // Reveal the cell

            if (cell.IsLive) // If it's a bomb
            {
                gameIsOver = true;
            }
            else if (cell.LiveNeighbors == 0) // Empty cell, perform flood fill
            {
                board.FloodFill(row, col);
            }
        }

        SaveBoard(board);

        if (gameIsOver)
        {
            HttpContext.Session.SetString("GameStarted", "false");
            return cell.IsLive ? RedirectToAction("Loss") : RedirectToAction("Win");
        }

        return PartialView("_CellPartial", cell); // Return updated cell view
    }
    // -------------------------- END: REVEAL CELL ACTION --------------------------

    // -------------------------- START: TOGGLE FLAG ACTION --------------------------
    // Toggles a flag on a cell (right-click)
    [HttpPost]
    public IActionResult ToggleFlag(int row, int col)
    {
        Board board = GetBoard();
        Cell cell = board.Grid[row, col];

        if (!cell.IsRevealed)
        {
            cell.IsFlagged = !cell.IsFlagged; // Toggle the flag
        }

        SaveBoard(board);
        return PartialView("_CellPartial", cell);
    }
    // -------------------------- END: TOGGLE FLAG ACTION --------------------------

    // -------------------------- START: GET BOARD METHOD --------------------------
    // Retrieves the board from session
    private Board GetBoard()
    {
        var boardJson = HttpContext.Session.GetString("Board");
        return string.IsNullOrEmpty(boardJson) ? new Board(10, 10) : JsonConvert.DeserializeObject<Board>(boardJson);
    }
    // -------------------------- END: GET BOARD METHOD --------------------------

    // -------------------------- START: SAVE BOARD METHOD --------------------------
    // Saves the board to session
    private void SaveBoard(Board board)
    {
        var boardJson = JsonConvert.SerializeObject(board);
        HttpContext.Session.SetString("Board", boardJson);
    }
    // -------------------------- END: SAVE BOARD METHOD --------------------------

    // -------------------------- START: UPDATE CELL AJAX HANDLER --------------------------
    // Handles AJAX updates for a cell
    public IActionResult UpdateCell(int row, int col)
    {
        Board board = GetBoard();
        Cell cell = board.Grid[row, col];

        if (!cell.IsFlagged && !cell.IsRevealed)
        {
            cell.IsRevealed = true; // Reveal the cell
        }

        SaveBoard(board);
        return PartialView("_CellPartial", cell);
    }
    // -------------------------- END: UPDATE CELL AJAX HANDLER --------------------------
}
