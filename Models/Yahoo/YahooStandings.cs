using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{


    [XmlRoot (ElementName = "team_points")]
    public class TeamPoints
    {
        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "season")]
        public string Season { get; set; }

        [XmlElement (ElementName = "total")]
        public string Total { get; set; }
    }



    [XmlRoot (ElementName = "streak")]
    public class Streak
    {
        [XmlElement (ElementName = "type")]
        public string Type { get; set; }

        [XmlElement (ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot (ElementName = "team_standings")]
    public class TeamStandings
    {
        [XmlElement (ElementName = "rank")]
        public string Rank { get; set; }

        [XmlElement (ElementName = "playoff_seed")]
        public string PlayoffSeed { get; set; }



        [XmlElement (ElementName = "streak")]
        public Streak Streak { get; set; }

        [XmlElement (ElementName = "points_for")]
        public string PointsFor { get; set; }

        [XmlElement (ElementName = "points_against")]
        public string PointsAgainst { get; set; }
    }

    // [XmlRoot (ElementName = "standings")]
    // public class Standings
    // {
    //     [XmlElement (ElementName = "teams")]
    //     public List<YahooTeam> TeamList { get; set; }
    // }
}
