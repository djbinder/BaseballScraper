// using BaseballScraper.Models.Player;
using BaseballScraper.Models.BaseballSavant;
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

        public DbSet<StartingPitcherCsw> StartingPitcherCsws { get; set; }
        public DbSet<StartingPitcherCswSingleDay> StartingPitcherCswsSingleDays { get; set; }
        public DbSet<StartingPitcherCswDateRange> StartingPitcherCswsDateRanges { get; set; }
    }
}


// Database Migrations
// 1) update appsettings.json first
// 2) delete old Migrations folder
// 3) dotnet ef migrations add YourMigrationName
        // dotnet ef migrations add mig05302019_1
// 4) dotnet ef database update

// help if issues with 42P07 (relation already exists) errors
// https://weblog.west-wind.com/posts/2016/jan/13/resetting-entity-framework-migrations-to-a-clean-slate


// DBInfo from json file may be stored and read from user secrets
    // view all secrets:
        // dotnet user-secrets list
    // remove one secret:
        // dotnet user-secrets remove "DBInfo:ConnectionString"
    // set secret:
        // dotnet user-secrets set "DBInfo:ConnectionString" "server=localhost;userId=postgres;password=root;port=5432;database=BS_05302019_1;"
// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows
// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=linux


// set all user secrets from appsettings.Development.json:
    // cat ./Configuration/appsettings.Development.json | dotnet user-secrets set
// clear all secrets
    // dotnet user-secrets clear
