// https://app.quicktype.io/


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

#pragma warning disable IDE0066
namespace BaseballScraper.Models.Yahoo.Collections.YahooPlayersCollection
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

        public List<Player> PlayerList { get; set; }


        [JsonProperty("player")]
        // public List<Player> Player { get; set; }
        public object Player { get; set; }

        protected List<Player> PlayerCasted
        {
            get
            {
                if(Player is String)
                {
                    return null;
                }

                else
                {
                    return PlayerList;
                    // return (List<Player>) PlayerList;
                    // return(List<Player>) Player;
                }
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
        // public Status? Status { get; set; }


        [JsonProperty("status_full", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusFull { get; set; }
        // public StatusFull? StatusFull { get; set; }


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
        // public string PositionType { get; set; }


        [JsonProperty("primary_position")]
        public string PrimaryPosition { get; set; }
        // public PrimaryPositionElement PrimaryPosition { get; set; }
        // public Position PrimaryPosition { get; set; }


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

    // public partial struct Position
    // {
    //     public string String;
    //     public List<string> StringArray;

    //     public static implicit operator Position(string String) => new Position { String = String };
    //     public static implicit operator Position(List<string> StringArray) => new Position { StringArray = StringArray };
    // }


    // public enum PrimaryPositionElement { C, Dl, If, Of, Ss, The1B, The2B, The3B, Util, P, Sp, Rp };
    // // public enum Position { C, Dl, If, Of, Ss, The1B, The2B, The3B, Util, P, Sp, Rp };
    // public partial struct PositionUnion
    // {
    //     public PrimaryPositionElement? Enum;
    //     public List<PrimaryPositionElement> StringArray;

    //     public static implicit operator PositionUnion(PrimaryPositionElement Enum) => new PositionUnion { Enum = Enum };
    //     public static implicit operator PositionUnion(List<PrimaryPositionElement> StringArray) => new PositionUnion { StringArray = StringArray };
    // }


    // public enum YPosition { Sp, SpRp };


    public enum Size { Small };

    public enum PositionType { B, P };

    // public enum Status { Dl7, Dl10, Dl60, Na, DTD };

    // public enum StatusFull { NotActive, The7DayDisabledList, The10DayDisabledList, The60DayDisabledList, DayToDay };


    public partial class YahooPlayersCollection
    {
        public static YahooPlayersCollection FromJson(string json) => JsonConvert.DeserializeObject<YahooPlayersCollection>(json, BaseballScraper.Models.Yahoo.Collections.YahooPlayersCollection.Converter.Settings);
    }


    public static class Serialize
    {
        public static string ToJson(this YahooPlayersCollection self) => JsonConvert.SerializeObject(self, BaseballScraper.Models.Yahoo.Collections.YahooPlayersCollection.Converter.Settings);
    }


    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                // YPositionConverter.Singleton,
                // PositionUnionConverter.Singleton,
                // PrimaryPositionElementConverter.Singleton,
                // PositionConverter.Singleton,
                SizeConverter.Singleton,
                // StatusConverter.Singleton,
                // StatusFullConverter.Singleton,
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
            if (Int64.TryParse(value, out long l))
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

    // internal class YPositionConverter : JsonConverter
    // {
    //     public override bool CanConvert(Type t) => t == typeof(YPosition) || t == typeof(YPosition?);

    //     public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //     {
    //         if (reader.TokenType == JsonToken.Null) return null;
    //         var value = serializer.Deserialize<string>(reader);
    //         switch (value)
    //         {
    //             case "SP":
    //                 return YPosition.Sp;
    //             case "SP,RP":
    //                 return YPosition.SpRp;
    //         }
    //         throw new Exception("Cannot unmarshal type YPosition");
    //     }

    //     public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //     {
    //         if (untypedValue == null)
    //         {
    //             serializer.Serialize(writer, null);
    //             return;
    //         }
    //         var value = (YPosition)untypedValue;
    //         switch (value)
    //         {
    //             case YPosition.Sp:
    //                 serializer.Serialize(writer, "SP");
    //                 return;
    //             case YPosition.SpRp:
    //                 serializer.Serialize(writer, "SP,RP");
    //                 return;
    //         }
    //         throw new Exception("Cannot marshal type YPosition");
    //     }

    //     public static readonly YPositionConverter Singleton = new YPositionConverter();
    // }


    // internal class PositionUnionConverter : JsonConverter
    // {
    //     public override bool CanConvert(Type t) => t == typeof(PositionUnion) || t == typeof(PositionUnion?);

    //     public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //     {
    //         switch (reader.TokenType)
    //         {
    //             case JsonToken.String:
    //             case JsonToken.Date:
    //                 var stringValue = serializer.Deserialize<string>(reader);
    //                 switch (stringValue)
    //                 {
    //                     case "1B":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.The1B };
    //                     case "2B":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.The2B };
    //                     case "3B":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.The3B };
    //                     case "C":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.C };
    //                     case "IF":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.If };
    //                     case "OF":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.Of };
    //                     case "SS":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.Ss };
    //                     case "Util":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.Util };
    //                     case "DL":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.Dl };
    //                     case "P":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.P };
    //                     case "SP":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.Sp };
    //                     case "RP":
    //                         return new PositionUnion { Enum = PrimaryPositionElement.Rp };
    //                 }
    //                 break;
    //             case JsonToken.StartArray:
    //                 var arrayValue = serializer.Deserialize<List<PrimaryPositionElement>>(reader);
    //                 return new PositionUnion { StringArray = arrayValue };
    //         }
    //         throw new Exception("Cannot unmarshal type PositionUnion");
    //     }


    //     public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //     {
    //         var value = (PositionUnion)untypedValue;
    //         if (value.Enum != null)
    //         {
    //             switch (value.Enum)
    //             {
    //                 case PrimaryPositionElement.The1B:
    //                     serializer.Serialize(writer, "1B");
    //                     return;
    //                 case PrimaryPositionElement.The2B:
    //                     serializer.Serialize(writer, "2B");
    //                     return;
    //                 case PrimaryPositionElement.The3B:
    //                     serializer.Serialize(writer, "3B");
    //                     return;
    //                 case PrimaryPositionElement.C:
    //                     serializer.Serialize(writer, "C");
    //                     return;
    //                 case PrimaryPositionElement.If:
    //                     serializer.Serialize(writer, "IF");
    //                     return;
    //                 case PrimaryPositionElement.Of:
    //                     serializer.Serialize(writer, "OF");
    //                     return;
    //                 case PrimaryPositionElement.Ss:
    //                     serializer.Serialize(writer, "SS");
    //                     return;
    //                 case PrimaryPositionElement.Util:
    //                     serializer.Serialize(writer, "Util");
    //                     return;
    //                 case PrimaryPositionElement.Dl:
    //                     serializer.Serialize(writer, "DL");
    //                     return;
    //                 case PrimaryPositionElement.P:
    //                     serializer.Serialize(writer, "P");
    //                     return;
    //                 case PrimaryPositionElement.Sp:
    //                     serializer.Serialize(writer, "SP");
    //                     return;
    //                 case PrimaryPositionElement.Rp:
    //                     serializer.Serialize(writer, "RP");
    //                     return;
    //             }
    //         }
    //         if (value.StringArray != null)
    //         {
    //             serializer.Serialize(writer, value.StringArray);
    //             return;
    //         }
    //         throw new Exception("Cannot marshal type PositionUnion");
    //     }

    //     public static readonly PositionUnionConverter Singleton = new PositionUnionConverter();
    // }


    // internal class PrimaryPositionElementConverter : JsonConverter
    // {
    //     public override bool CanConvert(Type t) => t == typeof(PrimaryPositionElement) || t == typeof(PrimaryPositionElement?);

    //     public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //     {
    //         if (reader.TokenType == JsonToken.Null) return null;
    //         Console.WriteLine("POINT 1");
    //         var value = serializer.Deserialize<string>(reader);
    //         Console.WriteLine("POINT 2");
    //         switch (value)
    //         {
    //             case "1B":
    //                 return PrimaryPositionElement.The1B;
    //             case "2B":
    //                 return PrimaryPositionElement.The2B;
    //             case "3B":
    //                 return PrimaryPositionElement.The3B;
    //             case "C":
    //                 return PrimaryPositionElement.C;
    //             case "IF":
    //                 return PrimaryPositionElement.If;
    //             case "OF":
    //                 return PrimaryPositionElement.Of;
    //             case "SS":
    //                 return PrimaryPositionElement.Ss;
    //             case "Util":
    //                 return PrimaryPositionElement.Util;
    //             case "DL":
    //                 return PrimaryPositionElement.Dl;
    //             case "P":
    //                 return PrimaryPositionElement.P;
    //             case "SP":
    //                 return PrimaryPositionElement.Sp;
    //             case "RP":
    //                 return PrimaryPositionElement.Rp;
    //         }
    //         Console.WriteLine("POINT 3");
    //         throw new Exception("Cannot unmarshal type PrimaryPositionElement");
    //     }

    //     public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //     {
    //         if (untypedValue == null)
    //         {
    //             serializer.Serialize(writer, null);
    //             return;
    //         }
    //         var value = (PrimaryPositionElement)untypedValue;
    //         switch (value)
    //         {
    //             case PrimaryPositionElement.The1B:
    //                 serializer.Serialize(writer, "1B");
    //                 return;
    //             case PrimaryPositionElement.The2B:
    //                 serializer.Serialize(writer, "2B");
    //                 return;
    //             case PrimaryPositionElement.The3B:
    //                 serializer.Serialize(writer, "3B");
    //                 return;
    //             case PrimaryPositionElement.C:
    //                 serializer.Serialize(writer, "C");
    //                 return;
    //             case PrimaryPositionElement.If:
    //                 serializer.Serialize(writer, "IF");
    //                 return;
    //             case PrimaryPositionElement.Of:
    //                 serializer.Serialize(writer, "OF");
    //                 return;
    //             case PrimaryPositionElement.Ss:
    //                 serializer.Serialize(writer, "SS");
    //                 return;
    //             case PrimaryPositionElement.Util:
    //                 serializer.Serialize(writer, "Util");
    //                 return;
    //             case PrimaryPositionElement.Dl:
    //                 serializer.Serialize(writer, "DL");
    //                 return;
    //             case PrimaryPositionElement.P:
    //                 serializer.Serialize(writer, "P");
    //                 return;
    //             case PrimaryPositionElement.Sp:
    //                 serializer.Serialize(writer, "SP");
    //                 return;
    //             case PrimaryPositionElement.Rp:
    //                 serializer.Serialize(writer, "RP");
    //                 return;
    //         }
    //         throw new Exception("Cannot marshal type PrimaryPositionElement");
    //     }

    //     public static readonly PrimaryPositionElementConverter Singleton = new PrimaryPositionElementConverter();
    // }





    // internal class StatusConverter : JsonConverter
    // {
    //     public override bool CanConvert(Type t) => t == typeof(Status) || t == typeof(Status?);

    //     public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //     {
    //         if (reader.TokenType == JsonToken.Null) return null;
    //         var value = serializer.Deserialize<string>(reader);
    //         switch (value)
    //         {
    //             case "DL7":
    //                 return Status.Dl7;
    //             case "DL10":
    //                 return Status.Dl10;
    //             case "DL60":
    //                 return Status.Dl60;
    //             case "NA":
    //                 return Status.Na;
    //             case "DTD":
    //                 return Status.DTD;
    //         }
    //         throw new Exception("Cannot unmarshal type Status");
    //     }

    //     public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //     {
    //         if (untypedValue == null)
    //         {
    //             serializer.Serialize(writer, null);
    //             return;
    //         }
    //         var value = (Status)untypedValue;
    //         switch (value)
    //         {
    //             case Status.Dl7:
    //                 serializer.Serialize(writer, "DL7");
    //                 return;
    //             case Status.Dl10:
    //                 serializer.Serialize(writer, "DL10");
    //                 return;
    //             case Status.Dl60:
    //                 serializer.Serialize(writer, "DL60");
    //                 return;
    //             case Status.Na:
    //                 serializer.Serialize(writer, "NA");
    //                 return;
    //             case Status.DTD:
    //                 serializer.Serialize(writer, "DTD");
    //                 return;
    //         }
    //         throw new Exception("Cannot marshal type Status");
    //     }

    //     public static readonly StatusConverter Singleton = new StatusConverter();
    // }


    // internal class StatusFullConverter : JsonConverter
    // {
    //     public override bool CanConvert(Type t) => t == typeof(StatusFull) || t == typeof(StatusFull?);

    //     public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //     {
    //         if (reader.TokenType == JsonToken.Null) return null;
    //         var value = serializer.Deserialize<string>(reader);
    //         switch (value)
    //         {
    //             case "7-Day Disabled List":
    //                 return StatusFull.The7DayDisabledList;
    //             case "10-Day Disabled List":
    //                 return StatusFull.The10DayDisabledList;
    //             case "60-Day Disabled List":
    //                 return StatusFull.The60DayDisabledList;
    //             case "Not Active":
    //                 return StatusFull.NotActive;
    //             case "Day-to-Day":
    //                 return StatusFull.DayToDay;
    //         }
    //         throw new Exception("Cannot unmarshal type StatusFull");
    //     }

    //     public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //     {
    //         if (untypedValue == null)
    //         {
    //             serializer.Serialize(writer, null);
    //             return;
    //         }
    //         var value = (StatusFull)untypedValue;
    //         switch (value)
    //         {
    //             case StatusFull.The7DayDisabledList:
    //                 serializer.Serialize(writer, "7-Day Disabled List");
    //                 return;
    //             case StatusFull.The10DayDisabledList:
    //                 serializer.Serialize(writer, "10-Day Disabled List");
    //                 return;
    //             case StatusFull.The60DayDisabledList:
    //                 serializer.Serialize(writer, "60-Day Disabled List");
    //                 return;
    //             case StatusFull.NotActive:
    //                 serializer.Serialize(writer, "Not Active");
    //                 return;
    //             case StatusFull.DayToDay:
    //                 serializer.Serialize(writer, "Day-to-Day");
    //                 return;
    //         }
    //         throw new Exception("Cannot marshal type StatusFull");
    //     }

    //     public static readonly StatusFullConverter Singleton = new StatusFullConverter();
    // }

}




// internal class PositionConverter : JsonConverter
//     {
//         public override bool CanConvert(Type t) => t == typeof(Position) || t == typeof(Position?);

//         public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
//         {
//             if (reader.TokenType == JsonToken.Null) return null;
//             var value = serializer.Deserialize<string>(reader);
//             switch (value)
//             {
//                 case "DL":
//                     return Position.Dl;
//                 case "P":
//                     return Position.P;
//                 case "RP":
//                     return Position.Rp;
//                 case "SP":
//                     return Position.Sp;
//                 case "1B":
//                     return Position.The1B;
//                 case "2B":
//                     return Position.The2B;
//                 case "3B":
//                     return Position.The3B;
//                 case "C":
//                     return Position.C;
//                 case "IF":
//                     return Position.If;
//                 case "OF":
//                     return Position.Of;
//                 case "SS":
//                     return Position.Ss;
//                 case "Util":
//                     return Position.Util;
//             }
//             throw new Exception("Cannot unmarshal type Position");
//         }

//         public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
//         {
//             if (untypedValue == null)
//             {
//                 serializer.Serialize(writer, null);
//                 return;
//             }
//             var value = (Position)untypedValue;
//             switch (value)
//             {
//                 case Position.Dl:
//                     serializer.Serialize(writer, "DL");
//                     return;
//                 case Position.P:
//                     serializer.Serialize(writer, "P");
//                     return;
//                 case Position.Rp:
//                     serializer.Serialize(writer, "RP");
//                     return;
//                 case Position.Sp:
//                     serializer.Serialize(writer, "SP");
//                     return;
//                 case Position.The1B:
//                     serializer.Serialize(writer, "1B");
//                     return;
//                 case Position.The2B:
//                     serializer.Serialize(writer, "2B");
//                     return;
//                 case Position.The3B:
//                     serializer.Serialize(writer, "3B");
//                     return;
//                 case Position.C:
//                     serializer.Serialize(writer, "C");
//                     return;
//                 case Position.If:
//                     serializer.Serialize(writer, "IF");
//                     return;
//                 case Position.Of:
//                     serializer.Serialize(writer, "OF");
//                     return;
//                 case Position.Ss:
//                     serializer.Serialize(writer, "SS");
//                     return;
//                 case Position.Util:
//                     serializer.Serialize(writer, "Util");
//                     return;
//             }
//             throw new Exception("Cannot marshal type Position");
//         }

//         public static readonly PositionConverter Singleton = new PositionConverter();
//     }



// public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
// {
//     if (untypedValue == null)
//     {
//         serializer.Serialize(writer, null);
//         return;
//     }
//     var value = (Position)untypedValue;
//     switch (value)
//     {
//         case Position.Dl:
//             serializer.Serialize(writer, "DL");
//             return;
//         case Position.P:
//             serializer.Serialize(writer, "P");
//             return;
//         case Position.Rp:
//             serializer.Serialize(writer, "RP");
//             return;
//         case Position.Sp:
//             serializer.Serialize(writer, "SP");
//             return;
//         case Position.The1B:
//             serializer.Serialize(writer, "1B");
//             return;
//         case Position.The2B:
//             serializer.Serialize(writer, "2B");
//             return;
//         case Position.The3B:
//             serializer.Serialize(writer, "3B");
//             return;
//         case Position.C:
//             serializer.Serialize(writer, "C");
//             return;
//         case Position.If:
//             serializer.Serialize(writer, "IF");
//             return;
//         case Position.Of:
//             serializer.Serialize(writer, "OF");
//             return;
//         case Position.Ss:
//             serializer.Serialize(writer, "SS");
//             return;
//         case Position.Util:
//             serializer.Serialize(writer, "Util");
//             return;
//     }
//     throw new Exception("Cannot marshal type Position");
// }

// public static readonly PositionConverter Singleton = new PositionConverter();
