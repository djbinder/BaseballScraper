// // DJB WORKING ON

// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.Xml.Serialization;

// namespace BaseballScraper.Models.Yahoo
// {
//     [XmlRoot (ElementName = "roster", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]

//     // Roster for a particular week: /fantasy/v2/team/{team_key}/roster;week={week}
//     public class YahooTeamRoster
//     {
//         [Key]
//         public int YahooTeamRosterRecordId { get; set; }

//         [XmlElement (ElementName = "coverage_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
//         public string RosterCoverageType { get ; set; }


//         [XmlElement (ElementName = "data", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
//         public string Date { get ; set; }


//         [XmlElement (ElementName = "is_editable", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
//         public string IsEditable { get ; set; }


//         [XmlElement (ElementName = "@count")]
//         public string Count { get ; set; }


//         public IList<YahooTeamRosterPlayer> Players { get; set; }

//         public YahooTeamRoster ()
//         {
//             Players = new List<YahooTeamRosterPlayer>();
//         }
//     }


//     public class YahooTeamRosterPlayer
//     {

//         [XmlElement (ElementName = "selected_position")]
//         public YahooPlayerSelectedPosition PlayersSelectedPosition { get; set; }
//         public PlayerName PlayerName { get; set; }
//         public Headshot Headshot { get; set; }


//         [XmlElement (ElementName = "coverage_type")]
//         public string StartingStatusCoverageType { get; set; }


//         [XmlElement (ElementName = "date")]
//         public string StartingStatusDate { get; set; }


//         [XmlElement (ElementName = "is_starting")]
//         public string IsStartingToday { get; set; }


//         [XmlElement (ElementName = "order_num")]
//         public string BattingOrderNumber { get; set; }


//         [XmlElement (ElementName = "is_editable")]
//         public string IsEditable { get; set; }


//         public YahooTeamRosterPlayer ()
//         {
//             PlayerName              = new PlayerName();
//             Headshot                = new Headshot();
//             PlayersSelectedPosition = new YahooPlayerSelectedPosition();
//         }
//     }

//     // the roster position of an mlb player on yahoo managers roster (e.g., Paul Goldschmidt is at 1B)
//     [XmlRoot(ElementName = "selected_position")]
//     public class YahooPlayerSelectedPosition
//     {
//         [XmlElement(ElementName = "coverage_type")]
//         public string CoverageType { get; set; }

//         [XmlElement(ElementName = "date")]
//         public string Date { get; set; }

//         [XmlElement(ElementName = "position")]
//         public string Position { get; set; }
//     }
// }
