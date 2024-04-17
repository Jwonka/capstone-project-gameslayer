using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace VideoGameGrade.Pages
{
    public class GameCollectionModel : PageModel
    {
        private readonly ILogger<GameCollectionModel> _logger;

        public bool DatabaseConnected { get; set; }

        public GameCollectionModel(ILogger<GameCollectionModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
             
             // Construct the connection string
             string connectionString = "Server=videogamegrade.mysql.database.azure.com;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DatabaseConnected = connection.State == System.Data.ConnectionState.Open;
                }
                catch (Exception ex)
                { 
                    _logger.LogError(ex, "An error occurred in GameCollection.");
              
                    DatabaseConnected = false;
                }
            }
        }
    }
}
