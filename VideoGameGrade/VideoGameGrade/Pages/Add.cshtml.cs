using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Pqc.Crypto.Picnic;
using System.Drawing;
using static VideoGameGrade.Pages.GameCollectionModel;

namespace VideoGameGrade.Pages
{
    public class AddModel : PageModel
    {
        public GamesInfo gamesInfo = new GamesInfo();
        public string errorMessage = string.Empty;
        public static string successMessage = string.Empty;
        public static string gameName = string.Empty;
        public string gamePub = string.Empty;
        public string gameConsole = string.Empty;
        public string gameCategory = string.Empty;
        public string title = string.Empty;
        public static string gameImg = string.Empty;
        public static string insertImg = string.Empty;
        public static bool success = false;

        public static string CapFirstLetter(string lower)
        {
            if (!string.IsNullOrEmpty(lower) && !string.IsNullOrWhiteSpace(lower))
            {
                var words = lower.Split(' ');
                var letter = string.Empty;
                foreach (var word in words)
                {
                    try
                    {
                        letter += char.ToUpper(word[0]) + word.Substring(1) + ' ';
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception occurred: " + ex.ToString());
                    }  
                }
                return letter.Trim();
            }
            else
            {
                return lower;
            }
        }
        public void OnGet()
        {
        }
        public void OnPost()
        {
            gamesInfo.gameTitle = Request.Form["gameTitle"];
            gamesInfo.gamePublisher = Request.Form["gamePublisher"];
            gamesInfo.gameConsole = Request.Form["gameConsole"];
            gamesInfo.gameCategory = Request.Form["gameCategory"];
            gamesInfo.gameImage = Request.Form["gameImage"];
            // Explicitly convert gameRating to int
            if (int.TryParse(Request.Form["gameRating"], out int rating) && rating >= 0 && rating <= 1)
            {
                gamesInfo.gameRating = rating;
            }
            else if(rating < 0)
            {
                errorMessage = "Ratings can not be negative.";
                return;
            }
            else
            {
                errorMessage = "New games can only have a maximum rating of 1.";
                return;
            }
            //gamesInfo.gameImage = Request.Form["gameImage"];

            if (!string.IsNullOrWhiteSpace(gamesInfo.gameTitle) && !string.IsNullOrWhiteSpace(gamesInfo.gamePublisher) && !string.IsNullOrWhiteSpace(gamesInfo.gameConsole) && !string.IsNullOrWhiteSpace(gamesInfo.gameCategory))
            {
                gameName = CapFirstLetter(gamesInfo.gameTitle.Trim().ToString());
                gamePub = CapFirstLetter(gamesInfo.gamePublisher.Trim().ToString());
                gameConsole = CapFirstLetter(gamesInfo.gameConsole.Trim().ToString());
                gameCategory = CapFirstLetter(gamesInfo.gameCategory.Trim().ToString());
            }
            else
            {
                errorMessage = "Game Title, Publisher, Console, and Category are required.";
                return;
            }

            //saving data to the database
            try
            {
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    String sqlTitle = "SELECT gameTitle, gameImage FROM gametable";
                    using (MySqlCommand gameCommand = new MySqlCommand(sqlTitle, connection))
                    {
                        using (MySqlDataReader reader = gameCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GamesInfo gName = new GamesInfo();
                                gName.gameTitle = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                                gName.gameImage = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                                title = gName.gameTitle.Trim().ToLower().ToString();
                                gameImg = gName.gameImage;

                                if (title.Equals(gameName.ToLower()))
                                {
                                    errorMessage = gameName + " is already in our records.";
                                    return;
                                }
                            }
                            reader.Close();
                        }
                    }
                        String sql = "INSERT INTO gametable" +
                            "(gameTitle,gamePublisher,gameConsole,gameCategory,gameRating,gameImage) VALUES " +
                            "(@gameTitle,@gamePublisher,@gameConsole,@gameCategory,@gameRating,@gameImage);";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@gameTitle", gameName);
                        command.Parameters.AddWithValue("@gamePublisher", gamePub);
                        command.Parameters.AddWithValue("@gameConsole", gameConsole);
                        command.Parameters.AddWithValue("@gameCategory", gameCategory);
                        command.Parameters.AddWithValue("@gameRating", gamesInfo.gameRating);
                        command.Parameters.AddWithValue("@gameImage", gamesInfo.gameImage);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch
            (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            gamesInfo.gameTitle = string.Empty;
            gamesInfo.gamePublisher = string.Empty;
            gamesInfo.gameConsole = string.Empty;
            gamesInfo.gameCategory = string.Empty;
            gamesInfo.gameRating = 0;
            gamesInfo.gameImage = string.Empty;

            successMessage = gameName + " was added.";
            insertImg = gamesInfo.gameImage;
            success = true;

            Response.Redirect("/GameCollection");
        }
    }
}
