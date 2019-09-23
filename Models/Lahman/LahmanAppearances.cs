// http://www.seanlahman.com/files/database/readme58.txt

using CsvHelper.Configuration;

#pragma warning disable MA0048
namespace BaseballScraper.Models.Lahman
{
    public class LahmanAppearances
    {
        public string Season { get; set; }
        public string LahmanTeamId { get; set; }
        public string League { get; set; }
        public string LahmanPlayerId { get; set; }
        public string TotalGamesPlayed { get; set; }
        public string GamesStarted { get; set; }
        public string GamesWhenPlayerBatted { get; set; }
        public string GamesWhenPlayerPlayedDefense { get; set; }
        public string GamesAsPitcher { get; set; }
        public string GamesAsCatcher { get; set; }
        public string GamesAs1B { get; set; }
        public string GamesAs2B { get; set; }
        public string GamesAs3B { get; set; }
        public string GamesAsSS { get; set; }
        public string GamesAsLF { get; set; }
        public string GamesAsCF { get; set; }
        public string GamesAsRF { get; set; }
        public string GamesAsOF { get; set; }
        public string GamesAsDH { get; set; }
        public string GamesAsPinchHitter { get; set; }
        public string GamesAsPinchRunner { get; set; }
    }

    public sealed class LahmanAppearancesClassMap: ClassMap<LahmanAppearances>
    {
        public LahmanAppearancesClassMap()
        {
            Map(m => m.Season).Name("yearID");
            Map(m => m.LahmanTeamId).Name("teamID");
            Map(m => m.League).Name("lgID");
            Map(m => m.LahmanPlayerId).Name("playerID");
            Map(m => m.TotalGamesPlayed).Name("G_all");
            Map(m => m.GamesStarted).Name("GS");
            Map(m => m.GamesWhenPlayerBatted).Name("G_batting");
            Map(m => m.GamesWhenPlayerPlayedDefense).Name("G_defense");
            Map(m => m.GamesAsPitcher).Name("G_p");
            Map(m => m.GamesAsCatcher).Name("G_c");
            Map(m => m.GamesAs1B).Name("G_1b");
            Map(m => m.GamesAs2B).Name("G_2b");
            Map(m => m.GamesAs3B).Name("G_3b");
            Map(m => m.GamesAsSS).Name("G_ss");
            Map(m => m.GamesAsLF).Name("G_lf");
            Map(m => m.GamesAsCF).Name("G_cf");
            Map(m => m.GamesAsRF).Name("G_rf");
            Map(m => m.GamesAsOF).Name("G_of");
            Map(m => m.GamesAsDH).Name("G_dh");
            Map(m => m.GamesAsPinchHitter).Name("G_ph");
            Map(m => m.GamesAsPinchRunner).Name("G_pr");
        }
    }
}