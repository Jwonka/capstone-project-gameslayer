using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using VideoGameGrade.Classes;
using static VideoGameGrade.Pages.GameCollectionModel;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;

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

                                //TODO Display game image from database
                                if (!reader.IsDBNull(6))
                                {
                                    //gamesInfo.gameImage = (byte[])reader.GetValue(7);
                                    gamesInfo.gameImage = null;
                                }
                                else
                                {
                                    gamesInfo.gameImage = null;
                                }
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
            List<GamesInfo> match = new List<GamesInfo>();

            // Check if searchGame has a value
            if (!string.IsNullOrEmpty(searchGame) && !string.IsNullOrWhiteSpace(searchGame))
            {
                foreach (var game in gamesList)
                {
                    // Optimize validation by trimming empty spaces, converting to lowercase and replacing hyphens with an empty space
                    searchGame = searchGame.Trim().ToLower().Replace("-", " ");
                    string gTitle = game.gameTitle.Trim().ToLower().Replace("-", " ");
                    string gPub = game.gamePublisher.Trim().ToLower().Replace("-", " ");
                    string console = game.gameConsole.Trim().ToLower().Replace("-", " ");
                    string gCat = game.gameCategory.Trim().ToLower().Replace("-", " ");

                    // Optimize search by replacing commas with an empty space
                    searchGame = searchGame.Replace(",", " ");
                    gTitle = gTitle.Replace(",", " ");
                    gCat = gCat.Replace(",", " ");
                    gPub = gPub.Replace(",", " ");
                    console = console.Replace(",", " ");

                    // Boolean that allows searching by gameId and not include titles or categories with numbers
                    Boolean digit = int.TryParse(searchGame, out number);

                    // If it is an integer search by gameId
                    if (number.Equals(game.gameId))
                    {
                        match.Add(game);
                    }

                    // Check for matches by title
                    if (gTitle.Contains(searchGame) && !match.Contains(game) && !digit)
                    {
                        match.Add(game);
                    }

                    // Check for matches by publisher
                    if (gPub.Contains(searchGame) && !match.Contains(game) && !digit)
                    {
                        match.Add(game);
                    }

                    // Check for matching consoles
                    if (console.Contains(searchGame) && !match.Contains(game) && !digit)
                    {
                        match.Add(game);
                    }

                    // Check for matching category
                    if (gCat.Contains(searchGame) && !match.Contains(game) && !digit)
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
                    gamesList.Clear();
                    ViewData["searchMessage"] = "Our records to not match your request";
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
            public string gameConsole { get; set; }
            public string gameCategory { get; set; }
            public int gameRating { get; set; }
            public byte[] gameImage { get; set; }
        }
    }
}
