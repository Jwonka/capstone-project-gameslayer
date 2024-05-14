using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Dapper;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace VideoGameGrade.Pages
{
    public class RateModel : PageModel
    {
        public List<RateInfo> ratings = new List<RateInfo>();
        public string rateTitle = string.Empty;
        public string rateImage = string.Empty;
        public string gameComment = string.Empty;
        public string deletedName = string.Empty;
        public string message = string.Empty;
        public int gameId = 0;
        public int rateID = 0;
        public int iD = 0;
        public int ID = 0;
        public int rating = 0;
        public static Boolean rated;

        private readonly IDbConnection _db;

        public RateModel(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Boolean to limit rating games one per page visit
            if (TempData["rated"] != null)
            {
                rated = (bool)TempData["rated"];
            }
            

            // Get gameId from Http header
            if (int.TryParse(Request.Query["id"], out int num))
            {
                iD = num;
            }
            else
            {
                message = "No ID provided.";
                return Page();
            }

            if (!readRateInfoFromDatabase(iD))
            {
                return Page();
            }

            ratings.Clear();

            // Get the name of the button pressed to determine rating increase or decrease
            string addRating = Request.Form["addRating"];
            string removeRating = Request.Form["removeRating"];

            // Get the comment entered in the input
            string comment = Request.Form["comment"];

            // Get confirmation the submit button was pressed
            string submit = Request.Form["submit"];

            // Verify that both comment and submit are not empty so we can add a comment
            if (!string.IsNullOrEmpty(comment) && !string.IsNullOrEmpty(submit))
            {
                if (!AddComment(comment, iD, rating, rateTitle))
                {
                    return Page();
                }
            }
            else if (string.IsNullOrWhiteSpace(comment) && !string.IsNullOrEmpty(submit) && submit.Equals("submit"))
            {   // Display error message for empty comment input
                TempData["message"] = rateTitle + "'s comments cannot be blank.";
                return Page();
            }

            // Check if remove or add rating button was pressed. If so call method to determine increase
            if (!string.IsNullOrEmpty(removeRating) || !string.IsNullOrEmpty(addRating))
            {
                if (rated)
                {
                    await DetermineRating(rating, iD, addRating, removeRating);
                    TempData["rated"] = false;
                    rated = false;
                    return RedirectToPage(new { id = iD, rated });
                }
                else
                {
                    TempData["message"] = "You can only rate once per game.";
                    return RedirectToPage(new { id = iD, rated });
                }
                
            }

            return RedirectToPage(new { id = iD});
        }

        // All related info for specific game for display on the page
        private bool readRateInfoFromDatabase(int iD)
        {
            try
            {
                // Connection string
                string rateString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

                using (MySqlConnection connect = new MySqlConnection(rateString))
                {
                    connect.Open();
                    string gTitleSql = "SELECT gametable.gameId, gametable.gameTitle, gametable.gameImage, ratings.rateID, ratings.gameRating, ratings.gameComment FROM ratings INNER JOIN gametable ON gametable.gameId = ratings.gameId WHERE ratings.gameId = @id;";
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
                                rateInfo.gameImage = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                rateInfo.rateID = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                rateInfo.gameRating = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                rateInfo.gameComment = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);

                                // Grab game rate info for display on Ratings page
                                gameId = rateInfo.gameId;
                                rateID = rateInfo.rateID;
                                rateTitle = AddModel.CapFirstLetter(rateInfo.gameTitle.ToString());
                                rateImage = rateInfo.gameImage;
                                rating = rateInfo.gameRating;
                                gameComment = rateInfo.gameComment;

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
                TempData["message"] = "Exception: " + ex.Message;
                return false;
            }
            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (int.TryParse(Request.Query["id"], out int num))
            {
                iD = num;
                if (!readRateInfoFromDatabase(iD))
                {
                    return Page();
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                TempData["message"] = "No ID provided.";
                return Page();
            }
        }

        // Determine whether to increase the games rating by one or decrease by one
        private async Task<bool> DetermineRating(int rating, int iD, string addRating, string removeRating)
        {
            // Set local variable to determine amount to increase or decrease ratings
            int ratingChange = 0;

            // Set local variable to equal the amount to increase or decrease
            if (!string.IsNullOrEmpty(removeRating) && removeRating.Equals("remove"))
            {
                ratingChange = -1;
            }
            else if (!string.IsNullOrEmpty(addRating) && addRating.Equals("add"))
            {
                ratingChange = 1;
            }

            // Determine if ratings can be increased or decreased
            if (rating <= 0 && ratingChange == -1)
            {
                TempData["message"] = rateTitle + " cannot have a negative rating.";
                ratingChange = 0;
                return false;
            }
            else if (rating >= 10 && ratingChange == 1)
            {
                TempData["message"] = rateTitle + " cannot have a rating higher than 10.";
                ratingChange = 0;
                return false;
            }
            else
            {
                // Once the change in rating has been set call method to change it
                await ChangeRating(ratingChange, iD);
                return true;
            }
        }

        // Update rating for the specified RateID
        private async Task<PageResult> ChangeRating(int ratingChange, int iD)
        {
            if (!ratingChange.Equals(null) && ratingChange != 0)
            {
                try
                {
                    // Connection string
                    string rateString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";

                    using (MySqlConnection connect = new MySqlConnection(rateString))
                    {
                        connect.Open();
                        string rateSql = "UPDATE ratings SET gameRating = gameRating + @ratingChange WHERE ratings.gameId = @id;";
                        // Adjust the rating based on the selected button
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
                    TempData["message"] = "Exception: " + ex.Message;
                    return Page();
                }
            }
            else
            {
                return Page();
            }
            // Reread the database so information is current
            readRateInfoFromDatabase(iD);
            return Page();
        }

        // Add a comment to the specific game
        private bool AddComment(string comment, int gameId, int rating, string rateTitle)
        {
            // Validate the comment to allow letters, numbers, and punctuation
            if (!Regex.IsMatch(comment, @"^[a-zA-Z0-9\s.,!?-]*$"))
            {
                TempData["message"] = "Comments can only contain letters, numbers, and punctuation.";
                return false;
            }

            // Get comment and trim it for insertion into the database
            gameComment = comment.Trim();
            try
            {
                // Connection string
                string rateString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;charset=utf8mb4;";

                using (MySqlConnection connect = new MySqlConnection(rateString))
                {
                    connect.Open();

                    string userEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                    int userId = _db.QuerySingleOrDefault<int>("SELECT userId FROM usertable WHERE userName = @UserName", new { UserName = userEmail });

                    if (userId == 0)
                    {
                        TempData["message"] = "User not found.";
                        return false;
                    }

                    // Add a new comment
                    string insertSql = "INSERT INTO ratings (gameId, gameRating, gameComment, userId) VALUES (@gameId, @rating, @comment, @userId)";
                    using (MySqlCommand command = new MySqlCommand(insertSql, connect))
                    {
                        command.Parameters.AddWithValue("@gameId", gameId);
                        command.Parameters.AddWithValue("@rating", rating);
                        command.Parameters.AddWithValue("@comment", gameComment);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();
                    }
                    //TempData["message"] = "You have added a comment to " + rateTitle + ".";

                    connect.Close();
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Exception: " + ex.Message;
                return false;
            }

            // Clear the list to avoid duplicates
            ratings.Clear();
            // Refresh the ratings list
            readRateInfoFromDatabase(gameId);
            return true;
        }

        public class RateInfo
        {
            [Key]
            public int rateID { get; set; }

            [Required]
            [DisplayName("Game Title")]
            public string gameTitle { get; set; }

            [Required]
            public int gameId { get; set; }

            [Required]
            public int gameRating { get; set; }

            public string? gameImage { get; set; }
            public string? addRating { get; set; }
            public string? removeRating { get; set; }
            public string? gameComment { get; set; }

            [Required]
            public int replyID { get; set; }
        }
    }
}