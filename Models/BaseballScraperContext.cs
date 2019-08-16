// using BaseballScraper.Models.Player;
using BaseballScraper.Models.BaseballHq;
using BaseballScraper.Models.BaseballSavant;
using BaseballScraper.Models.FanGraphs;
using BaseballScraper.Models.Player;
using BaseballScraper.Models.Yahoo;
using BaseballScraper.Models.Yahoo.Resources.YahooTeamResource;
using Microsoft.EntityFrameworkCore;

namespace BaseballScraper.Models
{
    public class BaseballScraperContext: DbContext
    {
        public BaseballScraperContext(DbContextOptions<BaseballScraperContext> options): base(options) { }

        public string Name                { get; set; }
        public string ConnectionString    { get; set; }
        public string SqlName             { get; set; }
        public string SqlConnectionString { get; set; }

        public DbSet<YahooTeamResource              > YahooTeamResource             { get; set; }
        public DbSet<SfbbPlayerBase                 > SfbbPlayerBases               { get; set; }
        public DbSet<PlayerNote                     > PlayerNotes                   { get; set; }
        public DbSet<StartingPitcherCsw             > StartingPitcherCsws           { get; set; }
        public DbSet<StartingPitcherCswSingleDay    > StartingPitcherCswsSingleDays { get; set; }
        public DbSet<StartingPitcherCswDateRange    > StartingPitcherCswsDateRanges { get; set; }
        public DbSet<FanGraphsPitcherForWpdiReport  > FanGraphsPitcherForWpdiReport { get; set; }
        public DbSet<HqHitterRestOfSeasonProjection > BaseballHqReportHitterROS     { get; set; }
        public DbSet<HqHitterYearToDate             > BaseballHqHitterYTD           { get; set; }
        public DbSet<ExitVelocityAndBarrelsHitter> ExitVelocityAndBarrelsHitter { get; set; }
        public DbSet<XstatsHitter> XStatsHitter { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<SfbbPlayerBase>()
            //     .HasOne(sfbb => sfbb.BaseballSavantHitter)
            //     .WithOne(bsh => bsh.SfbbPlayerBase)
            //     .HasForeignKey<BaseballSavantHitter>(bsh => bsh.PlayerId);
            modelBuilder.Entity<BaseballSavantHitter>()
                .HasOne(bsh => bsh.SfbbPlayerBase)
                .WithOne(sfbb => sfbb.BaseballSavantHitter)
                .HasForeignKey<SfbbPlayerBase>(sfbb => sfbb.MLBID);
        }
    }
}


// Database Migrations
// 1) update appsettings.Development.json first
//      i.e., change this "database=BS_08_05_2019_1" to today's date
// 2) Clear secrets; set new secrets (for DB info in appsettings files)
//      * dotnet user-secrets clear
        /*
            cat ./Configuration/appsettings.Development.json | dotnet user-secrets set
            cat ./Configuration/airtableConfiguration.json | dotnet user-secrets set
        */
// 3) delete old Migrations folder
// 4) dotnet ef migrations add YourMigrationName
        // dotnet ef migrations add mig05302019_1 OR mig08_06_2019_1
// 5) dotnet ef database update

// To add a table to an already migrated database, just do steps 4 and 5 (after you've added the DbSet to this file)



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


// set all user secrets from appsettings.Development.json and other config files:
    // cat ./Configuration/appsettings.Development.json | dotnet user-secrets set
    // cat ./Configuration/airtableConfiguration.json | dotnet user-secrets set
    // etc.
// clear all secrets
    // dotnet user-secrets clear
