using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VideoGameGrade.Pages
{
    public class ContactUsModel : PageModel
    {
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

            // Process the form data here



            return RedirectToPage("/ContactUsConfirmation");
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
}