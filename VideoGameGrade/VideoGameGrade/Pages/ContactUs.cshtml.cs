using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;

namespace VideoGameGrade.Pages
{
    public class ContactUsModel : PageModel
    {
        private readonly ILogger<ContactUsModel> _logger;
        private readonly SendGridSettings _sendGridSettings;

        public ContactUsModel(ILogger<ContactUsModel> logger, IOptions<SendGridSettings> sendGridSettings)
        {
            _logger = logger;
            _sendGridSettings = sendGridSettings.Value;
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

            var client = new SendGridClient(_sendGridSettings.ApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_sendGridSettings.SenderEmail, "VideoGameGrade Support"),
                Subject = "New Contact Us Message",
                PlainTextContent = $"Name: {ContactForm.Name}\nEmail: {ContactForm.Email}\nMessage: {ContactForm.Message}"
            };
            msg.AddTo(new EmailAddress("receiver@example.com", "Receiver Name"));

            try
            {
                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
                }

                _logger.LogInformation("Email sent successfully to {Recipient}", "receiver@example.com");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message from {Email}", ContactForm.Email);
                ModelState.AddModelError("", "An error occurred while sending your message.");
                return Page();
            }

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

    public class SendGridSettings
    {
        public string ApiKey { get; set; }
        public string SenderEmail { get; set; }
    }
}