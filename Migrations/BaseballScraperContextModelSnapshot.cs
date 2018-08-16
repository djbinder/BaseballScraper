﻿// <auto-generated />
using System;
using BaseballScraper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BaseballScraper.Migrations
{
    [DbContext(typeof(BaseballScraperContext))]
    partial class BaseballScraperContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("BaseballScraper.Models.Yahoo.RosterAdds", b =>
                {
                    b.Property<int>("RosterAddsId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CoverageType");

                    b.Property<string>("CoverageValue");

                    b.Property<string>("Value");

                    b.HasKey("RosterAddsId");

                    b.ToTable("RosterAdds");
                });

            modelBuilder.Entity("BaseballScraper.Models.Yahoo.TeamLogo", b =>
                {
                    b.Property<int>("TeamLogoId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Size");

                    b.Property<string>("Url");

                    b.HasKey("TeamLogoId");

                    b.ToTable("TeamLogo");
                });

            modelBuilder.Entity("BaseballScraper.Models.Yahoo.TeamManagers", b =>
                {
                    b.Property<int>("TeamManagersId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Guid");

                    b.Property<string>("ImageUrl");

                    b.Property<int?>("IsCommissioner");

                    b.Property<int?>("IsCurrentLogin");

                    b.Property<string>("ManagerId");

                    b.Property<string>("NickName");

                    b.Property<int?>("YahooTeamBaseId");

                    b.HasKey("TeamManagersId");

                    b.HasIndex("YahooTeamBaseId");

                    b.ToTable("TeamManagers");
                });

            modelBuilder.Entity("BaseballScraper.Models.Yahoo.YahooTeamBase", b =>
                {
                    b.Property<int>("YahooTeamBaseId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HasDraftGrade");

                    b.Property<int?>("IsOwnedByCurrentLogin");

                    b.Property<string>("Key");

                    b.Property<string>("LeagueScoringType");

                    b.Property<int?>("NumberOfMoves");

                    b.Property<int?>("NumberOfTrades");

                    b.Property<int?>("RosterAddsId");

                    b.Property<int?>("TeamId");

                    b.Property<int?>("TeamLogoId");

                    b.Property<int?>("TeamManagersId");

                    b.Property<string>("TeamName");

                    b.Property<string>("Url");

                    b.Property<int?>("WaiverPriority");

                    b.HasKey("YahooTeamBaseId");

                    b.HasIndex("RosterAddsId");

                    b.HasIndex("TeamLogoId");

                    b.HasIndex("TeamManagersId");

                    b.ToTable("YahooTeamBase");
                });

            modelBuilder.Entity("BaseballScraper.Models.Yahoo.TeamManagers", b =>
                {
                    b.HasOne("BaseballScraper.Models.Yahoo.YahooTeamBase")
                        .WithMany("ManagersList")
                        .HasForeignKey("YahooTeamBaseId");
                });

            modelBuilder.Entity("BaseballScraper.Models.Yahoo.YahooTeamBase", b =>
                {
                    b.HasOne("BaseballScraper.Models.Yahoo.RosterAdds", "RosterAdds")
                        .WithMany()
                        .HasForeignKey("RosterAddsId");

                    b.HasOne("BaseballScraper.Models.Yahoo.TeamLogo", "TeamLogo")
                        .WithMany()
                        .HasForeignKey("TeamLogoId");

                    b.HasOne("BaseballScraper.Models.Yahoo.TeamManagers", "TeamManager")
                        .WithMany()
                        .HasForeignKey("TeamManagersId");
                });
#pragma warning restore 612, 618
        }
    }
}
