using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using BaseballScraper.Models;

namespace BaseballScraper.Models.Yahoo
{
    public abstract class TeamBase
    {
        [XmlElement (ElementName = "team_key")]
        public string TeamKey { get; set; } //


        [XmlElement (ElementName = "team_id")]
        public string TeamId { get; set; }//


        [XmlElement (ElementName = "team_name")]
        public string TeamName { get; set; }//


        [XmlElement (ElementName = "is_owned_by_current_login")]
        public bool IsOwnedByCurrentLogin { get; set; }


        [XmlElement (ElementName = "url")]
        public string Url { get; set; }


        [XmlElement (ElementName = "waiver_priority")]
        public string WaiverPriority { get; set; }


        [XmlElement (ElementName = "number_of_moves")]
        public string NumberOfMoves { get; set; }


        [XmlElement (ElementName = "number_of_trades")]
        public string NumberOfTrades { get; set; }


        [XmlElement (ElementName = "roster_adds")]
        public int? RosterAdds { get; set; }


        [XmlElement (ElementName = "managers")]
        public List<YahooManager> ManagerList { get; set; }


        [XmlElement (ElementName = "clinched_playoffs")]
        public bool ClinchedPlayoffs { get; set; }
    }


    [XmlRoot (ElementName = "team")]
    public class YahooTeam: TeamBase
    {

        [XmlElement (ElementName = "roster")]
        public List<YahooRoster> Roster { get; set; }


        [XmlElement (ElementName = "team_wins")]
        public int? TeamWins { get; set; }


        [XmlElement (ElementName = "team_losses")]
        public int? TeamLosses { get; set; }


        [XmlElement (ElementName = "team_ties")]
        public int? TeamTies { get; set; }


        [XmlElement (ElementName = "team_winning_percentage")]
        public int? TeamWinningPercentage { get; set; }


    }

    public class YahooTeamBase
    {
        [XmlElement (ElementName = "team_key")]
        public string Key { get; set; }
        public string Name { get; set; }
        public int? TeamId { get; set; }
        public int? IsOwnedByCurrentLogin { get; set; }
        public string Url { get; set; }
        public string TeamLogo { get; set; }
        public int? WaiverPriority { get; set; }
        public int? NumberOfMoves { get; set; }
        public int? NumberOfTrades { get; set; }
        public int? RosterAdds { get; set; }
        public string CoverageType { get; set; }
        public string CoverageValue { get; set; }
        public string Value { get; set; }
        public string LeagueScoringType { get; set; }
        public string HasDraftGuide { get; set; }
        public string ManagerId { get; set; }
        public string NickName { get; set; }
        public string Guid { get; set; }
        public int? IsCommissioner { get; set; }
        public int? IsCurrentLogin { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
    }


    public class YahooTeamStats
    {
        public string Season { get; set; }
        public string CoverageType { get; set; }
        public string WeekNumber { get; set; }
        public string HitsAtBatsId { get; set; } = "60";
        public string HitsAtBatsTotal { get; set; }
        public string RunsId { get; set; } = "7";
        public string RunsTotal { get; set; }
        public string HomeRunsId { get; set; } = "12";
        public string HomeRunsTotal { get; set; }
        public string RbiId { get; set; } = "13";
        public string RbiTotal { get; set; }
        public string StolenBasesId { get; set; } = "16";
        public string StolenBasesTotal { get; set; }
        public string WalksId { get; set; } = "18";
        public string WalksTotal { get; set; }
        public string BattingAverageId { get; set; } = "3";
        public string BattingAverageTotal { get; set; }
        public string InningsPitchedId { get; set; } = "50";
        public string InningsPitchedTotal { get; set; }
        public string WinsId { get; set; } = "28";
        public string WinsTotal { get; set; }
        public string StrikeoutsId { get; set; } = "32";
        public string StrikeoutsTotal { get; set; }
        public string SavesId { get; set; } = "42";
        public string SavesTotal { get; set; }
        public string HoldsId { get; set; } = "48";
        public string HoldsTotal { get; set; }
        public string EraId { get; set; } = "26";
        public string EraTotal { get; set; }
        public string WhipId { get; set; } = "27";
        public string WhipTotal { get; set; }

        public string TeamPoints { get; set; }
    }



}
