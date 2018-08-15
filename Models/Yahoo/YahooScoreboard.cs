using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{


    [XmlRoot (ElementName = "stat_winners")]
    public class StatWinnerList
    {
        [XmlElement (ElementName = "stat_winner")]
        public List<StatWinner> StatWinner { get; set; }
    }





    // [XmlRoot (ElementName = "team")]
    // public class ScoreboardTeam: TeamBase
    // {
    //     [XmlElement (ElementName = "team_points")]
    //     public TeamWeekPoints TeamPoints { get; set; }

    //     [XmlElement (ElementName = "team_stats")]
    //     public TeamStats TeamStats { get; set; }
    // }

    // [XmlRoot (ElementName = "teams")]
    // public class ScoreboardTeamList
    // {
    //     [XmlElement (ElementName = "team")]
    //     public List<ScoreboardTeam> Teams { get; set; }

    //     [XmlAttribute (AttributeName = "count")]
    //     public string Count { get; set; }
    // }

    // [XmlRoot (ElementName = "matchup")]
    // public class Matchup
    // {
    //     [XmlElement (ElementName = "week")]
    //     public int Week { get; set; }

    //     [XmlElement (ElementName = "week_start")]
    //     public DateTime WeekStart { get; set; }

    //     [XmlElement (ElementName = "week_end")]
    //     public DateTime WeekEnd { get; set; }

    //     [XmlElement (ElementName = "status")]
    //     public string Status { get; set; }

    //     [XmlElement (ElementName = "is_playoffs")]
    //     public bool IsPlayoffs { get; set; }

    //     [XmlElement (ElementName = "is_consolation")]
    //     public bool IsConsolation { get; set; }

    //     [XmlElement (ElementName = "is_tied")]
    //     public bool IsTied { get; set; }

    //     [XmlElement (ElementName = "winner_team_key")]
    //     public string WinnerTeamKey { get; set; }

    //     [XmlElement (ElementName = "stat_winners")]
    //     public StatWinnerList StatWinners { get; set; }

    //     // [XmlElement (ElementName = "teams")]
    //     // public ScoreboardTeamList Teams { get; set; }
    // }



    [XmlRoot (ElementName = "scoreboard")]
    public class Scoreboard
    {
        [XmlElement (ElementName = "week")]
        public string Week { get; set; }

        // [XmlElement (ElementName = "matchups")]
        // public MatchupList Matchups { get; set; }
    }

}
