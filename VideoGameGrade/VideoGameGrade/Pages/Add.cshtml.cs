using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using static VideoGameGrade.Pages.GameCollectionModel;

namespace VideoGameGrade.Pages
{
    public class AddModel : PageModel
    {
        public GamesInfo gamesInfo = new GamesInfo();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
        }
        public void OnPost() 
        {
            gamesInfo.gameTitle = Request.Form["gameTitle"];
            gamesInfo.gamePublisher = Request.Form["gamePublisher"];
            gamesInfo.gameConsole = Request.Form["gameConsole"];
            gamesInfo.gameCategory = Request.Form["gameCategory"];
            // Explicitly convert gameRating to int
            if (int.TryParse(Request.Form["gameRating"], out int rating))
            {
                gamesInfo.gameRating = rating;
            }
            else
            {
                //just a stand in for now
                errorMessage = "Must be a whole number";
            }
            //gamesInfo.gameImage = Request.Form["gameImage"];
            gamesInfo.gameQuiz = Request.Form["gameQuiz"];
            gamesInfo.gameAnswer = Request.Form["gameAnswer"];

            if (gamesInfo.gameTitle.Length == 0 || gamesInfo.gamePublisher.Length == 0 || gamesInfo.gameConsole.Length == 0 ||
                gamesInfo.gameCategory.Length == 0 || gamesInfo.gameImage.Length == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }

            //saving data to the database

            try 
            {
                string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "INSERT INTO gametable" +
                        "(gameTitle,gamePublisher,gameConsole,gameCategory,gameRating,gameImage,gameQuiz,gameAnswer) VALUES " +
                        "(@gameTitle,@gamePublisher,@gameConsole,@gameCategory,@gameRating,@gameImage,@gameQuiz,@gameAnswer);";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)) 
                    {
                        command.Parameters.AddWithValue("@gameTitle", gamesInfo.gameTitle);
                        command.Parameters.AddWithValue("@gamePublisher", gamesInfo.gamePublisher);
                        command.Parameters.AddWithValue("@gameConsole", gamesInfo.gameConsole);
                        command.Parameters.AddWithValue("@gameCategory", gamesInfo.gameCategory);
                        command.Parameters.AddWithValue("@gameRating", 0);
                        command.Parameters.AddWithValue("@gameImage", "Stand-in");
                        command.Parameters.AddWithValue("@gameQuiz", gamesInfo.gameQuiz);
                        command.Parameters.AddWithValue("@gameAnswer", gamesInfo.gameAnswer);

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch
            (Exception ex)
            {
                errorMessage =ex.Message;
                return;
            }

            gamesInfo.gameTitle = "";
            gamesInfo.gamePublisher = "";
            gamesInfo.gameConsole = "";
            gamesInfo.gameCategory = "";
            gamesInfo.gameRating = 0;
            //gamesInfo.gameImage = "";
            successMessage = "New Game Added";

            Response.Redirect("/GameCollection");
        }
    }
}
