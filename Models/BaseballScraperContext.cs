using BaseballScraper.Models.Player;
using BaseballScraper.Models.Yahoo;
using Microsoft.EntityFrameworkCore;

namespace BaseballScraper.Models
{
    public class BaseballScraperContext: DbContext
    {
        public BaseballScraperContext(DbContextOptions<BaseballScraperContext> options): base(options) { }

        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string SqlName { get; set; }
        public string SqlConnectionString { get; set; }


        public DbSet<YahooTeamBase> YahooTeamBase { get; set; }
        public DbSet<PlayerNote> PlayerNotes { get; set; }
    }
}


// Database Migrations
// dotnet ef migrations add YourMigrationName
// dotnet ef database update