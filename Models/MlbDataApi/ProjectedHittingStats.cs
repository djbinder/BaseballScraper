// https://appac.github.io/mlb-data-api-docs/#stats-data-projected-hitting-stats-get


using System.Runtime.Serialization;

namespace BaseballScraper.Models.MlbDataApi
{
    [DataContract]
    public class ProjectedHittingStats
    {
        [DataMember(Name="hr")]
        public int? HomeRuns { get; set; }

        [DataMember(Name="gidp")]
        public int? Gidp { get; set; }

        [DataMember(Name="player")]
        public string Player { get; set; }

        [DataMember(Name="sac")]
        public int? Sacrifices { get; set; }

        [DataMember(Name="rbi")]
        public int? Rbi { get; set; }

        [DataMember(Name="tb")]
        public int? TotalBases { get; set; }

        [DataMember(Name="slg")]
        public decimal? SluggingPercentage { get; set; }

        [DataMember(Name="avg")]
        public decimal? BattingAverage { get; set; }

        [DataMember(Name="bb")]
        public int? Walks { get; set; }

        [DataMember(Name="ops")]
        public decimal? Ops { get; set; }

        [DataMember(Name="hbp")]
        public int? Hbp { get; set; }

        [DataMember(Name="g")]
        public int? Games { get; set; }

        [DataMember(Name="d")]
        public int? Doubles { get; set; }

        [DataMember(Name="e")]
        public int? Errors { get; set; }

        [DataMember(Name="so")]
        public int? Strikeouts { get; set; }

        [DataMember(Name="sf")]
        public int? SacrificeFlys { get; set; }

        [DataMember(Name="tpa")]
        public int? TotalPlateAppearances { get; set; }

        [DataMember(Name="h")]
        public int? Hits { get; set; }

        [DataMember(Name="cs")]
        public int? CaughtStealing { get; set; }

        [DataMember(Name="obp")]
        public decimal? Obp { get; set; }

        [DataMember(Name="t")]
        public int? T { get; set; }

        [DataMember(Name="s")]
        public int? S { get; set; }

        [DataMember(Name="r")]
        public int? Runs { get; set; }

        [DataMember(Name="sb")]
        public int? StolenBases { get; set; }

        [DataMember(Name="sbpct")]
        public decimal? StolenBasePercentage { get; set; }

        [DataMember(Name="player_id")]
        public int? PlayerId { get; set; }

        [DataMember(Name="ab")]
        public int? AtBats { get; set; }

        [DataMember(Name="ibb")]
        public int? IntentionalWalks { get; set; }
    }
}