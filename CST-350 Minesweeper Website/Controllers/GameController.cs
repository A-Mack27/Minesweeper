using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CST_350_Minesweeper_Website.Models;
using CST_350_Minesweeper_Website.Services.Business;
using System.Linq.Expressions;

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
		return View(GetBoard());
	}
	// ---------------------------------------------- END OF BOARD VIEW ---------------------------------------------- //

	// ------------------------------------------------ RESUME ACTION ------------------------------------------------ //
	/// <summary>
	/// Action to handle the resume button
	/// </summary>
	/// <returns></returns>
	public IActionResult ResumeSaved()
	{
		return RedirectToAction("Index", "SavedGame");
	}
    // --------------------------------------------- END OF RESUME ACTION -------------------------------------------- //

    // ----------------------------------------------- INITIALIZE ACTION --------------------------------------------- //
    /// <summary>
    /// Action to start the game
    /// </summary>
    /// <param name="boardSize"></param>
    /// <param name="difficulty"></param>
    /// <param name="resumingSaved"></param>
    /// <returns></returns>
    [HttpPost]
    [HttpGet]
    public IActionResult Initialize(string boardSize, string difficulty, bool resumingSavedGame = false)
	{
		// Convert the board size string to an int or set it to 0 for a new board
		int boardSizeInt = !resumingSavedGame? 0 : Int32.Parse(boardSize);
		string cellSize = "";

		// If a new game is being started, all these numbers need to be run
		if (!resumingSavedGame)
		{
			// Get the board size
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

			// Create and save the new board
            BoardModel board = new BoardModel(boardSizeInt, difficultyInt);
            SaveSessionBoard(board, "Board");
        }
        
		// This is left out because the cell size needs to be determined regardless of if it's a new game or not
        switch (boardSizeInt)
		{
			case 10: cellSize = "60px"; break;
            case 15: cellSize = "45px"; break;
            case 20: cellSize = "35px"; break;
        }

        HttpContext.Session.Remove("GameWon");
        gameStarted = resumingSavedGame ? true : false; gameIsOver = false;					// Set the status of the game
		HttpContext.Session.SetString("GameStarted", resumingSavedGame? "true" : "false");  // Set the session start variable
		HttpContext.Session.SetString("CellSize", cellSize);                                // Set the session cell size
		HttpContext.Session.SetString("StartTime", DateTime.Now.ToString());                // Start time for the timer

		return RedirectToAction("Play");
	}
	// ------------------------------------------- END OF INITIALIZE ACTION ------------------------------------------ //

	// -------------------------------------------------- START ACTION ----------------------------------------------- //
	/// <summary>
	/// Action to restart the game and remove the board
	/// </summary>
	/// <returns></returns>
	public IActionResult Start()
	{
		HttpContext.Session.Remove("GameWon");
		HttpContext.Session.Remove("Board");
		HttpContext.Session.SetString("GameStarted", "false");
		return RedirectToAction("Index", "Theme");
	}
	// ----------------------------------------------- END OF START ACTION ------------------------------------------- //

	// -------------------------------------------------- CONFIGURE VIEW --------------------------------------------- //
	/// <summary>
	/// Action to send the user to the configure screen
	/// </summary>
	/// <returns></returns>
	public IActionResult Configure()
	{
		return View();
	}
	// --------------------------------------------- END OF CONFIGURE VIEW ------------------------------------------- //

	// --------------------------------------------------- WIN VIEW -------------------------------------------------- //
	/// <summary>
	/// Action to send the user to the win screen
	/// </summary>
	/// <returns></returns>
	public IActionResult Win()
	{
		// Get the time that it took for the user to complete the game
#pragma warning disable CS8604 // Possible null reference argument.
		TimeSpan elapsedTime = DateTime.Now - DateTime.Parse(HttpContext.Session.GetString("StartTime"));
#pragma warning restore CS8604 // Possible null reference argument.
		// Get the board
		BoardModel board = GetBoard();
		// Set the score
		board.Score = boardLogic.CalculateScore(elapsedTime, GetBoard());
		// Cleard the board
		boardLogic.WipeBoard(board);
		HttpContext.Session.SetString("GameWon", "true");
		// Remove the actual board from the session
		HttpContext.Session.Remove("Board");
		return View(board);
	}
	// ------------------------------------------------ END OF WIN VIEW ---------------------------------------------- //

	// --------------------------------------------------- LOSS VIEW ------------------------------------------------- //
	/// <summary>
	/// Action to send the user to the loss screen
	/// </summary>
	/// <returns></returns>
	public IActionResult Loss()
	{
		// Get and then clear the board
		BoardModel board = GetBoard();
		boardLogic.WipeBoard(board);
		// Flag the game as lost and remove the board from the session
		HttpContext.Session.SetString("GameWon", "false");
		HttpContext.Session.Remove("Board");
		return View(board);
	}
	// ----------------------------------------------- END OF LOSS VIEW ---------------------------------------------- //

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
			boardLogic.GenerateBombs(board, row, col);            // Generate the bombs
			gameStarted = true;                                   // Set the game start to true
			HttpContext.Session.SetString("GameStarted", "true"); // Update the session variable
		}

		CellModel cell = board.Grid[row, col];
		(gameIsOver, multipleCellsUpdated, cellsLeft, bombCount) = boardLogic.UpdateBoard(board, row, col, false, true);
		SaveSessionBoard(board, "Board");

		// If the game is over...
		if (gameIsOver)
		{
			HttpContext.Session.SetString("GameStarted", "false");                // Set the game start status to false
			return cellsLeft == 0 ? Content("/Game/Win") : Content("/Game/Loss"); // Send the user to the win or loss screen
		}
		if (multipleCellsUpdated) return PartialView("_BoardPartial", board);     // Return the whole board as a partial view
		return PartialView("_CellPartial", cell);                                 // Return the updated cell as partial view
	}
	// -------------------------------------------- END OF REVEAL CELL ACTION ------------------------------------------ //

	// ------------------------------------------------- FLAG CELL ACTION ---------------------------------------------- //
	/// <summary>
	/// Action to flag a cell
	/// </summary>
	/// <param name="cellLocation"></param>
	/// <returns></returns>
	public IActionResult FlagCell(string cellLocation)
	{
		// Extract the cell coordinates
		var parts = cellLocation.Split(',');
		int row = Convert.ToInt32(parts[0]);
		int col = Convert.ToInt32(parts[1]);

		// Get the board from the session and get the cell that was clicked
		BoardModel board = GetBoard();
		CellModel cell = board.Grid[row, col];

		// Toggle the flag's state
		boardLogic.UpdateBoard(board, row, col, true, false);
		// Save the board
		SaveSessionBoard(board, "Board");
		// Return the partial view
		return PartialView("_CellPartial", cell);
	}
	// -------------------------------------------- END OF FLAG CELL ACTION -------------------------------------------- //

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
	private void SaveSessionBoard(BoardModel board, string sessionVariable)
	{
		var boardJson = JsonConvert.SerializeObject(board);
		HttpContext.Session.SetString(sessionVariable, boardJson);
	}
	// --------------------------------------------- END OF SAVE BOARD METHOD ------------------------------------------- //
}
