// http://www.seanlahman.com/files/database/readme58.txt


using CsvHelper.Configuration;

#pragma warning disable MA0048
namespace BaseballScraper.Models.Lahman
{
public class LahmanBatting
{
    public string LahmanPlayerId { get; set; }
    public string Season { get; set; }
    public string Stint { get; set; }
    public string LahmanTeamId { get; set; }
    public string League { get; set; }
    public string Games { get; set; }
    public string AtBats { get; set; }
    public string Runs { get; set; }
    public string Hits { get; set; }
    public string Doubles { get; set; }
    public string Triples { get; set; }
    public string HomeRuns { get; set; }
    public string RunsBattedIn { get; set; }
    public string StolenBases { get; set; }
    public string CaughtStealing { get; set; }
    public string Walks { get; set; }
    public string Strikeouts { get; set; }
    public string IntentionalWalks { get; set; }
    public string HitByPitches { get; set; }
    public string SacrificeHits { get; set; }
    public string SacrificeFlies { get; set; }
    public string GroundedIntoDoublePlays { get; set; }
}

public sealed class LahmanBattingClassMap: ClassMap<LahmanBatting>
{
    public LahmanBattingClassMap()
    {
        Map(m => m.LahmanPlayerId).Name("playerID");
        Map(m => m.Season).Name("yearID");
        Map(m => m.Stint).Name("stint");
        Map(m => m.LahmanTeamId).Name("teamID");
        Map(m => m.League).Name("lgID");
        Map(m => m.Games).Name("G");
        Map(m => m.AtBats).Name("AB");
        Map(m => m.Runs).Name("R");
        Map(m => m.Hits).Name("H");
        Map(m => m.Doubles).Name("2B");
        Map(m => m.Triples).Name("3B");
        Map(m => m.HomeRuns).Name("HR");
        Map(m => m.RunsBattedIn).Name("RBI");
        Map(m => m.StolenBases).Name("SB");
        Map(m => m.CaughtStealing).Name("CS");
        Map(m => m.Walks).Name("BB");
        Map(m => m.Strikeouts).Name("SO");
        Map(m => m.IntentionalWalks).Name("IBB");
        Map(m => m.HitByPitches).Name("HBP");
        Map(m => m.SacrificeHits).Name("SH");
        Map(m => m.SacrificeFlies).Name("SF");
        Map(m => m.GroundedIntoDoublePlays).Name("GIDP");
    }
}

}