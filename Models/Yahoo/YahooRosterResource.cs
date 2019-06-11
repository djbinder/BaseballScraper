using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BaseballScraper.Models.Yahoo.YahooRosterResource
{
    [XmlRoot (ElementName = "team", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class YahooRosterResource
    {
        [Key]
        public int YahooTeamRosterRecordId { get; set; }

        [XmlElement (ElementName = "team_key")]
        [JsonProperty("team_key")]
        public string TeamKey { get; set; }

        [JsonProperty("team_id")]
        public long TeamId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("team_logos")]
        public TeamLogos TeamLogos { get; set; }

        [JsonProperty("waiver_priority")]
        public long WaiverPriority { get; set; }

        [JsonProperty("number_of_moves")]
        public long NumberOfMoves { get; set; }

        [JsonProperty("number_of_trades")]
        public long NumberOfTrades { get; set; }

        [JsonProperty("roster_adds")]
        public RosterAdds RosterAdds { get; set; }

        [JsonProperty("league_scoring_type")]
        public string LeagueScoringType { get; set; }

        [JsonProperty("has_draft_grade")]
        public long HasDraftGrade { get; set; }

        [JsonProperty("managers")]
        public Managers Managers { get; set; }

        [JsonProperty("roster")]
        public Roster Roster { get; set; }
    }


    public partial class Managers
    {
        [JsonProperty("manager")]
        public Manager Manager { get; set; }
    }


    public partial class Manager
    {
        [JsonProperty("manager_id")]
        public long ManagerId { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }


    public partial class Roster
    {
        [JsonProperty("coverage_type")]
        public CoverageType CoverageType { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("is_editable")]
        public long IsEditable { get; set; }

        [JsonProperty("players")]
        public Players Players { get; set; }

        [JsonProperty("outs_pitched")]
        public RosterAdds OutsPitched { get; set; }
    }


    public partial class RosterAdds
    {
        [JsonProperty("coverage_type")]
        public string CoverageType { get; set; }

        [JsonProperty("coverage_value")]
        public long CoverageValue { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }


    public partial class Players
    {
        [JsonProperty("@count")]
        public long Count { get; set; }

        [JsonProperty("player")]
        public List<Player> Player { get; set; }
    }


    public partial class Player
    {
        [JsonProperty("player_key")]
        public string PlayerKey { get; set; }

        [JsonProperty("player_id")]
        public long PlayerId { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("editorial_player_key")]
        public string EditorialPlayerKey { get; set; }

        [JsonProperty("editorial_team_key")]
        public string EditorialTeamKey { get; set; }

        [JsonProperty("editorial_team_full_name")]
        public string EditorialTeamFullName { get; set; }

        [JsonProperty("editorial_team_abbr")]
        public string EditorialTeamAbbr { get; set; }

        [JsonProperty("uniform_number")]
        public long UniformNumber { get; set; }

        [JsonProperty("display_position")]
        public string DisplayPosition { get; set; }

        [JsonProperty("headshot")]
        public TeamLogo Headshot { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("is_undroppable")]
        public long IsUndroppable { get; set; }

        [JsonProperty("position_type")]
        public PositionType PositionType { get; set; }

        [JsonProperty("primary_position")]
        public string PrimaryPosition { get; set; }

        [JsonProperty("eligible_positions")]
        public EligiblePositions EligiblePositions { get; set; }

        [JsonProperty("has_player_notes", NullValueHandling = NullValueHandling.Ignore)]
        public long? HasPlayerNotes { get; set; }

        [JsonProperty("player_notes_last_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public long? PlayerNotesLastTimestamp { get; set; }

        [JsonProperty("selected_position")]
        public SelectedPosition SelectedPosition { get; set; }

        [JsonProperty("has_recent_player_notes", NullValueHandling = NullValueHandling.Ignore)]
        public long? HasRecentPlayerNotes { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("status_full", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusFull { get; set; }

        [JsonProperty("on_disabled_list", NullValueHandling = NullValueHandling.Ignore)]
        public long? OnDisabledList { get; set; }
    }


    public partial class EligiblePositions
    {
        [JsonProperty("position")]
        public List<string> EligiblePosition { get; set; }
    }


    public partial class TeamLogo
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("size")]
        public Size Size { get; set; }
    }


    public partial class Name
    {
        [JsonProperty("full")]
        public string Full { get; set; }

        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("ascii_first")]
        public string AsciiFirst { get; set; }

        [JsonProperty("ascii_last")]
        public string AsciiLast { get; set; }
    }


    public partial class SelectedPosition
    {
        [JsonProperty("coverage_type")]
        public CoverageType CoverageType { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }
    }


    public partial class TeamLogos
    {
        [JsonProperty("team_logo")]
        public TeamLogo TeamLogo { get; set; }
    }


    public partial class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        public string Encoding { get; set; }
    }


    public enum CoverageType { Date };


    public enum Size { Large, Small };


    public enum PositionType { B, P };


    }

