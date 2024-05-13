using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;
using Dapper;
using BCrypt.Net;

namespace VideoGameGrade.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IDbConnection _db;

        public RegisterModel(IDbConnection db)
        {
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The password must be at least 6 characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Checks for an existing user with the same email
            var existingUser = await _db.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM usertable WHERE userName = @UserName", new { UserName = Input.Email });

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "User already exists with this email.");
                return Page();
            }

            // Ensure password column can store the value
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.Password);

            var newUser = new User { UserName = Input.Email, Password = hashedPassword };
            var insertQuery = "INSERT INTO usertable (userName, password) VALUES (@UserName, @Password)";

            var result = await _db.ExecuteAsync(insertQuery, newUser);

            if (result > 0)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Input.Email),
                    new Claim(ClaimTypes.Email, Input.Email),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = Url.Content("~/Account") // Redirect to account page after registration
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect(authProperties.RedirectUri);
            }

            ModelState.AddModelError(string.Empty, "An error occurred while registering your account.");
            return Page();
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}