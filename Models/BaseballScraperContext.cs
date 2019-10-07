using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseballScraper.Controllers.FanGraphsControllers;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.BaseballHq;
using BaseballScraper.Models.BaseballSavant;
using BaseballScraper.Models.FanGraphs;
using BaseballScraper.Models.Player;
using BaseballScraper.Models.Yahoo;
using BaseballScraper.Models.Yahoo.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using C = System.Console;

#pragma warning disable MA0061
namespace BaseballScraper.Models
{
    public class BaseballScraperContext: DbContext
    {
        private readonly Helpers _helpers;
        // private readonly FanGraphsSpController _fangraphsSpController;
        // private readonly PlayerBaseController _playerBaseController;
        private readonly PlayerBaseController _playerBaseController = new PlayerBaseController();

        // FanGraphsSpController fangraphsSpController, PlayerBaseController playerBaseController
        public BaseballScraperContext(DbContextOptions<BaseballScraperContext> options, Helpers helpers): base(options)
        {
            _helpers = helpers;
            // _fangraphsSpController = fangraphsSpController;
            // _playerBaseController = playerBaseController;
        }

        public BaseballScraperContext(){}



        public string Name                { get; set; }
        public string ConnectionString    { get; set; }
        public string SqlName             { get; set; }
        public string SqlConnectionString { get; set; }


        /* PLAYER BASES */
        public DbSet<SfbbPlayerBase>        SfbbPlayerBases       { get; set; }
        public DbSet<CrunchTimePlayerBase>  CrunchTimePlayerBases { get; set; }


        /* BASEBALL HQ */
        public DbSet<HqHitterRestOfSeasonProjection>    HqHitterRestOfSeasonProjections { get; set; }
        public DbSet<HqHitterYearToDate>                HqHitterYearToDates             { get; set; }


        /* BASEBALL SAVANT */
        public DbSet<ExitVelocityAndBarrelsHitter>   ExitVelocityAndBarrelsHitters   { get; set; }
        public DbSet<XstatsHitter>                   XStatsHitters                   { get; set; }

        public DbSet<StartingPitcherCswSingleDay>    StartingPitcherCswsSingleDays   { get; set; }
        public DbSet<StartingPitcherCswDateRange>    StartingPitcherCswsDateRanges   { get; set; }
        public DbSet<StartingPitcherCswFullSeason>   StartingPitcherCswsFullSeason   { get; set; }


        /* YAHOO */
        public DbSet<YahooTeamResource> YahooTeamResource { get; set; }


        /* FANGRAPHS */
        public DbSet<FanGraphsPitcherWpdi>     FanGraphsPitchersForWpdiReport  { get; set; }


        public DbSet<PlayerNote> PlayerNotes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _helpers.OpenMethod(1);


            var cascadeFks = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(
                    fk => !fk.IsOwnership &&
                    fk.DeleteBehavior == DeleteBehavior.Cascade
                );

            foreach(Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey fk in cascadeFks)
                fk.DeleteBehavior = DeleteBehavior.SetNull;

            base.OnModelCreating(modelBuilder);


            /* PLAYER BASES */
            modelBuilder.Entity<SfbbPlayerBase>()
                .ToTable("_BASE_Sfbb");

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
            modelBuilder.Entity<FanGraphsPitcherWpdi>().ToTable("FG_SP_wPDI");

            SeedDatabase(modelBuilder);


            _helpers.CloseMethod(1);

        }


        // See: http://bit.ly/2ZJTvbK
        // * Runs when you do _context.SaveChanges();
        public override int SaveChanges()
        {
            // _helpers.OpenMethod(1);

            IEnumerable<EntityEntry> entities = ChangeTracker.Entries()
                .Where(
                    entityEntry => entityEntry.Entity is IBaseEntity && (
                        entityEntry.State == EntityState.Added ||
                        entityEntry.State == EntityState.Modified
                    )
                );

            foreach (EntityEntry entity in entities)
            {
                if (entity.State == EntityState.Added)
                    ((IBaseEntity)entity.Entity).DateCreated = DateTime.Now;

                ((IBaseEntity)entity.Entity).DateUpdated = DateTime.Now;
            }

            // _helpers.CompleteMethod();
            // Call the SaveChanges method on the context;
            return base.SaveChanges();
        }


        // See: http://bit.ly/2ZJTvbK
        // * Runs when you do _context.SaveChangesAsync();
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            // _helpers.OpenMethod(3);

            IEnumerable<EntityEntry> entities = ChangeTracker.Entries()
                .Where(
                    entityEntry => entityEntry.Entity is IBaseEntity && (
                        entityEntry.State == EntityState.Added ||
                        entityEntry.State == EntityState.Modified
                    )
                );

