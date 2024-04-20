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
                                gamesInfo.gameQuiz = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);

                                //TODO Display game image from database
                                if (!reader.IsDBNull(7))
                                {
                                    //gamesInfo.gameImage = (byte[])reader.GetValue(7);
                                    gamesInfo.gameImage = null;
                                }
                                else
                                {
                                    gamesInfo.gameImage = null;
                                }
                                gamesInfo.gameAnswer = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);

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
                    string gTitle = game.gameTitle.Trim().ToLower();
                    string gPub = game.gamePublisher.Trim().ToLower();
                    string console = game.gameConsole.Trim().ToLower();
                    string gCat = game.gameCategory.Trim().ToLower();

                    // Strip searchGame of whitespace and convert to lowercase for verification
                    searchGame = searchGame.Trim().ToLower();

                    // If it is an integer search by gameId
                    if (int.TryParse(searchGame, out number) && number.Equals(game.gameId))
                    {
                        match.Add(game);
                    }

                    // Check for matches by title  or  publisher
                    if (searchGame.Equals(gTitle) || searchGame.Equals(gPub))
                    {
                        match.Add(game);
                    }

                    // Check for matching consoles
                    if (searchGame.Contains("nintendo") && console.Contains("nintendo") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("playstation") && console.Contains("playstation") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("xbox") && console.Contains("xbox") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("pc") && console.Contains("pc") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("steamdeck") && console.Contains("steamdeck") && !match.Contains(game))
                    {
                        match.Add(game);
                    }

                    // Check for matching category
                    if (searchGame.Contains("action") && gCat.Contains("action") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("adventure") && gCat.Contains("adventure") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if ((searchGame.Contains("role playing game") && gCat.Contains("role playing game") && !match.Contains(game) || searchGame.Contains("rpg") && gCat.Contains("rpg")) && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("platformer") && gCat.Contains("platformer") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("platform") && gCat.Contains("platform") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("2d") && gCat.Contains("2d") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("3d") && gCat.Contains("3d") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("shooter") && gCat.Contains("shooter") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("strategy") && gCat.Contains("strategy") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("sports") && gCat.Contains("sports") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("puzzle") && gCat.Contains("puzzle") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("simulation") && gCat.Contains("simulation") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("horror") && gCat.Contains("horror") && !match.Contains(game))
                    {
                        match.Add(game);
                    }
                    else if (searchGame.Contains("racing") && gCat.Contains("racing") && !match.Contains(game))
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
                    ViewData["SearchMessage"] = "Our records to not match your request";
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
            public string gameQuiz { get; set; }
            public byte[] gameImage { get; set; }
            public string gameAnswer { get; set; }
        }
    }
}
