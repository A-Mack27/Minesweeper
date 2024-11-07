using CST_350_Minesweeper_Website.Models;
using Microsoft.AspNetCore.Mvc;

namespace CST_350_Minesweeper_Website.Controllers
{
	public class GameController : Controller
	{
		// Create the board (fixed size and difficulty for now)
		static BoardModel board = new BoardModel(10,25);

		public IActionResult Index()
		{
			return View("Index", board);
		}

		public IActionResult HandleButtonClick(string row, string colum)
		{
			return View();
		}
	}
}
