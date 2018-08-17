// DJB WORKING ON

using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
        // /fantasy/v2/team/{team_key}/standings
    public class YahooTeamStandings: YahooTeamBase
    {
        [XmlElement (ElementName = "rank")]
        public string Rank { get; set; }

        [XmlElement (ElementName = "playoff_seed")]
        public string PlayoffSeed { get; set; }

        [XmlElement (ElementName = "outcome_totals")]
        public YahooMatchupOutcomeTotals OutcomeTotals { get; set; }

        [XmlElement (ElementName = "games_back")]
        public string GamesBack { get; set; }

        public YahooTeamStandings()
        {
            OutcomeTotals = new YahooMatchupOutcomeTotals();
        }
    }


    [XmlRoot (ElementName = "team_points")]
    public class YahooTeamWeekPoints
    {
        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "week")]
        public int? Week { get; set; }

        [XmlElement (ElementName = "total")]
        public double? Total { get; set; }
    }



    // [XmlRoot (ElementName = "streak")]
    // public class Streak
    // {
    //     [XmlElement (ElementName = "type")]
    //     public string Type { get; set; }

    //     [XmlElement (ElementName = "value")]
    //     public string Value { get; set; }
    // }


}
