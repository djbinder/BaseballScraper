using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Newtonsoft.Json;
// using BaseballScraper.Models.Yahoo.Collections.YahooPlayersCollection;
using System;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace BaseballScraper.Models.Yahoo.Resources.YahooRosterResource
{
    [XmlRoot (ElementName = "team", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public partial class YahooRosterResource
    {
        [Key]
        public int YahooRosterResourceRecordId { get; set; }

        [XmlElement (ElementName = "team_key")]
        [JsonProperty("team_key")]
        public string TeamKey { get; set; }

        [JsonProperty("team_id")]
        [JsonConverter(typeof(ParseStringConverter))]
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
        [JsonConverter(typeof(ParseStringConverter))]
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
        [JsonConverter(typeof(ParseStringConverter))]
        public long CoverageValue { get; set; }

        [JsonProperty("value")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Value { get; set; }
    }

    public partial class Players
    {
        [JsonProperty("@count")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Count { get; set; }

        [JsonProperty("player")]
        public List<Player> Player { get; set; }
    }

    public partial class Player
    {
        [JsonProperty("player_key")]
        public string PlayerKey { get; set; }

        [JsonProperty("player_id")]
        [JsonConverter(typeof(ParseStringConverter))]
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
        [JsonConverter(typeof(ParseStringConverter))]
        public long UniformNumber { get; set; }

        [JsonProperty("display_position")]
        public string DisplayPosition { get; set; }

        [JsonProperty("headshot")]
        public TeamLogo Headshot { get; set; }

        [JsonProperty("image_url")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("is_undroppable")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IsUndroppable { get; set; }

        [JsonProperty("position_type")]
        public PositionType PositionType { get; set; }

        [JsonProperty("primary_position")]
        public string PrimaryPosition { get; set; }

        [JsonProperty("eligible_positions")]
        public EligiblePositions EligiblePositions { get; set; }

        [JsonProperty("has_player_notes", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? HasPlayerNotes { get; set; }

        [JsonProperty("player_notes_last_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? PlayerNotesLastTimestamp { get; set; }

        [JsonProperty("selected_position")]
        public SelectedPosition SelectedPosition { get; set; }

        [JsonProperty("starting_status", NullValueHandling = NullValueHandling.Ignore)]
        public StartingStatus StartingStatus { get; set; }

        [JsonProperty("batting_order", NullValueHandling = NullValueHandling.Ignore)]
        public BattingOrder BattingOrder { get; set; }

        [JsonProperty("has_recent_player_notes", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? HasRecentPlayerNotes { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("status_full", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusFull { get; set; }

        [JsonProperty("on_disabled_list", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? OnDisabledList { get; set; }
    }

    public partial class BattingOrder
    {
        [JsonProperty("order_num")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long OrderNum { get; set; }
    }


    // 99% of the time a player will have 2+ eligible positions because 'Util' is applicable to all
    // This means their positions will show as a list in the json
    // But if a player only has 'Util' (i.e., they're a DH only) the return json is a string
    public partial class EligiblePositions
    {
        [JsonProperty("position")]
        public object Position { get; set; }


        protected string[] PositionCasted
        {
            get
            {
                // player is DH / Util only
                if(Position is String)
                {
                    return null;
                }
                // player is eligible for multiple positions
                else
                {
                    return (string[]) Position;
                }
            }
        }
    }



    public partial class TeamLogo
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

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

    public partial class StartingStatus
    {
        [JsonProperty("coverage_type")]
        public CoverageType CoverageType { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("is_starting")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IsStarting { get; set; }
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


    public partial class YahooRosterResource
    {
        public static YahooRosterResource FromJson(string json) => JsonConvert.DeserializeObject<YahooRosterResource>(json, BaseballScraper.Models.Yahoo.Resources.YahooRosterResource.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this YahooRosterResource self) => JsonConvert.SerializeObject(self, BaseballScraper.Models.Yahoo.Resources.YahooRosterResource.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {

            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                CoverageTypeConverter.Singleton,
                SizeConverter.Singleton,
                PositionTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            // Console.WriteLine("A");
            // Console.WriteLine($"existingValue: {existingValue}");
            // Console.WriteLine($"existingValue.GetType(): {existingValue.GetType()}");
            // Console.WriteLine($"reader.TokenTYpe: {reader.TokenType}");

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var value = serializer.Deserialize<string>(reader);

            if (Int64.TryParse(value, out long l))
            {
                return l;
            }
            Console.WriteLine("A2");
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





    internal class CoverageTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(CoverageType) || t == typeof(CoverageType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "date")
            {
                return CoverageType.Date;
            }
            throw new Exception("Cannot unmarshal type CoverageType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (CoverageType)untypedValue;
            if (value == CoverageType.Date)
            {
                serializer.Serialize(writer, "date");
                return;
            }
            throw new Exception("Cannot marshal type CoverageType");
        }

        public static readonly CoverageTypeConverter Singleton = new CoverageTypeConverter();
    }



    internal class SizeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Size) || t == typeof(Size?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "large":
                    return Size.Large;
                case "small":
                    return Size.Small;
            }
            throw new Exception("Cannot unmarshal type Size");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Size)untypedValue;
            switch (value)
            {
                case Size.Large:
                    serializer.Serialize(writer, "large");
                    return;
                case Size.Small:
                    serializer.Serialize(writer, "small");
                    return;
            }
            throw new Exception("Cannot marshal type Size");
        }

        public static readonly SizeConverter Singleton = new SizeConverter();
    }

    internal class PositionTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PositionType) || t == typeof(PositionType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "B":
                    return PositionType.B;
                case "P":
                    return PositionType.P;
            }
            throw new Exception("Cannot unmarshal type PositionType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (PositionType)untypedValue;
            switch (value)
            {
                case PositionType.B:
                    serializer.Serialize(writer, "B");
                    return;
                case PositionType.P:
                    serializer.Serialize(writer, "P");
                    return;
            }
            throw new Exception("Cannot marshal type PositionType");
        }

        public static readonly PositionTypeConverter Singleton = new PositionTypeConverter();
    }
}
