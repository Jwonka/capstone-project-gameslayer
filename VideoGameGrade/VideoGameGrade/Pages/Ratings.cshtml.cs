using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Configuration;
using static VideoGameGrade.Pages.GameCollectionModel;
using System.ComponentModel.DataAnnotations.Schema;
using static VideoGameGrade.Pages.RateModel;

namespace VideoGameGrade.Pages
{
    public class RateModel : PageModel
    {
        public List<RateInfo> ratings = new List<RateInfo>();
        public string rateTitle = string.Empty;
        public string rateSuccess = string.Empty;
        public string rateError = string.Empty;
        public string comment = string.Empty;
        public string addRating = string.Empty;
        public string removeRating = string.Empty;
        public int gameId = 0;
        public int replyID = 0;
        public int rateID = 0;
        public int iD = 0;
        public int rating = 0;
        public int up = 0;
        public int down = 0;

        public void OnGet()
        {
            if (int.TryParse(Request.Query["id"], out int num))
            {
                iD = num;
            }
            else
            {
                rateError = "No ID provided.";
                return;
            }

            try
            {
                // Connection string
                string rateString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

                using (MySqlConnection connect = new MySqlConnection(rateString))
                {
                    connect.Open();
                    String gTitleSql = "SELECT gametable.gameId, gametable.gameTitle,ratings.rateID, ratings.gameRating, ratings.comment, ratings.replyID FROM ratings INNER JOIN gametable ON gametable.gameId = ratings.gameId WHERE ratings.gameId = @id;";
                    using (MySqlCommand command = new MySqlCommand(gTitleSql, connect))
                    {
                        command.Parameters.AddWithValue("@id", iD);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Reads data from the database if data is null converts it to an empty string or zero if it is an integer
                                RateInfo rateInfo = new RateInfo();
                                rateInfo.gameId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                rateInfo.gameTitle = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                rateInfo.rateID = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                rateInfo.gameRating = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                rateInfo.comment = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                                rateInfo.replyID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);

                                // Grab game title for display on Ratings page
                                gameId = rateInfo.gameId;
                                rateID = rateInfo.rateID;
                                rateTitle = AddModel.CapFirstLetter(rateInfo.gameTitle.ToString());
                                rating = rateInfo.gameRating;
                                comment = rateInfo.comment;
                                replyID = rateInfo.replyID;

                                ratings.Add(rateInfo);
                            }
                            reader.Close();
                        }
                    }
                    connect.Close();
                }
            }
            catch (Exception ex)
            {
                rateError = "Exception: " + ex.Message;
            }
        }

        public void OnPost()
        {
            if (int.TryParse(Request.Query["id"], out int num))
            {
                iD = num;
            }
            else
            {
                rateError = "No ID provided.";
                return;
            }

            int ratingChange = 0;

            removeRating = Request.Form["removeRating"];
            addRating = Request.Form["addRating"];

            if (!string.IsNullOrEmpty(removeRating))
            {
                ratingChange = -1;
            }
            else if (!string.IsNullOrEmpty(addRating))
            {
                ratingChange = 1;
            }
            else
            {
                ratingChange = 0;
            }

            try
            {
                // Connection string
                string rateString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

                using (MySqlConnection connect = new MySqlConnection(rateString))
                {
                    connect.Open();
                    String gTitleSql = "SELECT ratings.gameRating, gametable.gameTitle, ratings.comment FROM ratings INNER JOIN gametable ON gametable.gameId = ratings.gameId WHERE ratings.gameId = @id;";
                    using (MySqlCommand command = new MySqlCommand(gTitleSql, connect))
                    {
                        command.Parameters.AddWithValue("@id", iD);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RateInfo info = new RateInfo();
                                info.gameRating = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                info.gameTitle = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                info.comment = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                rating = info.gameRating;
                                rateTitle = info.gameTitle;
                                comment = info.comment;
                                ratings.Add(info);
                            }
                            reader.Close();
                        }
                    }
                    connect.Close();
                }
            }
            catch (Exception ex)
            {
                rateError = "Exception: " + ex.Message;
            }

            if (rating <= 0 && ratingChange == -1)
            {
                rateError = "Games cannot have a negative rating.";
                ratingChange = 0;
            }
            else if (rating >= 10 && ratingChange == 1)
            {
                rateError = "Games cannot have a rating higher than 10.";
                ratingChange = 0;
            }

            try
            {
                // Connection string
                string rateString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

                using (MySqlConnection connect = new MySqlConnection(rateString))
                {
                    connect.Open();
                    String rateSql = "UPDATE ratings SET gameRating = gameRating + @ratingChange WHERE ratings.gameId = @id;";
                    using (MySqlCommand command = new MySqlCommand(rateSql, connect))
                    {
                        command.Parameters.AddWithValue("@id", iD);
                        command.Parameters.AddWithValue("@ratingChange", ratingChange);
                        command.ExecuteNonQuery();
                    }
                    connect.Close();
                }
            }
            catch (Exception ex)
            {
                rateError = "Exception: " + ex.Message;
            }
            if (ratingChange != 0)
            {
                Response.Redirect($"/Ratings?id={iD}");
            }
        }

        public class RateInfo
        {
            [Key]
            public int rateID { get; set; }

            [Required]
            [DisplayName("Game Title")]
            public string gameTitle { get; set; }


            public int gameId { get; set; }
            public int gameRating { get; set; }
            public string? addRating { get; set; }
            public string? removeRating { get; set; }
            public string? comment { get; set; }

            [Required]
            public int replyID { get; set; }
        }
    }
}
