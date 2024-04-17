using Microsoft.EntityFrameworkCore;
namespace VideoGameGrade.Classes
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) 
        {
        }
        public virtual DbSet<Game> Game {  get; set; }
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
