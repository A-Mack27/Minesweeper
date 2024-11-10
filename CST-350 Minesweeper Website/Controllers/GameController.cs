using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CST_350_Minesweeper_Website.Models;

public class GameController : Controller
{
    // No concurrent users means that we can use static instead of an HTML getter and setter
    static BoardModel? board;
    private static bool gameStarted;
    private static bool gameIsOver;
    private static int cellsLeft;
	private static int bombCount;

    // ------------------------------------------------- INDEX VIEW -------------------------------------------------- //
    /// <summary>
    /// Index view of the game controller
    /// </summary>
    /// <returns></returns>
	public IActionResult Index()
    {
        // Check if the user session is active
        if (HttpContext.Session.GetString("User") == null)
        {
            // If no session, redirect to login page
            return RedirectToAction("Index", "Login");
        }
        return View();
    }
    // ---------------------------------------------- END OF INDEX VIEW ---------------------------------------------- //

    // ------------------------------------------------- BOARD VIEW -------------------------------------------------- //
    /// <summary>
    /// Action to show the board
    /// </summary>
    /// <returns></returns>
	public IActionResult Board()
	{
		return View("Board", board);
	}
    // ---------------------------------------------- END OF BOARD VIEW ---------------------------------------------- //

    // ------------------------------------------------ RESUME ACTION ------------------------------------------------ //
    /// <summary>
    /// Action to handle the resume button
    /// </summary>
    /// <returns></returns>
    public IActionResult Resume()
    {
        if (!gameStarted)
        {
            return RedirectToAction("Index", "Theme");
        }
        return RedirectToAction("Board", "Game");
    }
    // --------------------------------------------- END OF RESUME ACTION -------------------------------------------- //

    // ------------------------------------------------- START ACTION ------------------------------------------------ //
    /// <summary>
    /// Action to start the game
    /// </summary>
    /// <param name="boardSize"></param>
    /// <param name="difficulty"></param>
    /// <returns></returns>
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
        gameStarted = false; gameIsOver = false;
        HttpContext.Session.SetString("GameStarted", "false");

        // If game is started, load the GameBoard view
        return RedirectToAction("Board");
    }
    // ---------------------------------------------- END OF START ACTION -------------------------------------------- //

    // Might move this to a GameController class in the future
    // Add this new action for StartGame
    public IActionResult Configure()
	{
		// Otherwise, return the StartGame view
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

    // ---------------------------------------------- REVEAL CELL ACTION --------------------------------------------- //
    /// <summary>
    /// Reveals a cell on the grid and updates the view
    /// </summary>
    /// <param name="cellLocation"></param>
    /// <returns></returns>
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
            HttpContext.Session.SetString("GameStarted", "true");
		}

        // Get the cell object at the location
        CellModel cell = board.Grid[row, col];
		// Update the board
		(gameIsOver, cellsLeft, bombCount) = board.UpdateBoard(row, col, false, false);
        if (gameIsOver)
        {
            // Create a bool to determine the game end state
            bool gameWon = cellsLeft == 0;
            if (gameWon)
            {
                // Send the user to the win screen if they won
                return RedirectToAction("Win");
            }
            // Send them to the lost screen if they lost
			return RedirectToAction("Loss");
		}
        // Continue by displaying the board if the game isn't over
		return RedirectToAction("Board");
	}
    // -------------------------------------------- END OF REVEAL CELL ACTION ------------------------------------------ //
}
