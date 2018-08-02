using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    [XmlRoot (ElementName = "player", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class YahooPlayer
    {
        [XmlElement (ElementName = "yahoo_full_name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string YahooFullName { get; set; }


        [XmlElement (ElementName = "player_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PlayerKey { get; set; }


        [XmlElement (ElementName = "player_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PlayerId { get; set; }


        [XmlElement (ElementName = "image_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string ImageUrl { get; set; }


        [XmlElement (ElementName = "image_size", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string ImageSize { get; set; }


        [XmlElement (ElementName = "is_undroppable", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string IsUndroppable { get; set; }


        [XmlElement (ElementName = "position_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PositionType { get; set; }


        [XmlElement (ElementName = "eligible_positions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public List<string> EligiblePositions { get; set; }


        [XmlElement (ElementName = "status", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Status { get; set; }


        [XmlElement (ElementName = "status_full", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string StatusFull { get; set; }


        [XmlElement (ElementName = "injury_note", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string InjuryNote { get; set; }

        [XmlElement (ElementName = "has_player_notes", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string HasPlayerNotes { get; set; }


        [XmlElement (ElementName = "transaction_data")]
        public TransactionData TransactionData { get; set; }


        [XmlElement (ElementName = "selected_position")]
        public SelectedPosition SelectedPosition { get; set; }


        [XmlElement (ElementName = "player_stats", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public YahooStats PlayerStats { get; set; }



    }
}