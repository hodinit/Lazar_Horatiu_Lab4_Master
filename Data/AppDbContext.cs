using Microsoft.EntityFrameworkCore;

namespace Lazar_Horatiu_Lab4_Master.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Models.PredictionHistory> PredictionHistories { get; set; }
    }
}
