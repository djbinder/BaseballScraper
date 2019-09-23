// DJB WORKING ON

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006, MA0016, MA0048
namespace BaseballScraper.Models.Yahoo
{
    [XmlRoot (ElementName = "matchups")]
    public class YahooMatchups
    {
        [XmlAttribute (AttributeName = "count")]
        public int Count { get; set; }

        [XmlElement (ElementName = "matchup")]
        public List<YahooMatchup> MatchupsList { get; set; }

        public YahooMatchups()
        {
            MatchupsList = new List<YahooMatchup>();
        }
    }


    // All matchups: /fantasy/v2/team/{team_key}/matchups
    public class YahooMatchup
    {
        [Key]
        public int YahooMatchupRecordId { get; set; }

        [XmlElement (ElementName = "week")]
        public string WeekNumber { get; set; }


        [XmlElement (ElementName = "week_start")]
        public string WeekStart { get; set; }


        [XmlElement (ElementName = "week_end")]
        public string WeekEnd { get; set; }


        [XmlElement (ElementName = "status")]
        public string Status { get; set; }


        [XmlElement (ElementName = "is_playoffs")]
        public string IsPlayoffs { get; set; }


        [XmlElement (ElementName = "is_consolation")]
        public string IsConsolation { get; set; }


        [XmlElement (ElementName = "is_tied")]
        public string IsTied { get; set; }


        [XmlElement (ElementName = "winner_team_key")]
        public string WinnerTeamKey { get; set; }


        [XmlElement (ElementName = "stat_winner")]
        public List<YahooMatchupStatWinner> YahooMatchupStatWinner { get; set; }


        // [XmlElement (ElementName = "stat")]
        // public List<YahooTeamStats> YahooTeamStats { get; set; }
        public YahooMatchup()
        {
            YahooMatchupStatWinner = new List<YahooMatchupStatWinner>();
            // YahooTeamStats         = new List<YahooTeamStats>();
        }
    }

    public class YahooMatchupStatWinners
    {
        List<YahooMatchupStatWinner> StatWinner { get; set; }

        public YahooMatchupStatWinners ()
        {
            StatWinner = new List<YahooMatchupStatWinner>();
        }
    }


    // this is correct
    [XmlRoot (ElementName = "stat_winner")]
    public class YahooMatchupStatWinner
    {
        [XmlElement (ElementName = "stat_id")]
        public string StatId { get; set; }


        [XmlElement (ElementName = "winner_team_key")]
        public string WinnerTeamKey { get; set; }


        [XmlElement (ElementName = "is_tied")]
        public string IsTied { get; set; }
    }

}
