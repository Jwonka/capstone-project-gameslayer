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
                                gamesInfo.gamePlatform = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                gamesInfo.gameCategory = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                                gamesInfo.gameRating = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                                gamesInfo.gameImage = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);

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

            // Get the input from the search bar
            string searchGame = HttpContext.Request.Query["searchGame"];

            // Create a new list to add games that match the search query
            List<GamesInfo> match = new List<GamesInfo>();

            // Check if searchGame has a value
            if (!string.IsNullOrWhiteSpace(searchGame))
            {
                foreach (var game in gamesList)
                {
                    // Optimize validation by trimming empty spaces, converting to lowercase and replacing hyphens with an empty space
                    searchGame = searchGame.Trim().ToLower().Replace("-", "");
                    string gTitle = game.gameTitle.Trim().ToLower().Replace("-", "");
                    string gPub = game.gamePublisher.Trim().ToLower().Replace(",", "");
                    string platform = game.gamePlatform.Trim().ToLower().Replace(",", "");
                    string gCat = game.gameCategory.Trim().ToLower().Replace(",", "");

                    // Optimize search by removing characters
                    searchGame = searchGame.Replace(",", "");
                    searchGame = searchGame.Replace("'", "");
                    searchGame = searchGame.Replace(":", "");
                    gTitle = gTitle.Replace(",", "");
                    gTitle = gTitle.Replace(":", "");
                    gTitle = gTitle.Replace("'", "");

                    // Check for matches by title
                    if (gTitle.Contains(searchGame) && !match.Contains(game))
                    {
                        match.Add(game);
                    }

                    // Check for matches by publisher
                    if (gPub.Contains(searchGame) && !match.Contains(game))
                    {
                        match.Add(game);
                    }

                    // Check for matching consoles
                    if (platform.Contains(searchGame) && !match.Contains(game))
                    {
                        match.Add(game);
                    }

                    // Check for matching category
                    if (gCat.Contains(searchGame) && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                }
                if (match.Count > 0)
                {
                    gamesList = match;
                    return;
                }
                else
                {
                    searchGame = AddModel.CapFirstLetter(searchGame);
                    gamesList.Clear();
                    ViewData["searchMessage"] = "Our records do not contain " + searchGame + ".";
                    return;
                }
            }
            else
            {
                return;
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
            public string gamePlatform { get; set; }
            public string gameCategory { get; set; }
            public int gameRating { get; set; }
            public string gameImage { get; set; }
        }
    }
}