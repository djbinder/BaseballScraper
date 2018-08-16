using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BaseballScraper.Migrations
{
    public partial class mig08151 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RosterAdds",
                columns: table => new
                {
                    RosterAddsId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CoverageType = table.Column<string>(nullable: true),
                    CoverageValue = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterAdds", x => x.RosterAddsId);
                });

            migrationBuilder.CreateTable(
                name: "TeamLogo",
                columns: table => new
                {
                    TeamLogoId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Size = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamLogo", x => x.TeamLogoId);
                });

            migrationBuilder.CreateTable(
                name: "YahooTeamBase",
                columns: table => new
                {
                    YahooTeamBaseId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Key = table.Column<string>(nullable: true),
                    TeamName = table.Column<string>(nullable: true),
                    TeamId = table.Column<int>(nullable: true),
                    IsOwnedByCurrentLogin = table.Column<int>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    TeamLogoId = table.Column<int>(nullable: true),
                    WaiverPriority = table.Column<int>(nullable: true),
                    NumberOfMoves = table.Column<int>(nullable: true),
                    NumberOfTrades = table.Column<int>(nullable: true),
                    RosterAddsId = table.Column<int>(nullable: true),
                    LeagueScoringType = table.Column<string>(nullable: true),
                    HasDraftGrade = table.Column<string>(nullable: true),
                    TeamManagersId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YahooTeamBase", x => x.YahooTeamBaseId);
                    table.ForeignKey(
                        name: "FK_YahooTeamBase_RosterAdds_RosterAddsId",
                        column: x => x.RosterAddsId,
                        principalTable: "RosterAdds",
                        principalColumn: "RosterAddsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YahooTeamBase_TeamLogo_TeamLogoId",
                        column: x => x.TeamLogoId,
                        principalTable: "TeamLogo",
                        principalColumn: "TeamLogoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamManagers",
                columns: table => new
                {
                    TeamManagersId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ManagerId = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    Guid = table.Column<string>(nullable: true),
                    IsCommissioner = table.Column<int>(nullable: true),
                    IsCurrentLogin = table.Column<int>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    YahooTeamBaseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamManagers", x => x.TeamManagersId);
                    table.ForeignKey(
                        name: "FK_TeamManagers_YahooTeamBase_YahooTeamBaseId",
                        column: x => x.YahooTeamBaseId,
                        principalTable: "YahooTeamBase",
                        principalColumn: "YahooTeamBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamManagers_YahooTeamBaseId",
                table: "TeamManagers",
                column: "YahooTeamBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_YahooTeamBase_RosterAddsId",
                table: "YahooTeamBase",
                column: "RosterAddsId");

            migrationBuilder.CreateIndex(
                name: "IX_YahooTeamBase_TeamLogoId",
                table: "YahooTeamBase",
                column: "TeamLogoId");

            migrationBuilder.CreateIndex(
                name: "IX_YahooTeamBase_TeamManagersId",
                table: "YahooTeamBase",
                column: "TeamManagersId");

            migrationBuilder.AddForeignKey(
                name: "FK_YahooTeamBase_TeamManagers_TeamManagersId",
                table: "YahooTeamBase",
                column: "TeamManagersId",
                principalTable: "TeamManagers",
                principalColumn: "TeamManagersId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamManagers_YahooTeamBase_YahooTeamBaseId",
                table: "TeamManagers");

            migrationBuilder.DropTable(
                name: "YahooTeamBase");

            migrationBuilder.DropTable(
                name: "RosterAdds");

            migrationBuilder.DropTable(
                name: "TeamLogo");

            migrationBuilder.DropTable(
                name: "TeamManagers");
        }
    }
}