            foreach (EntityEntry entity in entities)
            {
                if (entity.State == EntityState.Added)
                    ((IBaseEntity)entity.Entity).DateCreated = DateTime.Now;

                ((IBaseEntity)entity.Entity).DateUpdated = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }


        // See:
        // * https://docs.microsoft.com/en-us/dotnet/api/system.data.entity.entitystate?view=entity-framework-6.2.0
        // STATES:
        // 1) ADD -> EntityState.Added
        // * Entity is being tracked by context but does not yet exist in db
        //
        // 2) DELETED -> EntityState.Deleted
        // * Entity is being tracked by context and exists in db, but has been marked for deletion from db next time SaveChanges is called.
        //
        // 3) DETACHED -> EntityState.Detached
        // * Entity is not being tracked by the context.
        // * An entity is in this state immediately after it has been created with the new operator or with one of the DbSet Create methods.
        //
        // 4) MODIFIED -> entityEntry.State == EntityState.Modified
        // * > Entity is being tracked by context and exists in db; some or all its property values have been modified.
        //
        // 5) UNCHANGED -> entityEntry.State == EntityState.Unchanged
        // * > Entity is being tracked by the context and exists in db, and its property values have not changed from the values in db.
        //



        private void SeedDatabase(ModelBuilder modelBuilder)
        {
            _helpers.OpenMethod(1);
            const string crunchTimePlayerBaseCsvFilePath = "BaseballData/00_SEED_DATA/_BASE_CrunchTime.csv";

            List<CrunchTimePlayerBase> listOfCrunchTimePlayerBases = _playerBaseController.GetAllToday_CSV(crunchTimePlayerBaseCsvFilePath);

            C.WriteLine("CrunchBase count : {listOfCrunchTimePlayerBases.Count}");

            modelBuilder.Entity<CrunchTimePlayerBase>().HasData(listOfCrunchTimePlayerBases);

            // const string fgSpWpdiSeedCsvFilePath = "BaseballData/00_SEED_DATA/FG_SP_wPDI.csv";

            // List<FanGraphsPitcherWpdi> listOfFanGraphsPitcherWpdi = _fangraphsSpController.GetAllSeed_CSV(fgSpWpdiSeedCsvFilePath, 2019);

            // modelBuilder.Entity<FanGraphsPitcherWpdi>().HasData(listOfFanGraphsPitcherWpdi);
        }



        public void PrintDatabaseAddOutcomes(int countAdded, int countNotAdded, Type type)
        {
            C.WriteLine($"\n-------------------------------------------------------------------");
            _helpers.PrintNameSpaceControllerNameMethodName(type);
            C.WriteLine($"ADDED TO DB   : {countAdded}");
            C.WriteLine($"ALREADY IN DB : {countNotAdded}");
            C.WriteLine($"-------------------------------------------------------------------\n");
        }

        public static void PrintDatabaseAddOutcomes(int countAdded, int countNotAdded)
        {
            C.WriteLine($"\n-------------------------------------------------------------------");
            C.WriteLine($"ADDED TO DB   : {countAdded}");
            C.WriteLine($"ALREADY IN DB : {countNotAdded}");
            C.WriteLine($"-------------------------------------------------------------------\n");
        }
    }


}


// DATABASE MIGRATIONS
// [ 1 ] update appsettings.Development.json first
// * i.e., change this "database=BS_08_05_2019_1" to today's date

// [ 2 ] Clear secrets; set new secrets (for DB info in appsettings files)
// * dotnet user-secrets clear
// * cat ./Configuration/appsettings.Development.json | dotnet user-secrets set// * cat ./Configuration/airtableConfiguration.json | dotnet user-secrets set

// [ 3 ] delete old Migrations folder

// [ 4 ] dotnet ef migrations add YourMigrationName
// * dotnet ef migrations add mig05302019_1 OR mig08_06_2019_1
// * dotnet ef migrations add MIG_08_29_2019_1

// [ 5 ] dotnet ef database update

// To add a table to an already migrated database, just do steps 4 and 5 (after you've added the DbSet to this file)


// POTENTIAL ERRORS
// [ A ] "Connection Refused" when trying to run dotnet ef database update
// * Make sure PostGres is running

// [ B ] help if issues with 42P07 (relation already exists) errors
// * https://weblog.west-wind.com/posts/2016/jan/13/resetting-entity-framework-migrations-to-a-clean-slate




// MANAGING SECRETS
// DBInfo from json file may be stored and read from user secrets
//
// [ A ] view all secrets:
// * dotnet user-secrets list
//
// [ B ] remove one secret:
// * dotnet user-secrets remove "DBInfo:ConnectionString"
//
// [ C ] set secret:
// * dotnet user-secrets set "DBInfo:ConnectionString" "server=localhost;userId=postgres;password=root;port=5432;database=BS_05302019_1;"
//
// See: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows
// See : https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=linux


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
