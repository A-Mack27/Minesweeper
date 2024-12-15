using CST_350_Minesweeper_Website.Models;
using CST_350_Minesweeper_Website.Services.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CST_350_Minesweeper_Website.Controllers
{
    public class SavedGameController : Controller
    {
        public List<SavedGameModel> savedGames = new List<SavedGameModel>();
        public IActionResult Index()
        {
            GetUserGames();
            return View(savedGames);
        }

        [HttpPost]
        public IActionResult SaveBoardToDatabase()
        {
            // Save the game to the database logic
            return Json(new { success = true });  // Send success message as JSON
        }

        public void GetUserGames()
        {
            UserModel user = JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("User"));
            GamesDAO gamesDAO = new();

            if (user != null)
            {
                savedGames = gamesDAO.GetAllGames(user.Id);
            }
            else
            {
                savedGames = new List<SavedGameModel>();
            }
        }
    }
}
