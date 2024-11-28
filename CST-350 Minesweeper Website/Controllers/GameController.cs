using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CST_350_Minesweeper_Website.Services.Business;
using CST_350_Minesweeper_Website.Models;

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
		Board board = new Board(boardSizeInt, difficultyInt);
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

	public IActionResult Configure()
	{
		return View();
	}

	// --------------------------------------------------- WIN VIEW -------------------------------------------------- //
	public IActionResult Win()
	{
		int score = CalculateScore();
		HttpContext.Session.Remove("Board");
		return View(score);
	}
	// ------------------------------------------------ END OF WIN VIEW ---------------------------------------------- //

	public IActionResult Loss()
	{
		return View();
	}

	// -------------------------------------------- CALCULATE SCORE METHOD ------------------------------------------- //
	private int CalculateScore()
	{
		Board board = GetBoard();
		TimeSpan elapsedTime = DateTime.Now - DateTime.Parse(HttpContext.Session.GetString("StartTime"));
		int baseScore = 10000;
		double sizeMulti = (double)board.Size / 10;
		double diffMulti = (double)board.Difficulty / 10;
		double score = (baseScore * sizeMulti * diffMulti) / (elapsedTime.TotalSeconds + 1);
		return (int)score;
	}
	// ----------------------------------------- END OF CALCULATE SCORE METHOD --------------------------------------- //

	// ---------------------------------------------- REVEAL CELL ACTION --------------------------------------------- //
	[HttpPost]
    public IActionResult RevealCell(string cellLocation)
    {
        var parts = cellLocation.Split(',');
        int row = Convert.ToInt32(parts[0]);
        int col = Convert.ToInt32(parts[1]);

		bool floodFillActivated;

        Board board = GetBoard();

        if (!gameStarted)
        {
            board.GenerateBombs(row, col);
            gameStarted = true;
            HttpContext.Session.SetString("GameStarted", "true");
        }

        CellModel cell = board.Grid[row, col];
        (gameIsOver, floodFillActivated, cellsLeft, bombCount) = board.UpdateBoard(row, col, false, false);
        SaveBoard(board);

        if (gameIsOver)
        {
            HttpContext.Session.SetString("GameStarted", "false");
            return cellsLeft == 0 ? RedirectToAction("Win") : RedirectToAction("Loss");
        }
		if (floodFillActivated) return RedirectToAction("Play", GetBoard());
        return PartialView("_CellPartial", cell); // Return the updated cell as partial view
    }
// -------------------------------------------- END OF REVEAL CELL ACTION ------------------------------------------ //

// ------------------------------------------------ GET BOARD METHOD ----------------------------------------------- //
	private Board GetBoard()
	{
		var boardJson = HttpContext.Session.GetString("Board");
		if (string.IsNullOrEmpty(boardJson)) return null;
		return JsonConvert.DeserializeObject<Board>(boardJson);
	}
	// --------------------------------------------- END OF GET BOARD METHOD -------------------------------------------- //

	// ------------------------------------------------ SAVE BOARD METHOD ----------------------------------------------- //
	private void SaveBoard(Board board)
	{
		var boardJson = JsonConvert.SerializeObject(board);
		HttpContext.Session.SetString("Board", boardJson);
	}
	// --------------------------------------------- END OF SAVE BOARD METHOD ------------------------------------------- //

	// --------------------------------------------- UPDATE CELL AJAX HANDLER ------------------------------------------- //
	public IActionResult UpdateCell(int row, int col)
	{
		Board board = GetBoard();
		CellModel cell = board.Grid[row, col];
		cell.IsRevealed = true;
		SaveBoard(board);
		return PartialView("_CellPartial", cell);
	}
	// -------------------------------------------- END OF UPDATE CELL AJAX HANDLER ------------------------------------ //
}
