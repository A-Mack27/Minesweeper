using CST_350_Minesweeper_Website.Models;
using System.Data.SqlClient;

namespace CST_350_Minesweeper_Website.Services.DataAccess
{
    public class UserDAO : IUserManager
    {
        // Define the connection string for MSSQL
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserAuth;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // Define the connection string
        static string serverName = "localhost";
        static string username = "root";
        static string password = "root";
        static string dbName = "userauth";
        static string port = "8889";

        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int AddUser(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();
                // Define the SQL query with parameter placeholders to prevent SQL Injection attack
                string query = "INSERT INTO UserAccount (Username, Password, Group)" +
                    "VALUES (@Username, @Password, @Group); " +
                    "SELECT SCOPE_IDENTITY();";

                // Create a SQL command object using the query and open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command to safely pass user input values, avoiding SQL injection
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.PasswordHash);
                    command.Parameters.AddWithValue("@Group", user.Groups);

                    // Execute the query and retrieve the new inserted ID using ExecuteScalar
                    int result = Convert.ToInt32(command.ExecuteScalar());

                    return result;
                }

                // Your homework to complete this so the user knows to re-enter the information
                throw new InvalidOperationException("Failed to retrieve the inserted ID.");
            }
        }

        public int CheckCredentials(string username, string password)
        {
            string query = "";

            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();
                // Define the SQL query to select user details from the UserAccount table where the username and password match
                query = "SELECT * FROM UserAccount WHERE Username = @Username AND MyPassword = @password";
                // Create a SQL command object using the query and the open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the username parameter to the command to prevent SQL injection attacks
                    command.Parameters.AddWithValue(@"username", username);
                    // Add the password parameter for the same reason
                    command.Parameters.AddWithValue(@"password", password);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                    }
                }
            }

            // Given a username and password, find a matching user
            // Return the user's Id
            // Iterate over the UserModel
            // Instantiate the UserDAO
            UserDAO dataAccess = new UserDAO();
            foreach (UserModel user in UserCollection._users)
            {
                // Get the id
                if (user.Username == username && UserCollection.VerifyPassword(password, user))
                {
                    return (dataAccess.CheckCredentials(username, password));
                }
            }
            // No matches found. Invalid Login
            return -1;
        } // End CheckCredentials

        public void DeleteUser(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();

                // Define the SQL query to delete the user based on their ID
                string query = "DELETE FROM UserAccount WHERE Id = @Id;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the ID parameter to the command to prevent SQL injection
                    command.Parameters.AddWithValue("@Id", user.Id);

                    // Execute the command to delete the user
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<UserModel> GetAllUsers()
        {
            List<UserModel> users = new List<UserModel>();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();

                // Define the SQL query to select all users from the UserAccount table
                string query = "SELECT * FROM UserAccount;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Read each record and create a UserModel for each user
                        while (reader.Read())
                        {
                            UserModel user = new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("MyPassword")),
                                Groups = reader.GetString(reader.GetOrdinal("GroupName"))
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }


        public UserModel GetUserById(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(UserModel user)
        {
            throw new NotImplementedException();
        }
    }
}
