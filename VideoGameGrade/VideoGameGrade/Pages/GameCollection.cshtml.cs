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

        protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>();
        }

        public void OnGet()
        {   
            try
            {
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

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
                                gamesInfo.gameTitle = "" + reader.GetString(1);
                                gamesInfo.gameCompany = "" + reader.GetString(2);
                                gamesInfo.gamePublisher = "" + reader.GetString(3);
                                gamesInfo.gameDesc = "" + reader.GetString(4);
                                gamesInfo.gameRating = reader.GetInt32(5);
                                gamesInfo.gameQuiz = "" + reader.GetString(6);
                                gamesInfo.gameImage = "" + reader.GetString(7);
                                gamesInfo.gameAnswer = "" + reader.GetString(8);

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

        public class GamesInfo
        {
            [Key]
            public int gameId { get; set; }

            [Required]
            [DisplayName("Game Title")]
            public string gameTitle { get; set; }

            public string gameCompany { get; set; }
            public string gamePublisher { get; set; }
            public string gameDesc { get; set; }
            public int gameRating { get; set; }
            public string gameQuiz { get; set; }
            public string gameImage { get; set; }
            public string gameAnswer { get; set; }
        }
    }
}
