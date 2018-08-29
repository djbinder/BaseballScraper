
// SOURCE:
    // https://appac.github.io/mlb-data-api-docs/#reports-pitching-leaders-get
// EXAMPLE:
    // the uri was: GET http://lookup-service-prod.mlb.com/json/named.leader_pitching_repeater.bam?sport_code='mlb'&results=5&game_type='R'&season='2017'&sort_column='era'&leader_pitching_repeater.col_in=era

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BaseballScraper.Models.MlbDataApi
{
    [DataContract]
    public class LeadingPitcher
    {
        [DataMember(Name="gidp")]
        public string Gidp { get; set; }

        [DataMember(Name="np")]
        public string Np { get; set; }

        [DataMember(Name="name_display_first_last")]
        public string NameDisplayFirstLast { get; set; }

        [DataMember(Name="gf")]
        public string Gf { get; set; }

        [DataMember(Name="k_9")]
        public string K9 { get; set; }

        [DataMember(Name="rank")]
        public string Rank { get; set; }

        [DataMember(Name="sho")]
        public string Shutouts { get; set; }

        [DataMember(Name="tb")]
        public string TotalBases { get; set; }

        [DataMember(Name="bk")]
        public string Bk { get; set; }

        [DataMember(Name="sport_id")]
        public string SportId { get; set; }

        [DataMember(Name="sv")]
        public string Saves { get; set; }

        [DataMember(Name="name_display_last_init")]
        public string NameDisplayLastInit { get; set; }

        [DataMember(Name="slg")]
        public string SluggingAgainst { get; set; }

        [DataMember(Name="avg")]
        public string AverageAgainst { get; set; }

        [DataMember(Name="whip")]
        public string Whip { get; set; }

        [DataMember(Name="bb")]
        public string Walks { get; set; }

        [DataMember(Name="ops")]
        public string OpsAgainst { get; set; }

        [DataMember(Name="p_ip")]
        public string PIp { get; set; }

        [DataMember(Name="team_abbrev")]
        public string TeamAbbreviations { get; set; }

        [DataMember(Name="so")]
        public string Strikeouts { get; set; }

        [DataMember(Name="tbf")]
        public string TotalBattersFaced { get; set; }

        [DataMember(Name="throws")]
        public string Throws { get; set; }

        [DataMember(Name="league_id")]
        public string LeagueId { get; set; }

        [DataMember(Name="wp")]
        public string WildPitches { get; set; }

        [DataMember(Name="team")]
        public string Team { get; set; }

        [DataMember(Name="league")]
        public string MlbLeague { get; set; }

        [DataMember(Name="hb")]
        public string HitBatters { get; set; }

        [DataMember(Name="cs")]
        public string CaughtStealingAgainst { get; set; }

        [DataMember(Name="pa")]
        public string PlateAppearances { get; set; }

        [DataMember(Name="go_ao")]
        public string GroundOutAirOutRatio { get; set; }

        [DataMember(Name="sb")]
        public string StolenBasesAgainst { get; set; }

        [DataMember(Name="last_name")]
        public string LastName { get; set; }

        [DataMember(Name="cg")]
        public string CompleteGames { get; set; }

        [DataMember(Name="player_id")]
        public string PlayerId { get; set; }

        [DataMember(Name="gs")]
        public string GamesStarted { get; set; }

        [DataMember(Name="ibb")]
        public string IntentionalWalks { get; set; }

        [DataMember(Name="h_9")]
        public string H9 { get; set; }

        [DataMember(Name="player_qualifier")]
        public string PlayerQualifier { get; set; }

        [DataMember(Name="team_id")]
        public string TeamId { get; set; }

        [DataMember(Name="go")]
        public string GroundOut { get; set; }

        [DataMember(Name="pk")]
        public string Pk { get; set; }

        [DataMember(Name="hr")]
        public string HomeRunsAgainst { get; set; }

        [DataMember(Name="bb_9")]
        public string BB9 { get; set; }

        [DataMember(Name="minimum_qualifier")]
        public string MinimumQualifier { get; set; }

        [DataMember(Name="wpct")]
        public string WinningPercentage { get; set; }

        [DataMember(Name="gdp")]
        public string InducedDouplePlays { get; set; }

        [DataMember(Name="era")]
        public string Era { get; set; }

        [DataMember(Name="name_display_roster")]
        public string NameDisplayRoster { get; set; }

        [DataMember(Name="qualifies")]
        public string Qualifies { get; set; }

        [DataMember(Name="g")]
        public string Games { get; set; }

        [DataMember(Name="hld")]
        public string Holds { get; set; }

        [DataMember(Name="k_bb")]
        public string StrikeoutsToWalkRatio { get; set; }

        [DataMember(Name="team_name")]
        public string TeamName { get; set; }

        [DataMember(Name="sport")]
        public string Sport { get; set; }

        [DataMember(Name="l")]
        public string Losses { get; set; }

        [DataMember(Name="svo")]
        public string SaveOpportunities { get; set; }

        [DataMember(Name="name_display_last_first")]
        public string NameDisplayLastFirst { get; set; }

        [DataMember(Name="h")]
        public string HitsAgainst { get; set; }

        [DataMember(Name="ip")]
        public string InningsPitched { get; set; }

        [DataMember(Name="obp")]
        public string ObpAgainst { get; set; }

        [DataMember(Name="w")]
        public string Wins { get; set; }

        [DataMember(Name="ao")]
        public string AirOuts { get; set; }

        [DataMember(Name="r")]
        public string RunsAgainst { get; set; }

        [DataMember(Name="name_last")]
        public string NameLast { get; set; }

        [DataMember(Name="er")]
        public string EarnedRuns { get; set; }
    }

    public partial class PitchingLeaders
    {
        [JsonProperty("leader_pitching_repeater")]
        public LeaderPitchingRepeater LeaderPitchingRepeater { get; set; }
    }

    public partial class LeaderPitchingRepeater
    {
        [JsonProperty("copyRight")]
        public string CopyRight { get; set; }

        [JsonProperty("leader_pitching_mux")]
        public LeaderPitchingMux LeaderPitchingMux { get; set; }
    }

    public partial class LeaderPitchingMux
    {
        [JsonProperty("sort_column")]
        public string SortColumn { get; set; }

        [JsonProperty("queryResults")]
        public QueryResults QueryResults { get; set; }
    }

    public partial class QueryResults
    {
        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("totalSize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalSize { get; set; }

        [JsonProperty("row")]
        public Dictionary<string, string>[] Row { get; set; }
    }

    public partial class PitchingLeaders
    {
        public static PitchingLeaders FromJson(string json) => JsonConvert.DeserializeObject<PitchingLeaders>(json, BaseballScraper.Models.MlbDataApi.Converter.Settings);
    }

    public static class SerializePitchingLeaders
    {
        public static string ToJson(this PitchingLeaders self) => JsonConvert.SerializeObject(self, BaseballScraper.Models.MlbDataApi.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling        = DateParseHandling.None,
            Converters               = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter: JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
