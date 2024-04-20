using Dapper;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VideoGameGrade.Pages
{
    public class ContactUsModel : PageModel
    {
        private readonly IDbConnection _connection;

        public ContactUsModel(IDbConnection connection)
        {
            _connection = connection;
        }

        [BindProperty]
        public ContactFormModel ContactForm { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var sql = "INSERT INTO Contacts (Name, Email, Message) VALUES (@Name, @Email, @Message);";

            var result = await _connection.ExecuteAsync(sql, new
            {
                Name = ContactForm.Name,
                Email = ContactForm.Email,
                Message = ContactForm.Message
            });

            if (result > 0)
            {
                return RedirectToPage("/ContactUsConfirmation");
            }
            else
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return Page();
            }
        }
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
    }
}