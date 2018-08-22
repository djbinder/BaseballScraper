// DJB this is right

using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    [XmlRoot (ElementName = "roster_adds")]
    public class YahooTeamRosterAdds
    {
        public int RosterAddsId { get; set; }

        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "coverage_value")]
        public string CoverageValue { get; set; }

        [XmlElement (ElementName = "value")]
        public string Value { get; set; }
    }
}