using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;
using System.Data;
using System.Linq;
using BCrypt.Net;

namespace VideoGameGrade.Pages
{
    public class AccountModel : PageModel
    {
        private readonly IDbConnection _db;

        public AccountModel(IDbConnection db)
        {
            _db = db;
        }

        [BindProperty]
        public ChangePasswordModel ChangePasswordModel { get; set; } = new ChangePasswordModel();

        public string UserEmail { get; set; }
        public List<GameInfo> UserGames { get; set; }
        public List<TriviaInfo> UserTrivia { get; set; }
        public List<CommentInfo> UserComments { get; set; }
        public int CorrectTriviaCount { get; set; }
        public int IncorrectTriviaCount { get; set; }

        public void OnGet()
        {
            LoadUserData(); // Load user data when the page is requested
        }

        private void LoadUserData()
        {
            UserEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(UserEmail))
            {
                UserEmail = "Email not available";
            }

            LoadUserGames();
            LoadUserTrivia();
            LoadUserTriviaStats();
            LoadUserComments();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            UserEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(UserEmail))
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            var user = await _db.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM usertable WHERE userName = @UserName", new { UserName = UserEmail });

            if (user == null || !BCrypt.Net.BCrypt.Verify(ChangePasswordModel.OldPassword, user.Password))
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                return Page();
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(ChangePasswordModel.NewPassword);
            var updateQuery = "UPDATE usertable SET password = @Password WHERE userName = @UserName";
            var result = await _db.ExecuteAsync(updateQuery, new { Password = hashedPassword, UserName = UserEmail });

            if (result > 0)
            {
                TempData["DeleteMsg"] = "Password successfully changed. Please login with your new password.";
                return RedirectToPage("/Login");
            }

            ModelState.AddModelError(string.Empty, "Failed to update password.");
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteGameAsync(int GameId)
        {
            var result = await _db.ExecuteAsync("DELETE FROM gametable WHERE gameId = @GameId", new { GameId });

            TempData["DeleteMsg"] = result > 0 ? "Game deleted successfully." : "Failed to delete the game.";
            LoadUserData();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteTriviaAsync(int QuizId)
        {
            var result = await _db.ExecuteAsync("DELETE FROM triviatable WHERE quizID = @QuizId", new { QuizId });

            TempData["DeleteMsg"] = result > 0 ? "Trivia question deleted successfully." : "Failed to delete the trivia question.";
            LoadUserData();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync(int CommentId)
        {
            var result = await _db.ExecuteAsync("DELETE FROM ratings WHERE rateID = @CommentId", new { CommentId });

            TempData["DeleteMsg"] = result > 0 ? "Comment deleted successfully." : "Failed to delete the comment.";
            LoadUserData();
            return Page();
        }

        private void LoadUserGames()
        {
            var userId = _db.QuerySingleOrDefault<int>("SELECT userId FROM usertable WHERE userName = @UserName", new { UserName = UserEmail });

            UserGames = _db.Query<GameInfo>(
                @"
                    SELECT g.gameId, g.gameTitle, g.gamePublisher, g.gamePlatform, g.gameCategory, g.gameRating, g.gameImage
                    FROM gametable g
                    WHERE g.userId = @UserId
                ",
                new { UserId = userId }
            ).ToList();
        }

        private void LoadUserTrivia()
        {
            var userId = _db.QuerySingleOrDefault<int>("SELECT userId FROM usertable WHERE userName = @UserName", new { UserName = UserEmail });

            UserTrivia = _db.Query<TriviaInfo>(
                @"
                    SELECT t.quizID, t.gameQuiz, t.gameAnswer, g.gameTitle
                    FROM triviatable t
                    INNER JOIN gametable g ON t.gameID = g.gameId
                    WHERE t.userId = @UserId
                ",
                new { UserId = userId }
            ).ToList();
        }

        private void LoadUserTriviaStats()
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return;
            }

            var userId = _db.QuerySingleOrDefault<int>("SELECT userId FROM usertable WHERE userName = @UserName", new { UserName = userEmail });

            if (userId == 0)
            {
                return;
            }

            var stats = _db.QuerySingleOrDefault<(int, int)>("SELECT SUM(answered_count) AS total_answered, SUM(correct_count) AS total_correct FROM user_trivia_stats WHERE userId = @UserId", new { UserId = userId });

            CorrectTriviaCount = stats.Item2;
            IncorrectTriviaCount = stats.Item1 - stats.Item2;
        }

        private void LoadUserComments()
        {
            var userId = _db.QuerySingleOrDefault<int>("SELECT userId FROM usertable WHERE userName = @UserName", new { UserName = UserEmail });

            UserComments = _db.Query<CommentInfo>(
                @"
                    SELECT r.rateID, r.gameId, r.gameRating, r.gameComment, g.gameTitle, g.gameImage
                    FROM ratings r
                    INNER JOIN gametable g ON r.gameId = g.gameId
                    WHERE r.userId = @UserId
                ",
                new { UserId = userId }
            ).ToList();
        }

        private class User
        {
            public int UserId { get; set; }
            public string Password { get; set; }
        }

        public class GameInfo
        {
            public int GameId { get; set; }
            public string GameTitle { get; set; }
            public string GamePublisher { get; set; }
            public string GamePlatform { get; set; }
            public string GameCategory { get; set; }
            public int GameRating { get; set; }
            public string GameImage { get; set; }
        }

        public class TriviaInfo
        {
            public int QuizId { get; set; }
            public string GameQuiz { get; set; }
            public string GameAnswer { get; set; }
            public string GameTitle { get; set; }
        }

        public class CommentInfo
        {
            public int RateId { get; set; }
            public int GameId { get; set; }
            public int GameRating { get; set; }
            public string GameComment { get; set; }
            public string GameTitle { get; set; }
            public string GameImage { get; set; }
        }
    }

    //representing model for changing password
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The new password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}