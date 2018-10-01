// SOURCE:
    // https://appac.github.io/mlb-data-api-docs/#player-data-player-info-get

using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BaseballScraper.Models.MlbDataApi
{
    [DataContract]
    public class PlayerInfo
    {
        [DataMember(Name="birth_country")]
        public string BirthCountry { get; set; }

        [DataMember(Name="name_prefix")]
        public string NamePrefix { get; set; }

        [DataMember(Name="name_display_first_last")]
        public string NameFirstLast { get; set; }

        [DataMember(Name="college")]
        public string College { get; set; }

        [DataMember(Name="height_inches")]
        public int? HeightInches { get; set; }

        [DataMember(Name="age")]
        public int? Age { get; set; }

        [DataMember(Name="name_display_first_last_html")]
        public string NameFirstLastHtml { get; set; }

        [DataMember(Name="height_feet")]
        public string HeightFeet { get; set; }

        [DataMember(Name="gender")]
        public string Gender { get; set; }

        [DataMember(Name="pro_debut_date")]
        [JsonConverter(typeof(CustomJavaScriptDateTimeConverter))]
        public string ProDebutDate { get; set; }

        [DataMember(Name="team_abbrev")]
        public string TeamAbbreviation { get; set; }

        [DataMember(Name="status")]
        public string Status { get; set; }


        [DataMember(Name="name_display_last_first_html")]
        public string NameLastFirstHtml { get; set; }

        [DataMember(Name="throws")]
        public string Throws { get; set; }

        [DataMember(Name="primary_position_txt")]
        public string PrimaryPositionText { get; set; }

        [DataMember(Name="high_school")]
        public string HighSchool { get; set; }

        [DataMember(Name="name_display_roster_html")]
        public string NameRosterHtml { get; set; }

        [DataMember(Name="name_use")]
        public string NameUse { get; set; }

        [DataMember(Name="player_id")]
        public int? MlbDataApiPlayerId { get; set; }

        [DataMember(Name="status_date")]
        public string StatusDate { get; set; }

        [DataMember(Name="primary_stat_type")]
        public string PrimaryStatType { get; set; }

        [DataMember(Name="team_id")]
        public int? MlbDataApiTeamId { get; set; }

        [DataMember(Name="active_sw")]
        public string Active { get; set; }

        [DataMember(Name="birth_state")]
        public string BirthState { get; set; }

        [DataMember(Name="weight")]
        public int? Weight { get; set; }

        [DataMember(Name="name_middle")]
        public string NameMiddle { get; set; }

        [DataMember(Name="name_display_roster")]
        public string NameLastCommaFirstInitial { get; set; }

        [DataMember(Name="jersey_number")]
        public int? JerseyNumber { get; set; }

        [DataMember(Name="name_first")]
        public string NameFirst { get; set; }

        [DataMember(Name="bats")]
        public string Bats { get; set; }

        [DataMember(Name="team_code")]
        public string TeamCode { get; set; }

        [DataMember(Name="birth_city")]
        public string BirthCity { get; set; }

        [DataMember(Name="name_nick")]
        public string Nickname { get; set; }

        [DataMember(Name="status_code")]
        public string StatusCode { get; set; }

        [DataMember(Name="name_matrilineal")]
        public string NameMatrilineal { get; set; }

        [DataMember(Name="team_name")]
        public string TeamNameFull { get; set; }

        [DataMember(Name="name_display_last_first")]
        public string NameLastCommaFirst { get; set; }

        [DataMember(Name="twitter_id")]
        public string TwitterId { get; set; }

        [DataMember(Name="name_title")]
        public string NameTitle { get; set; }

        [DataMember(Name="file_code")]
        public string FileCode { get; set; }

        [DataMember(Name="name_last")]
        public string NameLast { get; set; }

        [DataMember(Name="start_date")]
        public string StartDate { get; set; }

        [DataMember(Name="name_full")]
        public string NameFull { get; set; }
    }
}