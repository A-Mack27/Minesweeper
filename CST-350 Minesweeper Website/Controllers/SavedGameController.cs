using CST_350_Minesweeper_Website.Models;
using CST_350_Minesweeper_Website.Services.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CST_350_Minesweeper_Website.Controllers
{
    public class SavedGameController : Controller
    {
        public List<SavedGameModel> savedGames = new List<SavedGameModel>();

        /// <summary>
        /// Index that will display the saved games of the user that's logged in
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            GetUserGames();
            return View(savedGames);
        }

        /// <summary>
        /// Uses the GamesDAO to save the game state to the database
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveBoardToDatabase()
        {
            // Get the user information
            UserModel user = JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("User"));
            GamesDAO gamesDAO = new();

            // Create the savedGame object to store the status of the game
            SavedGameModel savedGame = new();
            savedGame.UserId = user.Id;
            savedGame.DateSaved = DateTime.Now;
            savedGame.SaveState = HttpContext.Session.GetString("Board");

            // Add it to the database
            gamesDAO.AddGame(savedGame);

            // Send success message as JSON
            return Json(new { success = true });  
        }

        /// <summary>
        /// Resumes a saved game that a user has
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ResumeSavedGame(int id)
        {
            GamesDAO gamesDAO = new();
            // Get all the current saved games
            GetUserGames();
            // Get the board string
            string boardJson = gamesDAO.GetGameById(id).SaveState;
            // Get the board object from the save data
            var board = JsonConvert.DeserializeObject<BoardModel>(boardJson);
            HttpContext.Session.SetString("Board", boardJson);
            // Send the user to the board page
            return RedirectToAction("Initialize", "Game", new { boardSize = board.Size.ToString(), difficulty = "", resumingSavedGame = true });
        }

        /// <summary>
        /// Deletes a saved game from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteSavedGame(int id)
        {
            // Create a new instance of the DAO
            GamesDAO gamesDAO = new();
            // Delete the game from the database
            gamesDAO.DeleteGame(id);
            // Refresh the page
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gets all the games saved by the user that's logged in
        /// </summary>
        public void GetUserGames()
        {
            // Get the user
            UserModel user = JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("User"));
            GamesDAO gamesDAO = new();
            // Get all the games that the user has saved
            if (user != null) savedGames = gamesDAO.GetAllGames(user.Id);
            else savedGames = new List<SavedGameModel>();
        }
    }
}
