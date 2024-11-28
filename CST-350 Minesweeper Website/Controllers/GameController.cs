using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CST_350_Minesweeper_Website.Models;
using CST_350_Minesweeper_Website.Services.Business;

public class GameController : Controller
{
	private static BoardLogic boardLogic = new();
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
	public IActionResult Play()
	{
		// Retrieve the board and pass it to the Board view
		return View("Play", GetBoard());
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
		return RedirectToAction("Play", "Game");
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
		string cellSize = "";
		switch (boardSize)
		{
			case "small": boardSizeInt = 10; cellSize = "70px"; break;
			case "medium": boardSizeInt = 15; cellSize = "50px"; break;
			case "large": boardSizeInt = 20; cellSize = "40px"; break;
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
		BoardModel board = new BoardModel(boardSizeInt, difficultyInt);
		gameStarted = false; gameIsOver = false;
		HttpContext.Session.SetString("GameStarted", "false");
		HttpContext.Session.SetString("CellSize", cellSize);
		HttpContext.Session.SetString("StartTime", DateTime.Now.ToString()); // Start time for the timer
		SaveBoard(board);
		return RedirectToAction("Play");
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
	/// <summary>
	/// Action to send the user to the configure screen
	/// </summary>
	/// <returns></returns>
	public IActionResult Configure()
	{
		return View();
	}

	// --------------------------------------------------- WIN VIEW -------------------------------------------------- //
	/// <summary>
	/// Action to send the user to the win screen
	/// </summary>
	/// <returns></returns>
	public IActionResult Win()
	{
		int score = CalculateScore();
		HttpContext.Session.Remove("Board");
		return View(score);
	}
	// ------------------------------------------------ END OF WIN VIEW ---------------------------------------------- //
	/// <summary>
	/// Action to send the user to the loss screen
	/// </summary>
	/// <returns></returns>
	public IActionResult Loss()
	{
		return View();
	}

	// -------------------------------------------- CALCULATE SCORE METHOD ------------------------------------------- //
	/// <summary>
	/// Calculate the score of the game
	/// </summary>
	/// <returns></returns>
	private int CalculateScore()
	{
		BoardModel board = GetBoard();
		TimeSpan elapsedTime = DateTime.Now - DateTime.Parse(HttpContext.Session.GetString("StartTime"));
		int baseScore = 10000;
		double sizeMulti = (double)board.Size / 10;
		double diffMulti = (double)board.Difficulty / 10;
		double score = (baseScore * sizeMulti * diffMulti) / (elapsedTime.TotalSeconds + 1);
		return (int)score;
	}
	// ----------------------------------------- END OF CALCULATE SCORE METHOD --------------------------------------- //

	// ---------------------------------------------- REVEAL CELL ACTION --------------------------------------------- //
	/// <summary>
	/// Action to reveal a cell of a board, update it, and redirect/return the correct 
	/// </summary>
	/// <param name="cellLocation"></param>
	/// <returns></returns>
	[HttpPost]
    public IActionResult RevealCell(string cellLocation)
    {
		// Extract the cell coordinates
        var parts = cellLocation.Split(',');
        int row = Convert.ToInt32(parts[0]);
        int col = Convert.ToInt32(parts[1]);
		// Create a variable to store if more than one cell was updated
		bool multipleCellsUpdated;
		// Get the board from the session variable
        BoardModel board = GetBoard();

		// If the game hasn't started...
        if (!gameStarted)
        {
            boardLogic.GenerateBombs(board, row, col);						  // Generate the bombs
            gameStarted = true;									  // Set the game start to true
            HttpContext.Session.SetString("GameStarted", "true"); // Update the session variable
        }

        CellModel cell = board.Grid[row, col];
        (gameIsOver, multipleCellsUpdated, cellsLeft, bombCount) = boardLogic.UpdateBoard(board, row, col, false, false);
        SaveBoard(board);

		// If the game is over...
        if (gameIsOver)
        {
            HttpContext.Session.SetString("GameStarted", "false");				  // Set the game start status to false
            return cellsLeft == 0 ? Content("/Game/Win") : Content("/Game/Loss"); // Send the user to the win or loss screen
        }
		if (multipleCellsUpdated) return PartialView("_BoardPartial", board);	  // Return the whole board as a partial view
        return PartialView("_CellPartial", cell);								  // Return the updated cell as partial view
    }
	// -------------------------------------------- END OF REVEAL CELL ACTION ------------------------------------------ //

	// ------------------------------------------------ GET BOARD METHOD ----------------------------------------------- //
	/// <summary>
	/// Method to get the board from the session
	/// </summary>
	/// <returns></returns>
	private BoardModel GetBoard()
	{
		var boardJson = HttpContext.Session.GetString("Board");
		if (string.IsNullOrEmpty(boardJson)) return null;
		return JsonConvert.DeserializeObject<BoardModel>(boardJson);
	}
	// --------------------------------------------- END OF GET BOARD METHOD -------------------------------------------- //

	// ------------------------------------------------ SAVE BOARD METHOD ----------------------------------------------- //
	/// <summary>
	/// Method to save the board to the session variable
	/// </summary>
	/// <param name="board"></param>
	private void SaveBoard(BoardModel board)
	{
		var boardJson = JsonConvert.SerializeObject(board);
		HttpContext.Session.SetString("Board", boardJson);
	}
	// --------------------------------------------- END OF SAVE BOARD METHOD ------------------------------------------- //

	// --------------------------------------------- UPDATE CELL AJAX HANDLER ------------------------------------------- //
	public IActionResult UpdateCell(int row, int col)
	{
		BoardModel board = GetBoard();
		CellModel cell = board.Grid[row, col];
		cell.IsRevealed = true;
		SaveBoard(board);
		return PartialView("_CellPartial", cell);
	}
	// -------------------------------------------- END OF UPDATE CELL AJAX HANDLER ------------------------------------ //
}
