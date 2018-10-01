// SOURCE
    // https://appac.github.io/mlb-data-api-docs/#player-data-player-search-get


using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BaseballScraper.Models.MlbDataApi
{
    [DataContract]
    public class PlayerSearch
    {
        [DataMember(Name="position")]
        public string Position { get; set; }

        [DataMember(Name="birth_country")]
        public string BirthCountry { get; set; }

        [DataMember(Name="weight")]
        public int? Weight { get; set; }

        [DataMember(Name="birth_state")]
        public string BirthState { get; set; }

        [DataMember(Name="name_display_first_last")]
        public string NameFirstLast { get; set; }

        [DataMember(Name="college")]
        public string College { get; set; }

        [DataMember(Name="height_inches")]
        public int? HeightInches { get; set; }

        [DataMember(Name="name_display_roster")]
        public string NameLastCommaFirstInitial { get; set; }

        [DataMember(Name="sport_code")]
        public string SportCode { get; set; }

        [DataMember(Name="bats")]
        public string Bats { get; set; }

        [DataMember(Name="name_first")]
        public string NameFirst { get; set; }

        [DataMember(Name="team_code")]
        public string TeamCode { get; set; }

        [DataMember(Name="birth_city")]
        public string BirthCity { get; set; }

        [DataMember(Name="height_feet")]
        public string HeightFeet { get; set; }

        [DataMember(Name="pro_debut_date")]
        [JsonConverter(typeof(CustomJavaScriptDateTimeConverter))]
        public string ProDebutDate { get; set; }

        [DataMember(Name="team_full")]
        public string TeamNameFull { get; set; }

        [DataMember(Name="team_abbrev")]
        public string TeamAbbreviation { get; set; }

        [DataMember(Name="birth_date")]
        public string BirthDay { get; set; }

        [DataMember(Name="throws")]
        public string Throws { get; set; }

        [DataMember(Name="league")]
        public string League { get; set; }

        [DataMember(Name="name_display_last_first")]
        public string NameLastComaFirst { get; set; }

        [DataMember(Name="position_id")]
        public int? PositionId { get; set; }

        [DataMember(Name="high_school")]
        public string HighSchool { get; set; }

        [DataMember(Name="name_use")]
        public string NameUse { get; set; }

        [DataMember(Name="player_id")]
        public int? MlbDataApiPlayerId { get; set; }

        [DataMember(Name="name_last")]
        public string NameLast { get; set; }

        [DataMember(Name="team_id")]
        public int? MlbDataApiTeamId { get; set; }

        [DataMember(Name="service_years")]
        public string ServiceYears { get; set; }

        [DataMember(Name="active_sw")]
        public string Active { get; set; }
    }

    public class CustomJavaScriptDateTimeConverter: JavaScriptDateTimeConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var js = new JsonSerializer() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
            js.Serialize(writer, value);
        }
    }
}