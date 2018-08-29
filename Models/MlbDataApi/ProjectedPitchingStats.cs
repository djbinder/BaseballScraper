// SOURCE
    // https://appac.github.io/mlb-data-api-docs/#stats-data-projected-pitching-stats-get

using System.Runtime.Serialization;

namespace BaseballScraper.Models.MlbDataApi
{
    [DataContract]
    public class ProjectedPitchingStats
    {
        [DataMember(Name="hr")]
        public int? Hr { get; set; }

        [DataMember(Name="player")]
        public string Player { get; set; }

        [DataMember(Name="wpct")]
        public decimal? WinningPercentage { get; set; }

        [DataMember(Name="era")]
        public decimal? Era { get; set; }

        [DataMember(Name="bsv")]
        public string BlownSaves { get; set; }

        [DataMember(Name="outs")]
        public int? Outs { get; set; }

        [DataMember(Name="sho")]
        public int ShutOuts { get; set; }

        [DataMember(Name="sv")]
        public int? Saves { get; set; }

        [DataMember(Name="whip")]
        public decimal? Whip { get; set; }

        [DataMember(Name="qs")]
        public int? QualityStarts { get; set; }

        [DataMember(Name="bb")]
        public int? Walks { get; set; }

        [DataMember(Name="g")]
        public int? Games { get; set; }

        [DataMember(Name="hld")]
        public int? Holds { get; set; }

        [DataMember(Name="so")]
        public int? Strikeouts { get; set; }

        [DataMember(Name="l")]
        public int? Losses { get; set; }

        [DataMember(Name="hb")]
        public int? HitBatters { get; set; }

        [DataMember(Name="svo")]
        public int? SaveOpportunities { get; set; }

        [DataMember(Name="h")]
        public int? Hits { get; set; }

        [DataMember(Name="ip")]
        public decimal? InningsPitched { get; set; }

        [DataMember(Name="w")]
        public int? Wins { get; set; }

        [DataMember(Name="r")]
        public int? RunsAgainst { get; set; }

        [DataMember(Name="pa")]
        public int? PlateAppearances { get; set; }

        [DataMember(Name="player_id")]
        public int? PlayerId { get; set; }

        [DataMember(Name="cg")]
        public int? CompleteGames { get; set; }

        [DataMember(Name="gs")]
        public int? GamesStarted { get; set; }

        [DataMember(Name="ibb")]
        public int? IntentionalWalks { get; set; }

        [DataMember(Name="er")]
        public int? EarnedRuns { get; set; }
    }
}