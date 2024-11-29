using CST_350_Minesweeper_Website.Interfaces;
using CST_350_Minesweeper_Website.Models;
using System.Data.SqlClient;

#pragma warning disable CS0618 // Type or member is obsolete
namespace CST_350_Minesweeper_Website.Services.DataAccess
{
    public class UserDAO : IUserManager
    {
        // Define the connection string for MSSQL
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserProfile;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

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
                string query = "INSERT INTO UserProfile (Username, PasswordHash, FirstName, LastName, Sex, Age, Email)" +
                    "VALUES (@Username, @PasswordHash, @FirstName, @LastName, @Sex, @Age, @Email); " +
                    "SELECT SCOPE_IDENTITY();";

                // Create a SQL command object using the query and open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command to safely pass user input values, avoiding SQL injection
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Sex", user.Sex);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@Email", user.Email);

                    // Execute the query and retrieve the new inserted ID using ExecuteScalar
                    int result = Convert.ToInt32(command.ExecuteScalar());

                    return result;
                }

                throw new InvalidOperationException("Failed to retrieve the inserted ID.");
            }
        }

        /// <summary>
        /// Checks the entered credentials of a user and vaidates them
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>The user found, or a blank user if none was found</returns>
        public UserModel CheckCredentials(string username, string password)
        {
            // Connect to the database
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();
                // Define the SQL query to select user details from the UserProfile table where the username and password match
                string query = "SELECT * FROM UserProfile WHERE Username = @Username AND PasswordHash = @Password";
                // Create a SQL command object using the query and the open connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the username parameter to the command to prevent SQL injection attacks
                    command.Parameters.AddWithValue(@"username", username);
                    // Add the password parameter for the same reason
                    command.Parameters.AddWithValue(@"password", password);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Return the user if the username and password are correct
                        foreach (UserModel user in UserCollection._users)
                            if (user.Username == username && UserCollection.VerifyPassword(password, user))
                                return user;
                    }
                }
            }
            // Return a new blank user if it's not correct
            return new UserModel
            {
                Id = 0,
                Username = "",
                PasswordHash = "",
                FirstName = "",
                LastName = "",
                Age = 0,
                Sex = "",
                Email = "",
            };
        }
        
        /// <summary>
        /// Removes a user from the database
        /// </summary>
        /// <param name="user"></param>
        public void DeleteUser(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();

                // Define the SQL query to delete the user based on their ID
                string query = "DELETE FROM UserProfile WHERE Id = @Id;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the ID parameter to the command to prevent SQL injection
                    command.Parameters.AddWithValue("@Id", user.Id);

                    // Execute the command to delete the user
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Returns all the users from the database
        /// </summary>
        /// <returns></returns>
        public List<UserModel> GetAllUsers()
        {
            List<UserModel> users = new List<UserModel>();

            using (SqlConnection connection = new SqlConnection(conn))
            {
                // Open the connection to the database
                connection.Open();

                // Define the SQL query to select all users from the UserAccount table
                string query = "SELECT * FROM UserProfile;";

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
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                Sex = reader.GetString(reader.GetOrdinal("Sex")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                            };
                            // Add it to the list
                            users.Add(user);
                        }
                    }
                }
            }
            // Return the list
            return users;
        }

        /// <summary>
        /// Searches for a user based on the inputted ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The user found</returns>
        /// <exception cref="NotImplementedException"></exception>
        public UserModel GetUserById(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the information of a desired user
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateUser(UserModel user)
        {
            throw new NotImplementedException();
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
