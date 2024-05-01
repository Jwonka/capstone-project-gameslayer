using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.IO;
using VideoGameGrade.Services;
using static VideoGameGrade.Pages.GameCollectionModel;

namespace VideoGameGrade.Pages
{
    public class AddModel : PageModel
    {
        // Properties to hold game information and error messages
        public GamesInfo gamesInfo = new GamesInfo();
        public string errorMessage = string.Empty;
        public static string successMessage = string.Empty;
        public string gameName = string.Empty;
        public string gamePub = string.Empty;
        public string gameConsole = string.Empty;
        public string gameCategory = string.Empty;
        public static bool success = false;
        public static string insertImg { get; set; }

        private readonly IAzureBlobStorageService _azureBlobStorageService;

        // Constructor to initialize the Azure Blob Storage service
        public AddModel(IAzureBlobStorageService azureBlobStorageService)
        {
            _azureBlobStorageService = azureBlobStorageService;
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
            gamesInfo.gameConsole = Request.Form["gameConsole"];
            gamesInfo.gameCategory = Request.Form["gameCategory"];

            // Validate game rating and required fields
            if (!int.TryParse(Request.Form["gameRating"], out int rating) || rating < 0 || rating > 1)
            {
                errorMessage = "Invalid game rating. Ensure it's between 0 and 1.";
                return Page(); // Stop execution and return the same page with an error message
            }
            gamesInfo.gameRating = rating;

            if (string.IsNullOrWhiteSpace(gamesInfo.gameTitle) || string.IsNullOrWhiteSpace(gamesInfo.gamePublisher) ||
                string.IsNullOrWhiteSpace(gamesInfo.gameConsole) || string.IsNullOrWhiteSpace(gamesInfo.gameCategory))
            {
                errorMessage = "All fields are required.";
                return Page(); // Stop execution and return the same page with an error message
            }

            // Capitalize game information
            gameName = CapFirstLetter(gamesInfo.gameTitle.Trim());
            gamePub = CapFirstLetter(gamesInfo.gamePublisher.Trim());
            gameConsole = CapFirstLetter(gamesInfo.gameConsole.Trim());
            gameCategory = CapFirstLetter(gamesInfo.gameCategory.Trim());

            // Upload game image
            string imageUrl = await UploadGameImage();
            if (imageUrl == null)
            {
                return Page(); // Return with error message if upload fails
            }

            // Save game information to the database
            if (!SaveGameToDatabase(imageUrl))
            {
                return Page(); // Return with database error message if saving fails
            }

            // Upon successful addition
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
                errorMessage = "An image file is required.";
                return null;
            }
        }

        // Method to save game information to the database
        private bool SaveGameToDatabase(string imageUrl)
        {
            string connectionString = "Server=videogamegrade.mysql.database.azure.com;Database=videogamegrade_db;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO gametable (gameTitle, gamePublisher, gameConsole, gameCategory, gameRating, gameImage) VALUES (@gameTitle, @gamePublisher, @gameConsole, @gameCategory, @gameRating, @gameImage)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@gameTitle", gameName);
                        command.Parameters.AddWithValue("@gamePublisher", gamePub);
                        command.Parameters.AddWithValue("@gameConsole", gameConsole);
                        command.Parameters.AddWithValue("@gameCategory", gameCategory);
                        command.Parameters.AddWithValue("@gameRating", gamesInfo.gameRating);
                        command.Parameters.AddWithValue("@gameImage", imageUrl);
                        command.ExecuteNonQuery();
                    }
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