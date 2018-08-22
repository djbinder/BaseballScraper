// DJB this is right

using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{

    [XmlRoot (ElementName = "team_logos")]
    public class YahooTeamLogos
    {
        public YahooTeamLogo TeamLogo { get; set; }
    }

    [XmlRoot (ElementName = "team_logo")]
    public class YahooTeamLogo
    {
        public int TeamLogoId { get; set; }

        [XmlElement (ElementName = "size")]
        public string Size { get; set; }

        [XmlElement (ElementName = "url")]
        public string Url { get; set; }

    }
}