using CST_350_Minesweeper_Website.Interfaces;
using CST_350_Minesweeper_Website.Models;
using System.Data.SqlClient;

#pragma warning disable CS0618 // Type or member is obsolete
namespace CST_350_Minesweeper_Website.Services.DataAccess
{
    public class GamesDAO
    {
        // Define the connection string for MSSQL
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserProfile;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        /// <summary>
        /// Adds a new saved game
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public int AddGame(SavedGameModel game)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                string query = "INSERT INTO dbo.Games (UserId, DateSaved, SaveState) " +
                               "VALUES (@UserId, @DateSaved, @SaveState); " +
                               "SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", game.UserId);
                    command.Parameters.AddWithValue("@DateSaved", game.DateSaved);
                    command.Parameters.AddWithValue("@SaveState", game.SaveState);

                    int result = Convert.ToInt32(command.ExecuteScalar());
                    return result;
                }
            }
        }

        /// <summary>
        /// Retrieves all saved games of a specific user from the database
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SavedGameModel> GetAllGames(int userId)
        {
            List<SavedGameModel> games = new List<SavedGameModel>();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                string query = "SELECT * FROM dbo.Games WHERE UserId = @UserId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SavedGameModel game = new();
                            game.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            game.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            game.DateSaved = reader.GetDateTime(reader.GetOrdinal("DateSaved"));
                            game.SaveState = reader.GetString(reader.GetOrdinal("SaveState"));
                            
                            games.Add(game);
                        }
                    }
                }
            }
            return games;
        }

        /// <summary>
        /// Retrieves a saved game by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SavedGameModel GetGameById(int id)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                string query = "SELECT * FROM dbo.Games WHERE Id = @Id;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SavedGameModel game = new();
                            game.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            game.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            game.DateSaved = reader.GetDateTime(reader.GetOrdinal("DateSaved"));
                            game.SaveState = reader.GetString(reader.GetOrdinal("SaveState"));

                            return game;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes a saved game by its ID
        /// </summary>
        /// <param name="id"></param>
        public void DeleteGame(int id)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                connection.Open();
                string query = "DELETE FROM dbo.Games WHERE Id = @Id;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
