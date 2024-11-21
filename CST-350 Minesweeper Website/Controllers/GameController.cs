using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CST_350_Minesweeper_Website.Models;

public class GameController : Controller
{
    // This action handles the form submission from StartGame.cshtml.
    // It stores the board size and difficulty level in session variables.
    [HttpPost]
    public IActionResult Start(string boardSize, string difficulty)
    {
        // Store settings in session for later use
        HttpContext.Session.SetString("BoardSize", boardSize);
        HttpContext.Session.SetString("Difficulty", difficulty);

        // Redirect to the GameBoard page where the game will be displayed
        return RedirectToAction("GameBoard");
    }

    // This action is used to display the Minesweeper game board.
    // It checks if the user has selected a board size, otherwise redirects to StartGame.
    public IActionResult GameBoard()
    {
        // Check if the board size is set in the session to validate access
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("BoardSize")))
        {
            // If no game has been started, redirect to StartGame page
            return RedirectToAction("StartGame", "Home");
        }

        // If game is started, load the GameBoard view
        return View();
    }

    // This action displays the Win page and calculates the final score.
    public IActionResult Win()
    {
        // Calculate score based on game parameters
        int score = CalculateScore();
        ViewBag.Score = score; // Pass score to the view using ViewBag

        // Show Win page with the score
        return View();
    }

    // This action displays the Loss page when the player loses the game.
    public IActionResult Loss()
    {
        // Load the Loss view
        return View();
    }

    // This helper method calculates the score for the game.
    // Replace with actual score calculation logic based on game requirements.
    private int CalculateScore()
    {
        // Example score calculation (replace with real logic)
        return 100;
    }

    // This action handles revealing a cell on the game board.
    // It returns JSON data to update the game board dynamically.
    public JsonResult RevealCell(int row, int col)
    {
        // Retrieve game state and reveal cell content based on Minesweeper logic
        var content = "1";
        

        // Return content as JSON data for client-side processing
        return Json(new { content });
    }

    // ADD THIS: Updates a specific cell and returns the partial view for that cell
    public IActionResult UpdateCell(int row, int col)
    {
        // Simulate fetching the cell from your game's logic or state
        var cell = new CellModel(row, col)
        {
            IsRevealed = true, // Example: Mark the cell as revealed
            IsLive = false,    // Example: Assume it's not a mine
            LiveNeighbors = 2  // Example: Assume it has 2 neighboring mines
        };

        // Return the partial view with the cell model
        return PartialView("_CellPartial", cell);
    }

}
