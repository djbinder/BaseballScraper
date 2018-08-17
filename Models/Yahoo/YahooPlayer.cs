using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    [XmlRoot (ElementName = "player", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class YahooPlayerBase
    {
        [XmlElement (ElementName = "player_key")]
        public string PlayerKey { get; set; }


        [XmlElement (ElementName = "player_id")]
        public string PlayerId { get; set; }


        [XmlElement (ElementName = "name")]
        public PlayerName PlayerName { get; set; }


        [XmlElement (ElementName = "editorial_player_key")]
        public string EditorialPlayerKey { get; set; }


        [XmlElement (ElementName = "editorial_team_key")]
        public string EditorialTeamKey { get; set; }


        [XmlElement (ElementName = "editorial_team_full_name")]
        public string EditorialTeamFullName { get; set; }


        [XmlElement (ElementName = "editorial_team_abbr")]
        public string EditorialTeamAbbreviation { get; set; }


        [XmlElement (ElementName = "uniform_number")]
        public string UniformNumber { get; set; }


        [XmlElement (ElementName = "display_position")]
        public string DisplayPosition { get; set; }


        [XmlElement(ElementName = "headshot", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public Headshot Headshot { get; set; }


        [XmlElement (ElementName = "image_url")]
        public string ImageUrl { get; set; }


        [XmlElement (ElementName = "is_undroppable")]
        public string IsUndroppable { get; set; }


        [XmlElement (ElementName = "position_type")]
        public string PositionType { get; set; }


        [XmlElement (ElementName = "eligible_positions")]
        public string EligiblePositions { get; set; }


        [XmlElement (ElementName = "has_player_notes")]
        public string HasPlayerNotes { get; set; }

    }

    [XmlRoot (ElementName = "name")]
    public class PlayerName
    {
        [XmlElement (ElementName = "full")]
        public string FullName { get; set; }

        [XmlElement (ElementName = "first")]
        public string FirstName { get; set; }

        [XmlElement (ElementName = "last")]
        public string LastName { get; set; }

        [XmlElement (ElementName = "ascii_first")]
        public string AsciiFirstName { get; set; }

        [XmlElement (ElementName = "ascii_last")]
        public string AsciiLastName { get; set; }
    }


    [XmlRoot(ElementName = "headshot", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class Headshot
    {
        [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Url { get; set; }

        [XmlElement(ElementName = "size", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Size { get; set; }
    }
}