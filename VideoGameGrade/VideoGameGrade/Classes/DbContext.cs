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
    }
}
