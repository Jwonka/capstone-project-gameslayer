using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;

namespace VideoGameGrade.Pages
{
    public class TestPageModel : PageModel
    {
        public string ConnectionStatusMessage { get; set; }

        public void OnGet()
        {
            // Construct the connection string
            string connectionString = "Server=videogamegrade.mysql.database.azure.com;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                 try
                {
                    connection.Open();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        ConnectionStatusMessage = "Database connection successful!";

                    }
                    else
                    {
                        ConnectionStatusMessage = "Database connection failed!";
                    }
                }
                catch (MySqlException ex)
                {
                    // Handle specific exceptions (e.g., authentication failure, network error)
                    ConnectionStatusMessage = $"Database connection failed: {ex.Message}";
                }
                catch (Exception ex)
                {
                    // Handle generic exceptions
                    ConnectionStatusMessage = $"An error occurred: {ex.Message}";
                }
            }
        }
    }
}