using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using BaseballScraper.Infrastructure;
using Newtonsoft.Json;

namespace BaseballScraper.Models.Yahoo.Resources.YahooPlayerResource
{
    // https://developer.yahoo.com/fantasysports/guide/#player-resource

    [XmlRoot (ElementName = "player", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class YahooPlayerResource
    {
        [Key]
        [XmlElement (ElementName = "player_key")]
        [JsonProperty("player_key")]
        public string PlayerKey { get; set; }


        [XmlElement (ElementName = "player_id")]
        [JsonProperty("player_id")]
        public string PlayerId { get; set; }


        [XmlElement (ElementName = "name")]
        [JsonProperty("name")]
        public PlayerName PlayerName { get; set; }


        [XmlElement (ElementName = "editorial_player_key")]
        [JsonProperty("editorial_player_key")]
        public string EditorialPlayerKey { get; set; }


        [XmlElement (ElementName = "editorial_team_key")]
        [JsonProperty("editorial_team_key")]
        public string EditorialTeamKey { get; set; }


        [XmlElement (ElementName = "editorial_team_full_name")]
        [JsonProperty("editorial_team_full_name")]
        public string EditorialTeamNameFull { get; set; }


        [XmlElement (ElementName = "editorial_team_abbr")]
        [JsonProperty("editorial_team_abbr")]
        public string EditorialTeamNameAbbreviation { get; set; }


        [XmlElement (ElementName = "uniform_number")]
        [JsonProperty("uniform_number")]
        public string UniformNumber { get; set; }


        [XmlElement (ElementName = "display_position")]
        [JsonProperty("display_position")]
        public string DisplayPosition { get; set; }


        [XmlElement(ElementName = "headshot", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        [JsonProperty("headshot")]
        public Headshot Headshot { get; set; }


        [XmlElement (ElementName = "image_url")]
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }


        [XmlElement (ElementName = "is_undroppable")]
        [JsonProperty("is_undroppable")]
        public string IsUndroppable { get; set; }


        [XmlElement (ElementName = "position_type")]
        [JsonProperty("position_type")]
        public string PositionType { get; set; }


        [XmlElement (ElementName = "eligible_positions")]
        [JsonProperty("eligible_positions")]
        public EligiblePositions EligiblePositions { get; set; }


        [XmlElement (ElementName = "has_player_notes")]
        [JsonProperty("has_player_notes")]
        public string HasPlayerNotes { get; set; }


        [XmlElement (ElementName = "player_notes_last_timestamp")]
        [JsonProperty("player_notes_last_timestamp")]
        public string PlayerNotesLastTimestamp { get; set; }
    }


    [XmlRoot (ElementName = "name")]
    public partial class PlayerName
    {
        [XmlElement (ElementName = "full")]
        [JsonProperty("full")]
        public string FullName { get; set; }


        [XmlElement (ElementName = "first")]
        [JsonProperty("first")]
        public string FirstName { get; set; }


        [XmlElement (ElementName = "last")]
        [JsonProperty("last")]
        public string LastName { get; set; }


        [XmlElement (ElementName = "ascii_first")]
        [JsonProperty("ascii_first")]
        public string AsciiFirstName { get; set; }


        [XmlElement (ElementName = "ascii_last")]
        [JsonProperty("ascii_last")]
        public string AsciiLastName { get; set; }
    }


    [XmlRoot(ElementName = "headshot", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class Headshot
    {
        [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        [JsonProperty("url")]
        public string Url { get; set; }


        [XmlElement(ElementName = "size", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        [JsonProperty("size")]
        public string Size { get; set; }
    }


    public partial class EligiblePositions
    {
        [XmlElement (ElementName = "position")]
        [JsonProperty("position")]
        public List<string> Position { get; set; }
    }
}
