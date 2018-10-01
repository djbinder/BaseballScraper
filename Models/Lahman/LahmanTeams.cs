// http://www.seanlahman.com/files/database/readme58.txt

using CsvHelper.Configuration;

namespace BaseballScraper.Models.Lahman
{
    public class LahmanTeams
    {
        public string Season { get; set; }
        public string League { get; set; }
        public string LahmanTeamId { get; set; }
        public string FranchiseId { get; set; }
        public string Division { get; set; }
        public string PlaceInStandings { get; set; }
        public string Games { get; set; }
        public string HomeGames { get; set; }
        public string Wins { get; set; }
        public string Losses { get; set; }
        public string DidTeamWinDivision { get; set; }
        public string DidTeamWinWildCard  { get; set; }
        public string DidTeamWinLeague { get; set; }
        public string DidTeamWinWorldSeries { get; set; }
        public string Runs { get; set; }
        public string AtBats { get; set; }
        public string Hits { get; set; }
        public string Doubles  { get; set; }
        public string Triples { get; set; }
        public string HomeRuns { get; set; }
        public string Walks { get; set; }
        public string Strikeouts { get; set; }
        public string StolenBases { get; set; }
        public string CaughtStealing { get; set; }
        public string HitByPitches { get; set; }
        public string SacrificeFlies { get; set; }
        public string RunsAgainst { get; set; }
        public string EarnedRuns { get; set; }
        public string EarnedRunAverage { get; set; }
        public string CompleteGames { get; set; }
        public string Shutouts { get; set; }
        public string Saves { get; set; }
        public string IpOuts { get; set; }
        public string HitsAgainst { get; set; }
        public string HomeRunsAgainst { get; set; }
        public string WalksAgainst { get; set; }
        public string StrikeoutsAgainst { get; set; }
        public string Errors { get; set; }
        public string DoublePlays { get; set; }
        public string FieldingPercentage { get; set; }
        public string TeamName { get; set; }
        public string ParkName { get; set; }
        public string Attendance { get; set; }
        public string Batter3YearParkFactor { get; set; }
        public string Pitcher3YearParkFactors { get; set; }
        public string BaseballReferenceTeamId { get; set; }
        public string Lahman45TeamId { get; set; }
        public string RetroTeamId { get; set; }
    }


    public sealed class LahmanTeamsClassMap: ClassMap<LahmanTeams>
    {
        public LahmanTeamsClassMap()
        {
            Map( m => m.Season).Name("yearID");
            Map( m => m.League).Name("lgID");
            Map( m => m.LahmanTeamId).Name("teamID");
            Map( m => m.FranchiseId ).Name("franchID");
            Map( m => m.Division).Name("divID");
            Map( m => m.PlaceInStandings).Name("Rank");
            Map( m => m.Games).Name("G");
            Map( m => m.HomeGames).Name("Ghome");
            Map( m => m.Wins).Name("W");
            Map( m => m.Losses).Name("L");
            Map( m => m.DidTeamWinDivision).Name("DivWin");
            Map( m => m.DidTeamWinWildCard).Name("WCWin");
            Map( m => m.DidTeamWinLeague).Name("LgWin");
            Map( m => m.DidTeamWinWorldSeries).Name("WSWin");
            Map( m => m.Runs).Name("R");
            Map( m => m.AtBats).Name("AB");
            Map( m => m.Hits).Name("H");
            Map( m => m.Doubles ).Name("2B");
            Map( m => m.Triples).Name("3B");
            Map( m => m.HomeRuns).Name("HR");
            Map( m => m.Walks).Name("WSWin");
            Map( m => m.Strikeouts).Name("SO");
            Map( m => m.StolenBases).Name("SB");
            Map( m => m.CaughtStealing).Name("CS");
            Map( m => m.HitByPitches).Name("HBP");
            Map( m => m.SacrificeFlies).Name("SF");
            Map( m => m.RunsAgainst).Name("RA");
            Map( m => m.EarnedRuns).Name("ER");
            Map( m => m.EarnedRunAverage).Name("ERA");
            Map( m => m.CompleteGames).Name("CG");
            Map( m => m.Shutouts).Name("SHO");
            Map( m => m.Saves).Name("SV");
            Map( m => m.IpOuts).Name("IPouts");
            Map( m => m.HitsAgainst).Name("HA");
            Map( m => m.HomeRunsAgainst).Name("HRA");
            Map( m => m.WalksAgainst).Name("BBA");
            Map( m => m.StrikeoutsAgainst).Name("SOA");
            Map( m => m.Errors).Name("E");
            Map( m => m.DoublePlays).Name("DP");
            Map( m => m.FieldingPercentage).Name("FP");
            Map( m => m.TeamName ).Name("name");
            Map( m => m.ParkName).Name("park");
            Map( m => m.Attendance).Name("attendance");
            Map( m => m.Batter3YearParkFactor).Name("BPF");
            Map( m => m.Pitcher3YearParkFactors).Name("PPF");
            Map( m => m.BaseballReferenceTeamId).Name("teamIDBR");
            Map( m => m.Lahman45TeamId).Name("teamIDlahman45");
            Map( m => m.RetroTeamId).Name("teamIDretro");
        }
    }


    // public class LahmanTeam
    // {
    //     public int? yearID { get; set; }
    //     public string lgID { get; set; }
    //     public string teamID { get; set; }
    //     public string franchID { get; set; }
    //     public string divID { get; set; }
    //     public int? Rank { get; set; }
    //     public int? G { get; set; }
    //     public string Ghome { get; set; }
    //     public int? W { get; set; }
    //     public int? L { get; set; }
    //     public string DivWin { get; set; }
    //     public string WCWin  { get; set; }
    //     public string LgWin { get; set; }
    //     public string WSWin { get; set; }
    //     public int? R { get; set; }
    //     public int? AB { get; set; }
    //     public int? H { get; set; }
    //     // public int? 2B  { get; set; }
    //     // public int? 3B { get; set; }
    //     public int? HR { get; set; }
    //     public int? BB { get; set; }
    //     public int? SO { get; set; }
    //     public int? SB { get; set; }
    //     public int? CS { get; set; }
    //     public int? HBP { get; set; }
    //     public int? SF { get; set; }
    //     public int? RA { get; set; }
    //     public int? ER { get; set; }
    //     public decimal? ERA  { get; set; }
    //     public int? CG { get; set; }
    //     public int? SHO { get; set; }
    //     public int? SV { get; set; }
    //     public int? IPouts { get; set; }
    //     public int? HA { get; set; }
    //     public int? HRA { get; set; }
    //     public int? BBA { get; set; }
    //     public int? SOA { get; set; }
    //     public int? E { get; set; }
    //     public int? DP { get; set; }
    //     public decimal? FP { get; set; }
    //     public string name { get; set; }
    //     public string park { get; set; }
    //     public int? attendance { get; set; }
    //     public int? BPF { get; set; }
    //     public int? PPF { get; set; }
    //     public string teamIDBR { get; set; }
    //     public string teamIDlahman45 { get; set; }
    //     public string teamIDretro { get; set; }
    // }
}