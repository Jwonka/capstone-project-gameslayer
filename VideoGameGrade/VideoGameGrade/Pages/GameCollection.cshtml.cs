using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VideoGameGrade.Pages
{
    public class GameCollectionModel : PageModel
    {
        private readonly ILogger<GameCollectionModel> _logger;

        public GameCollectionModel(ILogger<GameCollectionModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
