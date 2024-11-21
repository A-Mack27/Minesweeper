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
            // If the user isn't logged in, redirect them to the login page
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
		return View("Board", GetBoard());
	}
    // ---------------------------------------------- END OF BOARD VIEW ---------------------------------------------- //

    // ------------------------------------------------ RESUME ACTION ------------------------------------------------ //
    /// <summary>
    /// Action to handle the resume button
    /// </summary>
    /// <returns></returns>
    public IActionResult Resume()
    {
        if (HttpContext.Session.GetString("Board") != null)
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
    public IActionResult Initialize(string boardSize, string difficulty)
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
        Board board = new Board(boardSizeInt, difficultyInt);
        gameStarted = false; gameIsOver = false;
        HttpContext.Session.SetString("GameStarted", "false");
        SaveBoard(board);
        // If game is started, load the GameBoard view
        return RedirectToAction("Board");
    }
    // ---------------------------------------------- END OF START ACTION -------------------------------------------- //

    // ------------------------------------------------- RESTART ACTION ---------------------------------------------- //
    /// <summary>
    /// Action to restart the game and remove the board
    /// </summary>
    /// <returns></returns>
    public IActionResult Start()
    {
        HttpContext.Session.Remove("Board");
        HttpContext.Session.SetString("GameStarted", "false");
        return RedirectToAction("Index", "Theme");
    }
    // ---------------------------------------------- END OF RESTART ACTION ------------------------------------------ //

    public IActionResult Configure()
	{
		return View();
	}

    // --------------------------------------------------- WIN VIEW -------------------------------------------------- //
    public IActionResult Win()
    {
        // Calculate score based on game parameters
        int score = CalculateScore();
        // Remove the board from the session
        HttpContext.Session.Remove("Board"); 
        // Show Win page with the score
        return View(score);
    }
    // ------------------------------------------------ END OF WIN VIEW ---------------------------------------------- //


    // This action displays the Loss page when the player loses the game.
    public IActionResult Loss()
    {
        // Load the Loss view
        return View();
    }

    // ------------------------------------------------ END OF WIN VIEW ---------------------------------------------- //

    // -------------------------------------------- CALCULATE SCORE METHOD ------------------------------------------- //
    /// <summary>
    /// Calculates the score after a game is won
    /// </summary>
    /// <returns></returns>
    private int CalculateScore()
    {
        // Aquire all the relevant variables
        Board board = GetBoard();
        TimeSpan elapsedTime = DateTime.Now - DateTime.Parse(HttpContext.Session.GetString("StartTime"));
        int baseScore = 10000;
        double sizeMulti = (double)board.Size / 10;
        double diffMulti = (double)board.Difficulty / 10;
        // Score formula
        double score = (baseScore * sizeMulti * diffMulti) / (elapsedTime.TotalSeconds + 1); // +1 so we don't divide by 0
        return (int)score; // Cast as an int to get a whole number
    }
    // ----------------------------------------- END OF CALCULATE SCORE METHOD --------------------------------------- //

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

        Board board = GetBoard();

        // Generate the bombs if the game hasn't been started
		if (!gameStarted)
        {
            board.GenerateBombs(row, col); // Generate the bombs based on the start location
            gameStarted = true; // Set the local game started variable to true
            HttpContext.Session.SetString("GameStarted", "true"); // set the session start variable to true
            HttpContext.Session.SetString("StartTime", DateTime.Now.ToString()); // Save the start time in a string
        }

        // Get the cell object at the location
        Cell cell = board.Grid[row, col];
		// Update the board
		(gameIsOver, cellsLeft, bombCount) = board.UpdateBoard(row, col, false, false);
        // Save the board state
        SaveBoard(board);

        if (gameIsOver)
        {
            HttpContext.Session.SetString("GameStarted", "false");
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

    // ------------------------------------------------ GET BOARD METHOD ----------------------------------------------- //
    /// <summary>
    /// Retrieve the board from the session variable
    /// </summary>
    /// <returns></returns>
    private Board GetBoard()
    {
        // Retrieve the serialized board string from the session
        var boardJson = HttpContext.Session.GetString("Board");

        if (string.IsNullOrEmpty(boardJson))
            return null; // If no board is found, return null

        // Deserialize the JSON string to BoardModel and return it
        var board = JsonConvert.DeserializeObject<Board>(boardJson);
        return board;
    }
    // --------------------------------------------- END OF GET BOARD METHOD -------------------------------------------- //

    // ------------------------------------------------ SAVE BOARD METHOD ----------------------------------------------- //
    /// <summary>
    /// Save the board to the session variable
    /// </summary>
    /// <param name="board"></param>
    private void SaveBoard(Board board)
    {
        // Serialize the BoardModel to a JSON string
        var boardJson = JsonConvert.SerializeObject(board);

        // Store the serialized string in the session
        HttpContext.Session.SetString("Board", boardJson);
    }
    // --------------------------------------------- END OF SAVE BOARD METHOD ------------------------------------------- //
}
