using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using VideoGameGrade.Classes;

namespace VideoGameGrade.Pages
{
    public class GameCollectionModel : PageModel
    {
        private readonly ILogger<GameCollectionModel> _logger;

        public List<GamesInfo> gamesList = new List<GamesInfo>();

        public GameCollectionModel(ILogger<GameCollectionModel> logger)
        {
            _logger = logger;
     
        }

        public void OnGet()
        {   
            try
            {
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

                using (MySqlConnection connection = new MySqlConnection(connectionString)) 
                { 
                    connection.Open();
                    String sql = "SELECT * FROM gametable";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) 
                            {
                                GamesInfo gamesInfo = new GamesInfo();
                                gamesInfo.gameId = reader.GetInt32(0);
                                gamesInfo.gameTitle = reader.IsDBNull(1) ? string.Empty: reader.GetString(1);
                                gamesInfo.gamePublisher = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                gamesInfo.gameConsole = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                gamesInfo.gameCategory = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                                gamesInfo.gameRating = reader.GetInt32(5);
                                gamesInfo.gameQuiz = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                                gamesInfo.gameImage = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                                gamesInfo.gameAnswer = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);

                                gamesList.Add(gamesInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
       
        }
    }

    public class GamesInfo
    {
        [Key]
        public int gameId { get; set; }

        [Required]
        [DisplayName("Game Title")]
        public string gameTitle { get; set; }

        public string gamePublisher { get; set; }
        public string gameConsole { get; set; }
        public string gameCategory { get; set; }
        public int gameRating { get; set; }
        public string gameQuiz { get; set; }
        public string gameImage { get; set; }
        public string gameAnswer { get; set; }
    }
}
