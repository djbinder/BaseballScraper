// DJB WORKING ON

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


// Season stats: /fantasy/v2/team/{team_key}/stats
namespace BaseballScraper.Models.Yahoo
{
    public enum SortOrder
    {
        [XmlEnum ("0")] Asc = 0, [XmlEnum ("1")] Desc = 1
    }

    [XmlRoot (ElementName = "stat", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class YahooStats
    {
        [XmlElement (ElementName = "stat_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string StatId { get; set; }


        [XmlElement (ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Name { get; set; }


        [XmlElement (ElementName = "display_name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string DisplayName { get; set; }


        [XmlElement (ElementName = "sort_order", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public SortOrder SortOrder { get; set; }


        // [XmlElement (ElementName = "position_types", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        // public PositionTypes PositionTypes { get; set; }


        [XmlElement (ElementName = "position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PositionType { get; set; }

    }

    public class YahooTeamStats: YahooTeamBase
    {
        public string Season { get; set; }

        [XmlElement (ElementName = "coverage_type")]
        public string StatCoverageType { get; set; }

        [XmlElement (ElementName = "week")]
        public string WeekNumber { get; set; }

        public string HitsAtBatsId { get; set; } = "60";
        public string HitsAtBatsTotal { get; set; }

        public string RunsId { get; set; } = "7";
        public string RunsTotal { get; set; }

        public string HomeRunsId { get; set; } = "12";
        public string HomeRunsTotal { get; set; }

        public string RbiId { get; set; } = "13";
        public string RbiTotal { get; set; }

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

        public string EraId { get; set; } = "26";
        public string EraTotal { get; set; }

        public string WhipId { get; set; } = "27";
        public string WhipTotal { get; set; }

        public YahooTeamWeekPoints TeamWeekPoints { get; set; }

        public IList<YahooTeamStat> TeamStats { get; set; }

        public YahooTeamStats()
        {
            TeamStats      = new List<YahooTeamStat>();
            TeamWeekPoints = new YahooTeamWeekPoints();
        }
    }


    [XmlRoot (ElementName = "team_stats")]
    public class YahooTeamStat
    {
        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "week")]
        public int Week { get; set; }

        [XmlElement (ElementName = "stats")]
        public YahooStats Stats { get; set; }

        public YahooTeamStat()
        {
            Stats = new YahooStats();
        }
    }
}
