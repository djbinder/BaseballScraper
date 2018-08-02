// https://developer.yahoo.com/fantasysports/guide/game-resource.html

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    public class YahooGame
    {
        [Key]
        public int YahooGameId { get; set; }

        // e.g., NFL , MLB, NBA
        [XmlElement (ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string GameName { get; set; } //


        [XmlElement (ElementName = "game_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string GameId { get; set; } //


        [XmlElement (ElementName = "season", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Season { get; set; } //


        [XmlElement (ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string GameUri { get; set; } //


        [XmlElement (ElementName = "game_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string GameKey { get; set; } //


        [XmlElement (ElementName = "code", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string GameCode { get; set; } //


        [XmlElement (ElementName = "type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string GameType { get; set; } //


        // [XmlElement (ElementName = "game_weeks", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        // public GameWeekList GameWeeks { get; set; } //


        // [XmlElement (ElementName = "leagues", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        // public LeagueList LeagueList { get; set; } //


        // [XmlElement (ElementName = "players", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        // public PlayerList PlayerList { get; set; } //


        // [XmlElement (ElementName = "stat_categories", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        // public StatCategories StatCategories { get; set; } //


        // [XmlElement (ElementName = "position_types", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        // public PositionTypes PositionTypes { get; set; } //


        // [XmlElement (ElementName = "roster_positions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        // public RosterPositions RosterPositions { get; set; } //
    }
}