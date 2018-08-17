// DJB WORKING ON

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using BaseballScraper.Models;
using BaseballScraper.Models.Yahoo;
using Newtonsoft.Json;

namespace BaseballScraper.Models.Yahoo
{
    [XmlRoot(ElementName = "team")]
    public class YahooTeam: YahooTeamBase
    {
        [XmlElement(ElementName = "roster")]
        public YahooTeamRoster YahooTeamRoster { get; set; }

        [XmlElement(ElementName = "team_points")]
        public YahooTeamWeekPoints YahooTeamWeekPoints { get; set; }

        [XmlElement(ElementName = "team_standings")]
        public YahooTeamStandings YahooTeamStandings { get; set; }
    }

    // meta: /fantasy/v2/team/{team_key}/metadata
    public class YahooTeamBase
    {
        public int YahooTeamBaseId { get; set; }

        [JsonProperty("team_key")]
        [XmlElement (ElementName = "team_key")]
        public string Key { get; set; }

        [JsonProperty("team_name")]
        [XmlElement (ElementName = "team_name")]
        public string TeamName { get; set; }

        [JsonProperty("team_id")]
        [XmlElement (ElementName = "team_id")]
        public int? TeamId { get; set; }

        [XmlElement (ElementName = "is_owned_by_current_login")]
        public int? IsOwnedByCurrentLogin { get; set; }

        [XmlElement (ElementName = "url")]
        public string Url { get; set; }

        [XmlElement (ElementName = "team_logo")]
        public YahooTeamLogo TeamLogo { get; set; }

        [XmlElement (ElementName = "waiver_priority")]
        public int? WaiverPriority { get; set; }

        [XmlElement (ElementName = "number_of_moves")]
        public int? NumberOfMoves { get; set; }

        [XmlElement (ElementName = "number_of_trades")]
        public int? NumberOfTrades { get; set; }

        [XmlElement (ElementName = "roster_adds")]
        public YahooTeamRosterAdds TeamRosterAdds { get; set; }

        [XmlElement (ElementName = "league_scoring_type")]
        public string LeagueScoringType { get; set; }

        [XmlElement (ElementName = "has_draft_grade")]
        public string HasDraftGrade { get; set; }

        [XmlElement (ElementName = "managers")]
        public IList<YahooManager> TeamManagersList { get; set; }

        public YahooManager PrimaryTeamManager { get; set; }

        public YahooTeamBase()
        {
            TeamLogo           = new YahooTeamLogo();
            TeamRosterAdds     = new YahooTeamRosterAdds();
            TeamManagersList   = new List<YahooManager>();
            PrimaryTeamManager = new YahooManager();
        }
    }

    [XmlRoot (ElementName = "team_logo")]
    public class YahooTeamLogo
    {
        public int TeamLogoId { get; set; }

        [XmlElement (ElementName = "size")]
        public string Size { get; set; }

        [XmlElement (ElementName = "url")]
        public string Url { get; set; }
    }


    [XmlRoot (ElementName = "roster_adds")]
    public class YahooTeamRosterAdds
    {
        public int RosterAddsId { get; set; }

        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "coverage_value")]
        public string CoverageValue { get; set; }

        [XmlElement (ElementName = "value")]
        public string Value { get; set; }

        // public int YahooTeamBaseId { get; set; }
        // public YahooTeamBase YahooTeamBase { get; set; }
    }


    // /fantasy/v2/team/{team_key}/draftresults
    public class YahooTeamDraftResult: YahooTeamBase
    {
        [XmlElement (ElementName = "@count")]
        public int? NumberOfPlayersDrafted { get; set; }

        public IList<YahooTeamDraftPick> DraftPicks { get; set; }

        public YahooTeamDraftResult()
        {
            DraftPicks = new List<YahooTeamDraftPick>();
        }
    }

    public class YahooTeamDraftPick
    {
        [XmlElement (ElementName = "pick")]
        public string PickNumber { get; set; }

        [XmlElement (ElementName = "round")]
        public string RoundNumber { get; set; }

        [XmlElement (ElementName = "team_key")]
        public string DraftingTeamsKey { get; set; }

        [XmlElement (ElementName = "player_key")]
        public string PlayerKey { get; set; }
    }
}