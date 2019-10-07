using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#pragma warning disable CA1819, IDE0066, MA0048
namespace BaseballScraper.Models.Yahoo.Collections
{
    public partial class YahooPlayersCollection
    {
        [Key]
        public int YahooPlayersCollectionRecordId { get; set; }

        [JsonProperty("league_key")]
        public string LeagueKey { get; set; }


        [JsonProperty("league_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long LeagueId { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }


        [JsonProperty("url")]
        public string Url { get; set; }


        [JsonProperty("logo_url")]
        public string LogoUrl { get; set; }


        [JsonProperty("password")]
        public object Password { get; set; }


        [JsonProperty("draft_status")]
        public string DraftStatus { get; set; }


        [JsonProperty("num_teams")]
        public string NumTeams { get; set; }


        [JsonProperty("edit_key")]
        public string EditKey { get; set; }


        [JsonProperty("weekly_deadline")]
        public object WeeklyDeadline { get; set; }


        [JsonProperty("league_update_timestamp")]
        public string LeagueUpdateTimestamp { get; set; }


        [JsonProperty("scoring_type")]
        public string ScoringType { get; set; }


        [JsonProperty("league_type")]
        public string LeagueType { get; set; }


        [JsonProperty("renew")]
        public string Renew { get; set; }


        [JsonProperty("renewed")]
        public object Renewed { get; set; }


        [JsonProperty("iris_group_chat_id")]
        public string IrisGroupChatId { get; set; }


        [JsonProperty("short_invitation_url")]
        public string ShortInvitationUrl { get; set; }


        [JsonProperty("allow_add_to_dl_extra_pos")]
        public string AllowAddToDlExtraPos { get; set; }


        [JsonProperty("is_pro_league")]
        public string IsProLeague { get; set; }


        [JsonProperty("is_cash_league")]
        public string IsCashLeague { get; set; }


        [JsonProperty("current_week")]
        public string CurrentWeek { get; set; }


        [JsonProperty("start_week")]
        public string StartWeek { get; set; }


        [JsonProperty("start_date")]
        public string StartDate { get; set; }


        [JsonProperty("end_week")]
        public string EndWeek { get; set; }


        [JsonProperty("end_date")]
        public string EndDate { get; set; }


        [JsonProperty("game_code")]
        public string GameCode { get; set; }


        [JsonProperty("season")]
        public string Season { get; set; }


        [JsonProperty("players")]
        public Players Players { get; set; }

    }

    public partial class Players
    {
        [JsonProperty("@count")]
        public string Count { get; set; }

        public IList<Player> PlayerList { get; set; }


        [JsonProperty("player")]
        public object Player { get; set; }

        protected IList<Player> PlayerCasted
        {
            // if player = string, null; else, PlayerList
            get
            {
                return Player is string ? null : PlayerList;
            }
        }
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


        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }


        [JsonProperty("status_full", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusFull { get; set; }


        [JsonProperty("editorial_player_key")]
        public string EditorialPlayerKey { get; set; }


        [JsonProperty("editorial_team_key")]
        public string EditorialTeamKey { get; set; }


        [JsonProperty("editorial_team_full_name")]
        public string EditorialTeamFullName { get; set; }


        [JsonProperty("editorial_team_abbr")]
        public string EditorialTeamAbbr { get; set; }


        [JsonProperty("uniform_number")]
        public string UniformNumber { get; set; }


        [JsonProperty("display_position")]
        public string DisplayPosition { get; set; }


        [JsonProperty("headshot")]
        public Headshot Headshot { get; set; }


        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }


        [JsonProperty("is_undroppable")]
        public string IsUndroppable { get; set; }


        [JsonProperty("position_type")]
        public PositionType PositionType { get; set; }


        [JsonProperty("primary_position")]
        public string PrimaryPosition { get; set; }


        [JsonProperty("eligible_positions")]
        public EligiblePositions EligiblePositions { get; set; }


        [JsonProperty("has_player_notes", NullValueHandling = NullValueHandling.Ignore)]
        public string HasPlayerNotes { get; set; }


        [JsonProperty("player_notes_last_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayerNotesLastTimestamp { get; set; }


        [JsonProperty("has_recent_player_notes", NullValueHandling = NullValueHandling.Ignore)]
        public string HasRecentPlayerNotes { get; set; }


        [JsonProperty("on_disabled_list", NullValueHandling = NullValueHandling.Ignore)]
        public string OnDisabledList { get; set; }

        // public static implicit operator Player(JToken v)
        // {
        //     throw new NotImplementedException();
        // }
    }

    // public partial class EligiblePositions
    // {
    //     [JsonProperty("position")]
    //     public PositionUnion Position { get; set; }
    //     // public Position Position { get; set; }
    //     // public List<Position> Position { get; set; }
    // }


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
                // * player is DH / Util only
                // * if Position = string, null; else (string[]) Position;
                return Position is String ? null : (string[])Position;
            }
        }
    }

    public partial class Headshot
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

    public partial class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }


        [JsonProperty("@encoding")]
        public string Encoding { get; set; }
    }


    public enum Size { Small };

    public enum PositionType { B, P };


    public partial class YahooPlayersCollection
    {
        public static YahooPlayersCollection FromJson(string json) => JsonConvert.DeserializeObject<YahooPlayersCollection>(json, Converter.Settings);
    }


    public static class Serialize
    {
        public static string ToJson(this YahooPlayersCollection self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }


    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                SizeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal },
            },
        };
    }


    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            string value = serializer.Deserialize<string>(reader);
            if (long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out long l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, value: null);
                return;
            }
            long value = (long)untypedValue;
            serializer.Serialize(writer, value: value.ToString( CultureInfo.InvariantCulture));
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class PositionTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PositionType) || t == typeof(PositionType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            string value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "B":
                    return PositionType.B;
                case "P":
                    return PositionType.P;
                default:
                    break;
            }
            throw new Exception("Cannot unmarshal type PositionType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, value: null);
                return;
            }
            PositionType value = (PositionType)untypedValue;
            switch (value)
            {
                case PositionType.B:
                    serializer.Serialize(writer, "B");
                    return;
                case PositionType.P:
                    serializer.Serialize(writer, "P");
                    return;
                default:
                    break;
            }
            throw new Exception("Cannot marshal type PositionType");
        }

        public static readonly PositionTypeConverter Singleton = new PositionTypeConverter();
    }


    internal class SizeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Size) || t == typeof(Size?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            string value = serializer.Deserialize<string>(reader);
            if (string.Equals(value, "small", StringComparison.Ordinal))
            {
                return Size.Small;
            }
            throw new Exception("Cannot unmarshal type Size");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, value: null);
                return;
            }
            Size value = (Size)untypedValue;
            if (value == Size.Small)
            {
                serializer.Serialize(writer, "small");
                return;
            }
            throw new Exception("Cannot marshal type Size");
        }

        public static readonly SizeConverter Singleton = new SizeConverter();
    }
}