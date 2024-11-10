using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CST_350_Minesweeper_Website.Models;

public class GameController : Controller
{
    // No concurrent users means that we can use static instead of an HTML getter and setter
    static BoardModel? board;
    private static bool gameStarted = false;
    public IActionResult Index()
    {
        return View("Index", board);
    }

    // This action handles the form submission from StartGame.cshtml.
    // It stores the board size and difficulty level in session variables.
    [HttpPost]
    public IActionResult Start(string boardSize, string difficulty)
    {
        // Convert the board size string to an int
        int boardSizeInt = 0;
        switch (boardSize)
        {
            case "small": boardSizeInt = 10; break;
            case "medium": boardSizeInt = 15; break;
            case "large": boardSizeInt = 20; break;
        }
        // Convert the board difficulty to an int
        int difficultyInt = 0;
        switch (difficulty)
        {
            case "easy": difficultyInt = 10; break;
            case "medium": difficultyInt = 15; break;
            case "hard": difficultyInt = 20; break;
        }
        // Create the board
        board = new BoardModel(boardSizeInt, difficultyInt);
        // Store the board in a session variable

        // If game is started, load the GameBoard view
        return RedirectToAction("Index");
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
    [HttpPost]
    public IActionResult RevealCell(string cellLocation)
    {
        // Get the location of the cell clicked on
		var parts = cellLocation.Split(',');
		int row = Convert.ToInt32(parts[0]);
		int col = Convert.ToInt32(parts[1]);

        // Generate the bombs if the game has started
		if (!gameStarted)
        {
            board.GenerateBombs(row, col);
            gameStarted = true;
        }

        // Get the cell object at the location
        CellModel cell = board.Grid[row, col];
        // Update the board
        board.UpdateBoard(row, col, false, false);
        // Display the board
		return RedirectToAction("Index");
	}
}
