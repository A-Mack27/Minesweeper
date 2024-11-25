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
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("User") == null)
        {
            return RedirectToAction("Index", "Login");
        }
        return View();
    }
    // ---------------------------------------------- END OF INDEX VIEW ---------------------------------------------- //

    // ------------------------------------------------- BOARD VIEW -------------------------------------------------- //
    public IActionResult Board()
    {
        return View("Board", GetBoard());
    }
    // ---------------------------------------------- END OF BOARD VIEW ---------------------------------------------- //

    // ------------------------------------------------ RESUME ACTION ------------------------------------------------ //
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
    [HttpPost]
    public IActionResult Initialize(string boardSize, string difficulty)
    {
        int boardSizeInt = 0;
        switch (boardSize)
        {
            case "small": boardSizeInt = 10; break;
            case "medium": boardSizeInt = 15; break;
            case "large": boardSizeInt = 20; break;
        }

        int difficultyInt = 0;
        switch (difficulty)
        {
            case "easy": difficultyInt = 10; break;
            case "medium": difficultyInt = 15; break;
            case "hard": difficultyInt = 20; break;
        }

        Board board = new Board(boardSizeInt, difficultyInt);
        gameStarted = false; gameIsOver = false;
        HttpContext.Session.SetString("GameStarted", "false");
        HttpContext.Session.SetString("StartTime", DateTime.Now.ToString());
        SaveBoard(board);

        return RedirectToAction("Board");
    }
    // ---------------------------------------------- END OF START ACTION -------------------------------------------- //

    // ------------------------------------------------- RESTART ACTION ---------------------------------------------- //
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

        Board board = GetBoard();

        if (!gameStarted)
        {
            board.GenerateBombs(row, col);
            gameStarted = true;
            HttpContext.Session.SetString("GameStarted", "true");
        }

        Cell cell = board.Grid[row, col];
        if (!cell.IsRevealed && !cell.IsFlagged)
        {
            cell.IsRevealed = true; // Reveal the cell
        }

        (gameIsOver, cellsLeft, bombCount) = board.UpdateBoard(row, col, false, false);
        SaveBoard(board);

        if (gameIsOver)
        {
            HttpContext.Session.SetString("GameStarted", "false");
            return cellsLeft == 0 ? RedirectToAction("Win") : RedirectToAction("Loss");
        }

        return PartialView("_CellPartial", cell); // Return the updated partial view
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
        Cell cell = board.Grid[row, col];

        // Ensure cell is updated correctly
        if (!cell.IsFlagged && !cell.IsRevealed)
        {
            cell.IsRevealed = true;
        }

        SaveBoard(board);
        return PartialView("_CellPartial", cell);
    }

    // -------------------------------------------- END OF UPDATE CELL AJAX HANDLER ------------------------------------ //
}
