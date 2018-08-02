using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{

    [XmlRoot (ElementName = "game_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class YahooMatchup
    {
        [XmlElement (ElementName = "week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Week { get; set; }


        [XmlElement (ElementName = "start", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Start { get; set; }


        [XmlElement (ElementName = "end", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string End { get; set; }


        [XmlElement (ElementName = "manager", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public List<YahooManager> Managers { get; set; }


        [XmlElement (ElementName = "matchup_winner", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public YahooManager WinningManager { get; set; }


        [XmlElement (ElementName = "matchup_loser", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public YahooManager LosingManager { get; set; }
    }
}
