// DJB this is right

using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{

    [XmlRoot (ElementName = "team_points")]
    public class YahooTeamPoints
    {
        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "week")]
        public int? WeekOrYear { get; set; }

        [XmlElement (ElementName = "total")]
        public double? Total { get; set; }
    }
}