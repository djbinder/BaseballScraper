using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;



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


        [XmlElement (ElementName = "position_types", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public PositionTypes PositionTypes { get; set; }


        [XmlElement (ElementName = "position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PositionType { get; set; }

    }
}
