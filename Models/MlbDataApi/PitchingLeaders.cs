
// SOURCE:
    // https://appac.github.io/mlb-data-api-docs/#reports-pitching-leaders-get
// EXAMPLE:
    // the uri was: GET http://lookup-service-prod.mlb.com/json/named.leader_pitching_repeater.bam?sport_code='mlb'&results=5&game_type='R'&season='2017'&sort_column='era'&leader_pitching_repeater.col_in=era

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Runtime.Serialization;
using BaseballScraper.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Models.MlbDataApi
{
    [DataContract]
    public class LeadingPitcher
    {
        [DataMember(Name="gidp")]
        public string GroundedIntoDoublePlays { get; set; }

        [DataMember(Name="np")]
        public string Np { get; set; }

        [DataMember(Name="name_display_first_last")]
        public string NameFirstLast { get; set; }

        [DataMember(Name="gf")]
        public string Gf { get; set; }

        [DataMember(Name="k_9")]
        public string StrikeoutsPerNine { get; set; }

        [DataMember(Name="rank")]
        public string Rank { get; set; }

        [DataMember(Name="sho")]
        public string Shutouts { get; set; }

        [DataMember(Name="tb")]
        public string TotalBases { get; set; }

        [DataMember(Name="bk")]
        public string Balks { get; set; }

        [DataMember(Name="sport_id")]
        public string MlbDataApiSportId { get; set; }

        [DataMember(Name="sv")]
        public string Saves { get; set; }

        [DataMember(Name="name_display_last_init")]
        public string NameDisplayLastInit { get; set; }

        [DataMember(Name="slg")]
        public string SluggingPercentageAgainst { get; set; }

        [DataMember(Name="avg")]
        public string BattingAverageAgainst { get; set; }

        [DataMember(Name="whip")]
        public string Whip { get; set; }

        [DataMember(Name="bb")]
        public string Walks { get; set; }

        [DataMember(Name="ops")]
        public string OnBasePlusSluggingAgainst { get; set; }

        [DataMember(Name="p_ip")]
        public string PitchesPerInningPitched { get; set; }

        [DataMember(Name="team_abbrev")]
        public string TeamAbbreviation { get; set; }

        [DataMember(Name="so")]
        public string Strikeouts { get; set; }

        [DataMember(Name="tbf")]
        public string TotalBattersFaced { get; set; }

        [DataMember(Name="throws")]
        public string Throws { get; set; }

        [DataMember(Name="league_id")]
        public string MlbDataApiLeagueId { get; set; }

        [DataMember(Name="wp")]
        public string WildPitches { get; set; }

        [DataMember(Name="team")]
        public string Team { get; set; }

        [DataMember(Name="league")]
        public string League { get; set; }

        [DataMember(Name="hb")]
        public string HitBatters { get; set; }

        [DataMember(Name="cs")]
        public string CaughtStealingAgainst { get; set; }

        [DataMember(Name="pa")]
        public string PlateAppearances { get; set; }

        [DataMember(Name="go_ao")]
        public string GroundBallToFlyBallRatio { get; set; }

        [DataMember(Name="sb")]
        public string StolenBasesAgainst { get; set; }


        [DataMember(Name="cg")]
        public string CompleteGames { get; set; }

        [DataMember(Name="player_id")]
        public string MlbDataApiPlayerId { get; set; }

        [DataMember(Name="gs")]
        public string GamesStarted { get; set; }

        [DataMember(Name="ibb")]
        public string IntentionalWalks { get; set; }

        [DataMember(Name="h_9")]
        public string HitsPerNine { get; set; }

        [DataMember(Name="player_qualifier")]
        public string PlayerQualifier { get; set; }

        [DataMember(Name="team_id")]
        public string MlbDataApiTeamId { get; set; }

        [DataMember(Name="go")]
        public string GroundOuts { get; set; }

        [DataMember(Name="pk")]
        public string Pk { get; set; }

        [DataMember(Name="hr")]
        public string HomeRunsAgainst { get; set; }

        [DataMember(Name="bb_9")]
        public string WalksPerNine { get; set; }

        [DataMember(Name="minimum_qualifier")]
        public string MinimumQualifier { get; set; }

        [DataMember(Name="wpct")]
        public string WinningPercentage { get; set; }

        [DataMember(Name="gdp")]
        public string DoublePlaysInduced { get; set; }

        [DataMember(Name="era")]
        public string EarnedRunAverage { get; set; }

        [DataMember(Name="name_display_roster")]
        public string NameLastCommaFirstInitial { get; set; }

        [DataMember(Name="qualifies")]
        public string Qualifies { get; set; }

        [DataMember(Name="g")]
        public string Games { get; set; }

        [DataMember(Name="hld")]
        public string Holds { get; set; }

        [DataMember(Name="k_bb")]
        public string StrikeoutsToWalksRatio { get; set; }

        [DataMember(Name="team_name")]
        public string TeamNameFull { get; set; }

        [DataMember(Name="sport")]
        public string Sport { get; set; }

        [DataMember(Name="l")]
        public string Losses { get; set; }

        [DataMember(Name="svo")]
        public string SaveOpportunities { get; set; }

        [DataMember(Name="name_display_last_first")]
        public string NameLastCommaFirst { get; set; }

        [DataMember(Name="h")]
        public string HitsAgainst { get; set; }

        [DataMember(Name="ip")]
        public string InningsPitched { get; set; }

        [DataMember(Name="obp")]
        public string OnBasePercentageAgainst { get; set; }

        [DataMember(Name="w")]
        public string Wins { get; set; }

        [DataMember(Name="ao")]
        public string FlyOuts { get; set; }

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

    // public partial class PitchingLeaders
    // {
    //     public static PitchingLeaders FromJson(string json) => JsonConvert.DeserializeObject<PitchingLeaders>(json, BaseballScraper.Models.MlbDataApi.Converter.Settings);
    // }

    // // public static class SerializePitchingLeaders
    // // {
    // //     public static string ToJson(this PitchingLeaders self) => JsonConvert.SerializeObject(self, BaseballScraper.Models.MlbDataApi.Converter.Settings);
    // // }

    // internal static class Converter
    // {
    //     public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //     {
    //         MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //         DateParseHandling        = DateParseHandling.None,
    //         Converters               = {
    //             new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
    //         },
    //     };
    // }

    // internal class ParseStringConverter: JsonConverter
    // {
    //     public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

    //     public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //     {
    //         if (reader.TokenType == JsonToken.Null) return null;
    //         var value = serializer.Deserialize<string>(reader);
    //         if (Int64.TryParse(value, out long l))
    //         {
    //             return l;
    //         }
    //         throw new Exception("Cannot unmarshal type long");
    //     }

    //     public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //     {
    //         if (untypedValue == null)
    //         {
    //             serializer.Serialize(writer, null);
    //             return;
    //         }
    //         var value = (long)untypedValue;
    //         serializer.Serialize(writer, value.ToString());
    //         return;
    //     }

    //     public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    // }
}
