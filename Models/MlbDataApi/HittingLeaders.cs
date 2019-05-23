// SOURCE:
// https://appac.github.io/mlb-data-api-docs/#reports-hitting-leaders-get
// EXAMPLE:
// http://lookup-service-prod.mlb.com/json/named.leader_hitting_repeater.bam?sport_code='mlb'&results=5&game_type='R'&season='2017'&sort_column='ab'&leader_hitting_repeater.col_in=ab


using System;
using System.Collections.Generic;

using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BaseballScraper.Infrastructure;

namespace BaseballScraper.Models.MlbDataApi
{
    [DataContract]
    public class LeadingHitter
    {
        [DataMember(Name="gidp")]
        public int? GroundedIntoDoublePlays { get; set; }

        [DataMember(Name="sac")]
        public int? Sacrifices { get; set; }

        [DataMember(Name="np")]
        public int? Np { get; set; }

        [DataMember(Name="name_display_first_last")]
        public string NameFirstLast { get; set; }

        [DataMember(Name="pos")]
        public string Position { get; set; }

        [DataMember(Name="tb")]
        public int? TotalBases { get; set; }

        [DataMember(Name="team_brief")]
        public string TeamNameShort { get; set; }

        [DataMember(Name="sport_id")]
        public int? MlbDataSportId { get; set; }

        [DataMember(Name="name_display_last_init")]
        public string NameDisplayLastInit { get; set; }

        [DataMember(Name="avg")]
        public decimal? BattingAverage { get; set; }

        [DataMember(Name="slg")]
        public decimal? SluggingPercentage { get; set; }

        [DataMember(Name="ops")]
        public decimal? OnBasePlusSlugging { get; set; }

        [DataMember(Name="hbp")]
        public int? HitByPitches { get; set; }

        [DataMember(Name="team_abbrev")]
        public string TeamAbbreviation { get; set; }

        [DataMember(Name="so")]
        public int? Strikeouts { get; set; }

        [DataMember(Name="league_id")]
        public int? MlbDataApiLeagueId { get; set; }

        [DataMember(Name="sf")]
        public int? SacrificeFlies { get; set; }

        [DataMember(Name="team")]
        public string TeamNameFull { get; set; }

        [DataMember(Name="league")]
        public string League { get; set; }

        [DataMember(Name="cs")]
        public int? CaughtStealing { get; set; }

        [DataMember(Name="sb")]
        public int? StolenBases { get; set; }

        [DataMember(Name="go_ao")]
        public decimal? GroundBallToFlyBallRatio { get; set; }


        [DataMember(Name="player_id")]
        public int? MlbDataApiPlayerId { get; set; }

        [DataMember(Name="ibb")]
        public int? IntentionalWalks { get; set; }

        [DataMember(Name="player_qualifier")]
        public int? PlayerQualifier { get; set; }

        [DataMember(Name="team_id")]
        public int? MlbDataApiTeamId { get; set; }

        [DataMember(Name="go")]
        public int? GroundOuts { get; set; }

        [DataMember(Name="hr")]
        public int? HomeRuns { get; set; }

        [DataMember(Name="minimum_qualifier")]
        public int? MinimumQualifier { get; set; }


        [DataMember(Name="name_display_roster")]
        public string NameLastCommaFirstInitial { get; set; }

        [DataMember(Name="qualifies")]
        public string Qualifies { get; set; }

        [DataMember(Name="rbi")]
        public int? RunsBattedIn { get; set; }

        [DataMember(Name="name_first")]
        public string NameFirst { get; set; }

        [DataMember(Name="bats")]
        public string Bats { get; set; }

        [DataMember(Name="xbh")]
        public int? ExtraBaseHits { get; set; }

        [DataMember(Name="g")]
        public int? Games { get; set; }

        [DataMember(Name="d")]
        public int? Doubles { get; set; }

        // [DataMember(Name="team_name")]
        // public string TeamNameFull { get; set; }

        [DataMember(Name="tpa")]
        public int? TotalPlateAppearances { get; set; }

        [DataMember(Name="name_display_last_first")]
        public string NameLastCommaFirst { get; set; }

        [DataMember(Name="h")]
        public int? Hits { get; set; }

        [DataMember(Name="obp")]
        public decimal? OnBasePercentage { get; set; }

        [DataMember(Name="t")]
        public int? T { get; set; }

        [DataMember(Name="ao")]
        public int? FlyOuts { get; set; }

        [DataMember(Name="r")]
        public int? Runs { get; set; }

        [DataMember(Name="ab")]
        public int? AtBats { get; set; }

        [DataMember(Name="name_last")]
        public string NameLast { get; set; }
    }

    public partial class HittingLeaders
    {
        [JsonProperty("leader_hitting_repeater")]
        public LeaderHittingRepeater LeaderHittingRepeater { get; set; }
    }

    public partial class LeaderHittingRepeater
    {
        [JsonProperty("copyRight")]
        public string CopyRight { get; set; }

        [JsonProperty("leader_hitting_mux")]
        public LeaderHittingMux LeaderHittingMux { get; set; }
    }

    public partial class LeaderHittingMux
    {
        [JsonProperty("sort_column")]
        public string SortColumn { get; set; }

        [JsonProperty("queryResults")]
        public QueryResults QueryResults { get; set; }
    }

    public partial class HitterQueryResults
    {
        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("totalSize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long TotalSize { get; set; }

        [JsonProperty("row")]
        public Dictionary<string, string>[] Row { get; set; }
    }

    // public partial class HittingLeaders
    // {
    //     public static HittingLeaders FromJson(string json) => JsonConvert.DeserializeObject<HittingLeaders>(json, BaseballScraper.Models.MlbDataApi.HittingLeaderConverter.Settings);
    // }

    // public static class SerializeHittingLeaders
    // {
    //     public static string ToJson(this HittingLeaders self) => JsonConvert.SerializeObject(self, BaseballScraper.Models.MlbDataApi.HittingLeaderConverter.Settings);
    // }

    // internal static class HittingLeaderConverter
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

    // internal class HitterParseStringConverter: JsonConverter
    // {
    //     public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

    //     public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //     {
    //         if (reader.TokenType == JsonToken.Null) return null;
    //         var value = serializer.Deserialize<string>(reader);
    //         long l;
    //         if (Int64.TryParse(value, out l))
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

    //     public static readonly HitterParseStringConverter Singleton = new HitterParseStringConverter();
    // }
}
