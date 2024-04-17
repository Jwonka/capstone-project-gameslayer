using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using VideoGameGrade.Classes;

namespace VideoGameGrade.Pages
{
    public class GameCollectionModel : PageModel
    {
        private readonly ILogger<GameCollectionModel> _logger;
        public JsonContent Game;
        public IEnumerable<Game> Games { get; set; }    

        public bool DatabaseConnected { get; set; }

        public GameCollectionModel(ILogger<GameCollectionModel> logger)
        {
            _logger = logger;
     
        }

        protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>();
        }

        public void OnGet()
        {   
       
        }
    }
}
