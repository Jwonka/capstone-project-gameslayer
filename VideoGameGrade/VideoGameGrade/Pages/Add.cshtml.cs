using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.IO;
using VideoGameGrade.Services;
using static VideoGameGrade.Pages.GameCollectionModel;
using VideoGameGrade.Classes;
using Dapper;
using System.Data;

namespace VideoGameGrade.Pages
{
    public class AddModel : PageModel
    {
        // Properties to hold game information and error messages
        public GamesInfo gamesInfo = new GamesInfo();
        public string errorMessage = string.Empty;
        public string errorMsg = string.Empty;
        public static string successMessage = string.Empty;
        public string gameName = string.Empty;
        public string gamePub = string.Empty;
        public string gamePlatform = string.Empty;
        public string gameCategory = string.Empty;
        public string title = string.Empty;
        public static string gameImg = string.Empty;
        public static bool success = false;
        public static string insertImg { get; set; }

        private readonly IAzureBlobStorageService _azureBlobStorageService;
        private readonly IDbConnection _db;

        // Constructor to initialize the Azure Blob Storage service and the database connection
        public AddModel(IAzureBlobStorageService azureBlobStorageService, IDbConnection db)
        {
            _azureBlobStorageService = azureBlobStorageService;
            _db = db;
        }

        // Property to bind the uploaded game image
        [BindProperty]
        public IFormFile gameImage { get; set; }

        // Method to capitalize the first letter of a string
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

        // POST handler for the page
        public async Task<IActionResult> OnPostAsync()
        {
            // Retrieve game information from form data
            gamesInfo.gameTitle = Request.Form["gameTitle"];
            gamesInfo.gamePublisher = Request.Form["gamePublisher"];
            gamesInfo.gamePlatform = Request.Form["gamePlatform"];
            gamesInfo.gameCategory = Request.Form["gameCategory"];

            // Set default gameRating
            gamesInfo.gameRating = 5;

            if (string.IsNullOrWhiteSpace(gamesInfo.gameTitle) || string.IsNullOrWhiteSpace(gamesInfo.gamePublisher) ||
                string.IsNullOrWhiteSpace(gamesInfo.gamePlatform) || string.IsNullOrWhiteSpace(gamesInfo.gameCategory))
            {
                errorMessage = "Title, publisher, platform, and category are required.";
                return Page(); // Stop execution and return the same page with an error message
            }

            // Capitalize game information
            gameName = CapFirstLetter(gamesInfo.gameTitle.Trim());
            gamePub = CapFirstLetter(gamesInfo.gamePublisher.Trim());

            // Upload game image
            string imageUrl = await UploadGameImage();

            // Get userId of the current user
            var userEmail = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var userId = _db.QuerySingleOrDefault<int>("SELECT userId FROM usertable WHERE userName = @UserName", new { UserName = userEmail });

            if (userId == 0)
            {
                errorMessage = "User not found.";
                return Page();
            }

            // Save game information to the database
            if (!SaveGameToDatabase(imageUrl, userId))
            {
                return Page(); // Return with database error message if saving fails
            }

            // Upon successful addition
            insertImg = imageUrl;
            successMessage = $"{gameName} was added.";
            success = true;

            // Redirect to GameCollection page after successful addition
            return RedirectToPage("/GameCollection");
        }

        // Method to upload the game image to Azure Blob Storage
        private async Task<string> UploadGameImage()
        {
            if (gameImage != null && gameImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(gameImage.FileName);
                try
                {
                    return await _azureBlobStorageService.UploadFileAsync(gameImage.OpenReadStream(), fileName);
                }
                catch (Exception ex)
                {
                    errorMessage = "Error uploading image: " + ex.Message;
                    return null;
                }
            }
            else
            {
                return "https://cs710032001ea63f0a3.blob.core.windows.net/images/kisspng-video-game-game-controller-joystick-online-game-vector-gamepad-5a7166f1d5b6b1.8005384115173813618754.png";
            }
        }

        // Method to save game information to the database
        private bool SaveGameToDatabase(string imageUrl, int userId)
        {
            string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Check for duplicate game titles
                    String sqlTitle = "SELECT gameTitle FROM gametable WHERE gameTitle = @gameTitle AND userId = @userId";

                    var existingTitle = connection.QuerySingleOrDefault<string>(sqlTitle, new { gameTitle = gameName, userId });

                    if (!string.IsNullOrEmpty(existingTitle))
                    {
                        errorMsg = gameName + " is already in our records.";
                        return false;
                    }

                    // Insert the new game into the database
                    string sql = "INSERT INTO gametable (gameTitle, gamePublisher, gamePlatform, gameCategory, gameRating, gameImage, userId) VALUES (@gameTitle, @gamePublisher, @gamePlatform, @gameCategory, @gameRating, @gameImage, @userId)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@gameTitle", gameName);
                        command.Parameters.AddWithValue("@gamePublisher", gamePub);
                        command.Parameters.AddWithValue("@gamePlatform", gamesInfo.gamePlatform);
                        command.Parameters.AddWithValue("@gameCategory", gamesInfo.gameCategory);
                        command.Parameters.AddWithValue("@gameRating", gamesInfo.gameRating);
                        command.Parameters.AddWithValue("@gameImage", imageUrl);
                        command.Parameters.AddWithValue("@userId", userId);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            }

            catch (Exception ex)
            {
                errorMessage = "Database error: " + ex.Message;
                return false;
            }
        }
    }
}