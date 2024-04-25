using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Dapper;
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

        public void OnGet()
        {
            UserEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(UserEmail))
            {
                UserEmail = "Email not available"; // email is not found
            }
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            UserEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value; // Retrieve the email
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
                return Page();  // Fail if the current password is wrong or the user is not found
            }

            // Hash the new password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(ChangePasswordModel.NewPassword);
            // Update the password in the database
            var updateQuery = "UPDATE usertable SET password = @Password WHERE userName = @UserName";
            var result = await _db.ExecuteAsync(updateQuery, new { Password = hashedPassword, UserName = UserEmail });

            if (result > 0)
            {
                // Log out the user to enforce re-authentication with the new password
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Message"] = "Password successfully changed. Please login with your new password.";
                return RedirectToPage("/Login");
            }

            ModelState.AddModelError(string.Empty, "Failed to update password.");
            return Page();  // Display an error message if the update fails
        }

        private class User
        {
            public int UserId { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }
    }

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