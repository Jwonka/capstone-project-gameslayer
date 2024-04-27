using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using VideoGameGrade.Classes;
using static VideoGameGrade.Pages.GameCollectionModel;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
            // Get the input from the search bar
            string searchGame = HttpContext.Request.Query["searchGame"];

            try
            {
                // Connection string
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM gametable";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Reads data from the database if data is null converts it to an empty string or zero if it is an integer
                                GamesInfo gamesInfo = new GamesInfo();
                                gamesInfo.gameId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                gamesInfo.gameTitle = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                gamesInfo.gamePublisher = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                gamesInfo.gameConsole = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                gamesInfo.gameCategory = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                                gamesInfo.gameRating = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                                gamesInfo.gameImage = reader.IsDBNull(6) ? null : reader.GetString(6);

                                
                                gamesList.Add(gamesInfo);
                            }
                            reader.Close();
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred: " + ex.ToString());
                Console.WriteLine("Exception: " + ex.ToString());
            }
            int number;

            for (int i = 0; i < gamesList.Count(); i++)
            {
                // Check if searchGame has a value
                if (!string.IsNullOrEmpty(searchGame)  && !string.IsNullOrWhiteSpace(searchGame))
                {
                    // Strip searchGame of whitespace from beginning and end
                    searchGame = searchGame.Trim();

                    // If it is an integer search by gameId
                    if (int.TryParse(searchGame, out number) && number.Equals(gamesList[i].gameId))
                    {
                        gamesList = gamesList.Where(Game => Game.gameId.Equals(number)).ToList();
                        return;
                    }

                    // No game found display message
                    if (!number.Equals(gamesList[i].gameId) && i == gamesList.Count() - 1)
                    {
                        gamesList.Clear();
                        ViewData["SearchMessage"] = "Our records do not match your request";
                        return;
                    }

                    // Search for title regardless of capitilization
                    if (searchGame.ToLower().Equals(gamesList[i].gameTitle.ToLower()))
                    {
                        gamesList = gamesList.Where(Game => Game.gameTitle.ToLower().Contains(searchGame.ToLower())).ToList();
                        return;
                    }

                    // No game found display message
                    if (!searchGame.ToLower().Equals(gamesList[i].gameTitle.ToLower()) && i == gamesList.Count() - 1)
                    {
                        gamesList.Clear();
                        ViewData["SearchMessage"] = "Our records do not match your request";
                        return;
                    }

                    // Search for publisher regardless of capitilization
                    if (searchGame.ToLower().Equals(gamesList[i].gamePublisher.ToLower()))
                    {
                        gamesList = gamesList.Where(Game => Game.gamePublisher.ToLower().Contains(searchGame.ToLower())).ToList();
                        return;
                    }

                    // No game found display message
                    if (!searchGame.ToLower().Equals(gamesList[i].gamePublisher.ToLower()) && i == gamesList.Count() - 1)
                    {
                        gamesList.Clear();
                        ViewData["SearchMessage"] = "Our records do not match your request";
                        return;
                    }

                    // Search for Console regardless of capitilization
                    if (searchGame.ToLower().Equals(gamesList[i].gameConsole.ToLower()))
                    {
                        gamesList = gamesList.Where(Game => Game.gameConsole.ToLower().Contains(searchGame.ToLower())).ToList();
                        return;
                    }

                    // No game found display message
                    if (!searchGame.ToLower().Equals(gamesList[i].gameConsole.ToLower()) && i == gamesList.Count() - 1)
                    {
                        gamesList.Clear();
                        ViewData["SearchMessage"] = "Our records do not match your request";
                        return;
                    }

                    // Search for Category regardless of capitilization
                    if (searchGame.ToLower().Equals(gamesList[i].gameCategory.ToLower()))
                    {
                        gamesList = gamesList.Where(Game => Game.gameCategory.ToLower().Contains(searchGame.ToLower())).ToList();
                        return;
                    }

                    // No game found display message
                    if (!searchGame.ToLower().Equals(gamesList[i].gameCategory.ToLower()) && i == gamesList.Count() - 1)
                    {
                        gamesList.Clear();
                        ViewData["SearchMessage"] = "Our records do not match your request";
                        return;
                    }
                }
                else
                {
                    return;
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
}
