using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BaseballScraper.Models.Yahoo.YahooPlayersCollection
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


        [JsonProperty("player")]
        public List<Player> Player { get; set; }
    }

    public partial class Player
    {
        [JsonProperty("player_key")]
        public string PlayerKey { get; set; }


        [JsonProperty("player_id")]
        public string PlayerId { get; set; }


        [JsonProperty("name")]
        public Name Name { get; set; }


        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public Status? Status { get; set; }


        [JsonProperty("status_full", NullValueHandling = NullValueHandling.Ignore)]
        public StatusFull? StatusFull { get; set; }


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
        public Position PrimaryPosition { get; set; }



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
    }

    public partial class EligiblePositions
    {
        [JsonProperty("position")]
        public List<Position> Position { get; set; }
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

    public enum YPosition { Sp, SpRp };

    public enum Position { C, Dl, If, Of, Ss, The1B, The2B, The3B, Util, P, Sp, Rp };

    public enum Size { Small };

    public enum PositionType { B, P };

    public enum Status { Dl7, Dl10, Dl60, Na };

    public enum StatusFull { NotActive, The7DayDisabledList, The10DayDisabledList, The60DayDisabledList };


    public partial class YahooPlayersCollection
    {
        public static YahooPlayersCollection FromJson(string json) => JsonConvert.DeserializeObject<YahooPlayersCollection>(json, BaseballScraper.Models.Yahoo.YahooPlayersCollection.Converter.Settings);
    }


    public static class Serialize
    {
        public static string ToJson(this YahooPlayersCollection self) => JsonConvert.SerializeObject(self, BaseballScraper.Models.Yahoo.YahooPlayersCollection.Converter.Settings);
    }


    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                YPositionConverter.Singleton,
                PositionConverter.Singleton,
                SizeConverter.Singleton,
                StatusConverter.Singleton,
                StatusFullConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }


    internal class ParseStringConverter : JsonConverter
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


    internal class YPositionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(YPosition) || t == typeof(YPosition?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "SP":
                    return YPosition.Sp;
                case "SP,RP":
                    return YPosition.SpRp;
            }
            throw new Exception("Cannot unmarshal type YPosition");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (YPosition)untypedValue;
            switch (value)
            {
                case YPosition.Sp:
                    serializer.Serialize(writer, "SP");
                    return;
                case YPosition.SpRp:
                    serializer.Serialize(writer, "SP,RP");
                    return;
            }
            throw new Exception("Cannot marshal type YPosition");
        }

        public static readonly YPositionConverter Singleton = new YPositionConverter();
    }


    internal class PositionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Position) || t == typeof(Position?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "DL":
                    return Position.Dl;
                case "P":
                    return Position.P;
                case "RP":
                    return Position.Rp;
                case "SP":
                    return Position.Sp;
                case "1B":
                    return Position.The1B;
                case "2B":
                    return Position.The2B;
                case "3B":
                    return Position.The3B;
                case "C":
                    return Position.C;
                case "IF":
                    return Position.If;
                case "OF":
                    return Position.Of;
                case "SS":
                    return Position.Ss;
                case "Util":
                    return Position.Util;
            }
            throw new Exception("Cannot unmarshal type Position");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Position)untypedValue;
            switch (value)
            {
                case Position.Dl:
                    serializer.Serialize(writer, "DL");
                    return;
                case Position.P:
                    serializer.Serialize(writer, "P");
                    return;
                case Position.Rp:
                    serializer.Serialize(writer, "RP");
                    return;
                case Position.Sp:
                    serializer.Serialize(writer, "SP");
                    return;
                case Position.The1B:
                    serializer.Serialize(writer, "1B");
                    return;
                case Position.The2B:
                    serializer.Serialize(writer, "2B");
                    return;
                case Position.The3B:
                    serializer.Serialize(writer, "3B");
                    return;
                case Position.C:
                    serializer.Serialize(writer, "C");
                    return;
                case Position.If:
                    serializer.Serialize(writer, "IF");
                    return;
                case Position.Of:
                    serializer.Serialize(writer, "OF");
                    return;
                case Position.Ss:
                    serializer.Serialize(writer, "SS");
                    return;
                case Position.Util:
                    serializer.Serialize(writer, "Util");
                    return;
            }
            throw new Exception("Cannot marshal type Position");
        }

        public static readonly PositionConverter Singleton = new PositionConverter();
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


    internal class SizeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Size) || t == typeof(Size?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "small")
            {
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
            if (value == Size.Small)
            {
                serializer.Serialize(writer, "small");
                return;
            }
            throw new Exception("Cannot marshal type Size");
        }

        public static readonly SizeConverter Singleton = new SizeConverter();
    }


    internal class StatusConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Status) || t == typeof(Status?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "DL7":
                    return Status.Dl7;
                case "DL10":
                    return Status.Dl10;
                case "DL60":
                    return Status.Dl60;
                case "NA":
                    return Status.Na;
            }
            throw new Exception("Cannot unmarshal type Status");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Status)untypedValue;
            switch (value)
            {
                case Status.Dl7:
                    serializer.Serialize(writer, "DL7");
                    return;
                case Status.Dl10:
                    serializer.Serialize(writer, "DL10");
                    return;
                case Status.Dl60:
                    serializer.Serialize(writer, "DL60");
                    return;
                case Status.Na:
                    serializer.Serialize(writer, "NA");
                    return;
            }
            throw new Exception("Cannot marshal type Status");
        }

        public static readonly StatusConverter Singleton = new StatusConverter();
    }


    internal class StatusFullConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(StatusFull) || t == typeof(StatusFull?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "7-Day Disabled List":
                    return StatusFull.The7DayDisabledList;
                case "10-Day Disabled List":
                    return StatusFull.The10DayDisabledList;
                case "60-Day Disabled List":
                    return StatusFull.The60DayDisabledList;
                case "Not Active":
                    return StatusFull.NotActive;
            }
            throw new Exception("Cannot unmarshal type StatusFull");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (StatusFull)untypedValue;
            switch (value)
            {
                case StatusFull.The7DayDisabledList:
                    serializer.Serialize(writer, "7-Day Disabled List");
                    return;
                case StatusFull.The10DayDisabledList:
                    serializer.Serialize(writer, "10-Day Disabled List");
                    return;
                case StatusFull.The60DayDisabledList:
                    serializer.Serialize(writer, "60-Day Disabled List");
                    return;
                case StatusFull.NotActive:
                    serializer.Serialize(writer, "Not Active");
                    return;
            }
            throw new Exception("Cannot marshal type StatusFull");
        }

        public static readonly StatusFullConverter Singleton = new StatusFullConverter();
    }

}
