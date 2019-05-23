
// https://app.quicktype.io/
// https://appac.github.io/mlb-data-api-docs/#player-data-player-teams-get

using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using BaseballScraper.Infrastructure;

namespace BaseballScraper.Models.MlbDataApi
{
    [DataContract]
    public class PlayerTeam
    {
        [JsonProperty("season_state", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="season_state")]
        public string SeasonState { get; set; }


        [JsonProperty("hitting_season", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="hitting_season")]
        public int? HittingSeason { get; set; }


        [JsonProperty("sport_full", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="sport_full")]
        public string SportFull { get; set; }


        [JsonProperty("org", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="org")]
        public string Org { get; set; }


        [JsonProperty("sport_code", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="sport_code")]
        public string SportCode { get; set; }


        [JsonProperty("org_short", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="org_short")]
        public string OrgShort { get; set; }


        [JsonProperty("jersey_number", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="jersey_number")]
        public int? JerseyNumber { get; set; }


        [JsonProperty("end_date", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="end_date")]
        public string EndDate { get; set; }


        [JsonProperty("team_brief", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="team_brief")]
        public string TeamBrief { get; set; }


        [JsonProperty("forty_man_sw", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="forty_man_sw")]
        public string FortyManSw { get; set; }


        [JsonProperty("sport_id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="sport_id")]
        public int? SportId { get; set; }


        [JsonProperty("league_short", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="league_short")]
        public string LeagueShort { get; set; }


        [JsonProperty("org_full", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="org_full")]
        public string OrgFull { get; set; }


        [JsonProperty("status_code", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="status_code")]
        public string StatusCode { get; set; }


        [JsonProperty("league_full", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="league_full")]
        public string LeagueFull { get; set; }


        [JsonProperty("primary_position", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="primary_position")]
        public string PrimaryPosition { get; set; }


        [JsonProperty("team_abbrev", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="team_abbrev")]
        public string TeamAbbrev { get; set; }


        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="status")]
        public string Status { get; set; }


        [JsonProperty("org_abbrev", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="org_abbrev")]
        public string OrgAbbrev { get; set; }


        [JsonProperty("league_id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="league_id")]
        public int? LeagueId { get; set; }


        [JsonProperty("class", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="class")]
        public string Class { get; set; }


        [JsonProperty("sport", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="sport")]
        public string Sport { get; set; }


        [JsonProperty("team_short", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="team_short")]
        public string TeamShort { get; set; }


        [JsonProperty("team", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="team")]
        public string TeamName { get; set; }


        [JsonProperty("league", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="league")]
        public string League { get; set; }


        [JsonProperty("fielding_season", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="fielding_season")]
        public int? FieldingSeason { get; set; }


        [JsonProperty("org_id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="org_id")]
        public int? OrgId { get; set; }


        [JsonProperty("class_id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="class_id")]
        public int? ClassId { get; set; }


        [JsonProperty("league_season", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="league_season")]
        public int? LeagueSeason { get; set; }


        [JsonProperty("pitching_season", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="pitching_season")]
        public string PitchingSeason { get; set; }


        [JsonProperty("sport_short", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="sport_short")]
        public string SportShort { get; set; }


        [JsonProperty("status_date", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="status_date")]
        public string StatusDate { get; set; }


        [JsonProperty("player_id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="player_id")]
        public int? PlayerId { get; set; }


        [JsonProperty("current_sw", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="current_sw")]
        public string CurrentSw { get; set; }


        [JsonProperty("primary_stat_type", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="primary_stat_type")]
        public string PrimaryStatType { get; set; }

        [JsonProperty("team_id", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        [DataMember(Name="team_id")]
        public int? TeamId { get; set; }


        [JsonProperty("start_date", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="start_date")]
        public string StartDate { get; set; }
    }

    public partial class PlayerTeams
    {
        [JsonProperty("player_teams", NullValueHandling = NullValueHandling.Ignore)]
        public PlayerTeamsRepeater PlayerTeamsRepeater { get; set; }
    }

    public partial class PlayerTeamsRepeater
    {
        [JsonProperty("copyRight")]
        public string CopyRight { get; set; }

        [JsonProperty("queryResults")]
        public PlayerTeamsQueryResults PlayerTeamsQueryResults { get; set; }
    }

    public partial class PlayerTeamsQueryResults
    {
        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public string Created { get; set; }

        [JsonProperty("totalSize", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public int TotalSize { get; set; }

        [JsonProperty("row", NullValueHandling = NullValueHandling.Ignore)]
        public List<PlayerTeam> Team { get; set; }

        [JsonProperty("row", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string>[] Row { get; set; }
    }


}
