using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{

    [XmlRoot (ElementName = "roster", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class YahooRoster
    {
        [XmlElement (ElementName = "coverage_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Week { get; set; }

        [XmlElement (ElementName = "is_editable", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string IsEditable { get; set; }

        [XmlElement (ElementName = "players", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public List<YahooPlayer> PlayerList { get; set; }
    }
}