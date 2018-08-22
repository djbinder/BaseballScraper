// DJB WORKING ON
// I think this is right

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using BaseballScraper.Models;
using BaseballScraper.Models.Yahoo;
using Newtonsoft.Json;

namespace BaseballScraper.Models.Yahoo
{
    // meta: /fantasy/v2/team/{team_key}/metadata
    [XmlRoot(ElementName = "team")]
    public class YahooTeamBase
    {
        // public int YahooTeamBaseId { get; set; }

        [JsonProperty("team_key")]
        [XmlElement (ElementName = "team_key")]
        public string TeamKey { get; set; }

        [JsonProperty("team_id")]
        [XmlElement (ElementName = "team_id")]
        public int? TeamId { get; set; }

        [JsonProperty("team_name")]
        [XmlElement (ElementName = "team_name")]
        public string TeamName { get; set; }

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

        [XmlElement (ElementName = "manager")]
        public YahooManager PrimaryTeamManager { get; set; }

        public YahooTeamBase()
        {
            TeamLogo           = new YahooTeamLogo();
            TeamRosterAdds     = new YahooTeamRosterAdds();
            TeamManagersList   = new List<YahooManager>();
            PrimaryTeamManager = new YahooManager();
        }
    }
}