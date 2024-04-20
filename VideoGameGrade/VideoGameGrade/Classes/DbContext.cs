using Microsoft.EntityFrameworkCore;
namespace VideoGameGrade.Classes
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) 
        {
        }
        public virtual DbSet<Game> Game {  get; set; }
    }
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
    public class GameController
    {
        private readonly AppDbContext _context;

        public GameController(AppDbContext context)
        {
  
            _context = context;
        }
    }
}
