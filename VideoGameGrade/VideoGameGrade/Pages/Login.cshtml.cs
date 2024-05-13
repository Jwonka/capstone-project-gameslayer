using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace VideoGameGrade.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IDbConnection _db; // Database connection

        public LoginModel(IDbConnection db)
        {
            _db = db; // Dependency injection of the database connection
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Use Dapper to query the database for the user
            string sql = "SELECT * FROM usertable WHERE username = @Username";
            var user = await _db.QuerySingleOrDefaultAsync<User>(sql, new { Username = Input.Username });

            // Check if user exists and the password is correct
            if (user != null && BCrypt.Net.BCrypt.Verify(Input.Password, user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = Url.Content("~/Account") // Redirect to account page after login
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return LocalRedirect(authProperties.RedirectUri);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        private class User
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}