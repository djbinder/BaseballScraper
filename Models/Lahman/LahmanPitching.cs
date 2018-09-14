// http://www.seanlahman.com/files/database/readme58.txt

using CsvHelper.Configuration;

namespace BaseballScraper.Models.Lahman
{
public class LahmanPitching
{
    public string PlayerId { get; set; }
    public string Year { get; set; }
    public string Stint { get; set; }
    public string TeamId { get; set; }
    public string League { get; set; }
    public string Wins { get; set; }
    public string Losses { get; set; }
    public string Games { get; set; }
    public string GamesStarted { get; set; }
    public string CompleteGames { get; set; }
    public string ShutOuts { get; set; }
    public string Saves { get; set; }
    public string IpOuts { get; set; }
    public string HitsAgainst { get; set; }
    public string EarnedRuns { get; set; }
    public string HomeRuns { get; set; }
    public string Walks { get; set; }
    public string Strikeouts { get; set; }
    public string BattingAverageAgainst { get; set; }
    public string EarnedRunAverage { get; set; }
    public string IntentionalWalks { get; set; }
    public string WildPitchers { get; set; }
    public string HitByPitches { get; set; }
    public string Balks { get; set; }
    public string BattersFacedByPitcher { get; set; }
    public string GamesFinished { get; set; }
    public string RunsAllowed { get; set; }
    public string SacrificesAgainst { get; set; }
    public string SacrificeFliesAgainst { get; set; }
    public string DoublePlaysInduced { get; set; }
}

public class LahmanPitchingClassMap: ClassMap<LahmanPitching>
{
    public LahmanPitchingClassMap()
    {
        Map(m => m.PlayerId).Name("playerID");
        Map(m => m.Year).Name("yearID");
        Map(m => m.Stint).Name("stint");
        Map(m => m.TeamId).Name("teamID");
        Map(m => m.League).Name("lgID");
        Map(m => m.Wins).Name("W");
        Map(m => m.Losses).Name("L");
        Map(m => m.Games).Name("G");
        Map(m => m.GamesStarted).Name("GS");
        Map(m => m.CompleteGames).Name("CG");
        Map(m => m.ShutOuts).Name("SHO");
        Map(m => m.Saves).Name("SV");
        Map(m => m.IpOuts).Name("IPouts");
        Map(m => m.HitsAgainst).Name("H");
        Map(m => m.EarnedRuns).Name("ER");
        Map(m => m.HomeRuns).Name("HR");
        Map(m => m.Walks).Name("BB");
        Map(m => m.Strikeouts).Name("SO");
        Map(m => m.BattingAverageAgainst).Name("BAOpp");
        Map(m => m.EarnedRunAverage).Name("ERA");
        Map(m => m.IntentionalWalks).Name("IBB");
        Map(m => m.WildPitchers).Name("WP");
        Map(m => m.HitByPitches).Name("HBP");
        Map(m => m.Balks).Name("BK");
        Map(m => m.BattersFacedByPitcher).Name("BFP");
        Map(m => m.GamesFinished).Name("GF");
        Map(m => m.RunsAllowed).Name("R");
        Map(m => m.SacrificesAgainst).Name("SH");
        Map(m => m.SacrificeFliesAgainst).Name("SF");
        Map(m => m.DoublePlaysInduced).Name("GIDP");
    }
}

}