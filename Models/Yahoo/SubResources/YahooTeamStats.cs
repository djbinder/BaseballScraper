using System.Xml.Serialization;


// Season stats: /fantasy/v2/team/{team_key}/stats
#pragma warning disable MA0016, MA0048
namespace BaseballScraper.Models.Yahoo
{
    public class YahooTeamStats : BaseEntity
    {
        public string Season { get; set; }


        // [XmlElement (ElementName = "coverage_type")]
        public string StatCoverageType { get; set; }


        [XmlElement (ElementName = "week")]
        public string WeekNumber { get; set; }
    }

    public class YahooTeamStatsList
    {
        public YahooTeamStats YahooTeamStats { get; set; }
        public string TeamKey { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }

        public string ManagerId { get; set; }
        public string ManagerNickName { get; set; }
        public string ManagerGuid { get; set; }

        public string HitsDividedByAtBatsId { get; set; } = "60";
        public string HitsDividedByAtBatsTotal { get; set; }

        public string RunsId { get; set; } = "7";
        public string RunsTotal { get; set; }

        public string HomeRunsId { get; set; } = "12";
        public string HomeRunsTotal { get; set; }

        public string RunsBattedInId { get; set; } = "13";
        public string RunsBattedInTotal { get; set; }

        public string StolenBasesId { get; set; } = "16";
        public string StolenBasesTotal { get; set; }

        public string WalksId { get; set; } = "18";
        public string WalksTotal { get; set; }

        public string BattingAverageId { get; set; } = "3";
        public string BattingAverageTotal { get; set; }

        public string InningsPitchedId { get; set; } = "50";
        public string InningsPitchedTotal { get; set; }

        public string WinsId { get; set; } = "28";
        public string WinsTotal { get; set; }

        public string StrikeoutsId { get; set; } = "32";
        public string StrikeoutsTotal { get; set; }

        public string SavesId { get; set; } = "42";
        public string SavesTotal { get; set; }

        public string HoldsId { get; set; } = "48";
        public string HoldsTotal { get; set; }

        public string EarnedRunAverageId { get; set; } = "26";
        public string EarnedRunAverageTotal { get; set; }

        public string WhipId { get; set; } = "27";
        public string WhipTotal { get; set; }
    }

    public class YahooStatPair
    {
        public string StatId { get; set; }
        public string StatValue { get; set; }
    }
}
