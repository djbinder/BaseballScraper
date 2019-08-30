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


        /* PLAYER BASES */
        public DbSet<SfbbPlayerBase>                    SfbbPlayerBases                 { get; set; }
        public DbSet<CrunchTimePlayerBase>              CrunchTimePlayerBases           { get; set; }


        /* BASEBALL HQ */
        public DbSet<HqHitterRestOfSeasonProjection>    HqHitterRestOfSeasonProjections { get; set; }
        public DbSet<HqHitterYearToDate>                HqHitterYearToDates             { get; set; }


        /* BASEBALL SAVANT */
        public DbSet<ExitVelocityAndBarrelsHitter>      ExitVelocityAndBarrelsHitters   { get; set; }
        public DbSet<XstatsHitter>                      XStatsHitters                   { get; set; }
        // public DbSet<StartingPitcherCsw>                StartingPitcherCsws             { get; set; }
        public DbSet<StartingPitcherCswSingleDay>       StartingPitcherCswsSingleDays   { get; set; }
        public DbSet<StartingPitcherCswDateRange>       StartingPitcherCswsDateRanges   { get; set; }
        public DbSet<StartingPitcherCswFullSeason>      StartingPitcherCswsFullSeason   { get; set; }


        /* YAHOO */
        public DbSet<YahooTeamResource>                 YahooTeamResource               { get; set; }


        /* FANGRAPHS */
        public DbSet<FanGraphsPitcherForWpdiReport>     FanGraphsPitchersForWpdiReport  { get; set; }


        public DbSet<PlayerNote>                        PlayerNotes                     { get; set; }



        // public DbSet<BaseballSavantHitter> BaseballSavantHitter { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* PLAYER BASES */
            modelBuilder.Entity<SfbbPlayerBase>().ToTable("_BASE_Sfbb");
            modelBuilder.Entity<CrunchTimePlayerBase>().ToTable("_BASE_CrunchTime");

            /* BASEBALL HQ */
            modelBuilder.Entity<HqHitterRestOfSeasonProjection>().ToTable("HQ_HIT_ROS");
            modelBuilder.Entity<HqHitterYearToDate>().ToTable("HQ_HIT_YTD");

            /* BASEBALL SAVANT */
            modelBuilder.Entity<ExitVelocityAndBarrelsHitter>().ToTable("SAVANT_HIT_ExVeloBarrels");
            modelBuilder.Entity<XstatsHitter>().ToTable("SAVANT_HIT_XStats");


            modelBuilder.Entity<StartingPitcherCswSingleDay>()
                .ToTable("SAVANT_SP_SINGLE")
                .HasKey(s => new { s.PlayerId, s.DatePitched });

            modelBuilder.Entity<StartingPitcherCswDateRange>()
                .ToTable("SAVANT_SP_RANGE")
                .HasKey(s => new { s.PlayerId, s.StartDate, s.EndDate });

            modelBuilder.Entity<StartingPitcherCswFullSeason>()
                .ToTable("SAVANT_SP_SEASON")
                .HasKey(s => new { s.PlayerId, s.Season });

            /* YAHOO */
            modelBuilder.Entity<YahooTeamResource>().ToTable("Y!_TeamResource");
            modelBuilder.Entity<YahooTeamLogo>().ToTable("Y!_TeamLogo");
            modelBuilder.Entity<YahooManager>().ToTable("Y!_Manager");
            modelBuilder.Entity<YahooTeamRosterAdds>().ToTable("Y!_TmRosterAdds");

            /* FANGRAPHS */
            modelBuilder.Entity<FanGraphsPitcherForWpdiReport>().ToTable("FG_SP_wPDI");

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
        // dotnet ef migrations add MIG_08_29_2019_1
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









// modelBuilder.Entity<StartingPitcherCsw>().ToTable("SAVANT_SP_CSW");

// modelBuilder.Entity<BaseballSavantHitter>(entity =>
// {
//     entity.HasOne(s => s.SfbbPlayerBase)
//         .WithOne(b => b.BaseballSavantHitter)
//         .HasForeignKey<SfbbPlayerBase>(s => s.MLBID);
// });


// modelBuilder.Entity<SfbbPlayerBase>(entity =>
// {
//     entity.HasOne(s => s.BaseballSavantHitter)
//         .WithOne(b => b.SfbbPlayerBase)
//         .HasForeignKey<BaseballSavantHitter>(s => s.MLBID);

//     entity.HasOne(q => q.BaseballHqReportHitterBase)
//         .WithOne(s => s.SfbbPlayerBase)
//         .HasForeignKey<BaseballHqReportHitterBase>(q => q.HQID_);
// });


// .HasOne<SfbbPlayerBase>();
// .WithOne(pBase => pBase.MLBID_);

// .HasForeignKey(bsh => bsh.PlayerId)
// .HasPrincipalKey(x => x.MLBID);

// modelBuilder.Entity<BaseballSavantHitter>()
//     .HasOne(bsh => bsh.SfbbPlayerBase)
//     .WithOne(sfbb => sfbb.BaseballSavantHitter)
//     .HasForeignKey<SfbbPlayerBase>(sfbb => sfbb.MLBID);

// modelBuilder.Entity<SfbbPlayerBase>()
//     .HasOne(sfbb => sfbb.BaseballSavantHitter)
//     .HasForeignKey<BaseballSavantHitter>(savant => savant.MLBID)
//     .HasPrincipalKey(bsh => bsh.PlayerId);

// modelBuilder.Entity<SfbbPlayerBase>().HasKey(t => t.IDPLAYER);

// modelBuilder.Entity<BaseballHqReportHitterBase>()
//     .HasOne(s => s.SfbbPlayerBase)
//     .WithOne(b => b.BaseballHqReportHitterBase)
//     .HasForeignKey<SfbbPlayerBase>(s => s.HQID);


// modelBuilder.Entity<SfbbPlayerBase>()
//     .HasOne(b => b.BaseballHqReportHitterBase)
//     .WithOne(s => s.SfbbPlayerBase)
//     .HasForeignKey<BaseballHqReportHitterBase>(b => b.HQID_)
//     .HasPrincipalKey<SfbbPlayerBase>(s => s.HQID_);
// Console.WriteLine($"\n\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n\n");

// modelBuilder.Entity<BaseballSavantHitter>()
//     .HasOne(s => s.SfbbPlayerBase)
//     .WithOne(b => b.BaseballSavantHitter)
//     .HasForeignKey<SfbbPlayerBase>(s => s.MLBID);



// modelBuilder.Entity<SfbbPlayerBase>()
//     .HasOne(b => b.BaseballSavantHitter)
//     .WithOne(s => s.SfbbPlayerBase)
//     .HasForeignKey<BaseballSavantHitter>(b => b.MLBID)
//     .HasPrincipalKey<SfbbPlayerBase>(s => s.MLBID);
