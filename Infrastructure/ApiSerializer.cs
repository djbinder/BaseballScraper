using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BaseballScraper.EndPoints;
using Newtonsoft.Json.Linq;
using RestSharp;
using BaseballScraper.Models.MlbDataApi;


#pragma warning disable CA2000, CC0021, CC0091, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE0066, IDE0067, IDE0068, IDE1006, MA0016, MA0048
namespace BaseballScraper.Infrastructure
{
    public class ApiInfrastructure
    {
        private readonly Helpers _h = new Helpers();

        private static readonly MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();


        /// <summary> Serialize a given object to a JSON stream (i.e., take a given object and convert it to JSON ) </summary>
        /// <param name="obj"> An object; typically a JObject (not certain how it deals with objects besides JObjects) </param>
        /// <returns></returns>
        public string ReturnJsonFromObject (object obj)
        {
            //Create a stream to serialize the object to.
            MemoryStream mS = new MemoryStream();

            Type objType = obj.GetType();
            // Console.WriteLine($"ApiSerializer > ReturnJsonFromObject.objType: {objType}");

            // Serializer the given object to the stream
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objType);

            serializer.WriteObject(mS, obj);
            byte[] json      = mS.ToArray();
                 mS.Position = 0;

            StreamReader sR = new StreamReader(mS);
            // this prints all object content in json format
            // Console.WriteLine($"Streamreader: {sR.ReadToEnd()}");
            sR.Dispose();

            mS.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }


        public JObject CreateModelJObject(IRestResponse response)
        {
            string responseToJson    = response.Content;
            JObject responseToJObject = JObject.Parse(responseToJson);
            // Extensions.PrintJObjectItems(responseToJObject);
            return responseToJObject;
        }


        /// <summary> Returns a JToken that lists keys/values for player items in PlayerSearch api </summary>
        /// <param name="obj">todo: describe obj parameter on CreateModelJToken</param>
        /// <param name="modelType">todo: describe modelType parameter on CreateModelJToken</param>
        /// <returns> A JToken</returns>
        public JToken CreateModelJToken(JObject obj, string modelType)
        {
            // _h.StartMethod();
            // _h.Spotlight("API SERIALIZER | Create model j token");
            // Console.WriteLine($"CREATING J TOKEN FOR MODEL TYPE: {modelType}");
            // Console.WriteLine();

            return CreateObjectJTokenFromSwitch(obj, modelType);
        }

        // May 22, 2019 status: this works
        // Example: check MlbDataPlayerTeams.GetTeamsforPlayerAllSeasons
        // IMPORTANT: every attribute in the model needs to have the 'DataMember' tag
            // E.g., [DataMember(Name="season_state")]
        public object CreateInstanceOfModel (JToken token, object obj, string modelType)
        {
            string tokenToString = token.ToString();

            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(tokenToString));

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

            obj = SetTypeToReadObjectAs(memoryStream, serializer, modelType);

            memoryStream.Close();

            // _h.Spotlight($"CREATED MODEL INSTANCE for {obj.GetType()}");

            ReturnJsonFromObject(obj);
            // _h.Intro(obj,"obj");
            // Console.WriteLine($"Object type: {obj.GetType()}");

            return obj;
        }


        // May 22, 2019 status: this works
        // Example: check MlbDataPlayerTeams.GetTeamsforPlayerAllSeasons
        // IMPORTANT: every attribute in the model needs to have the 'DataMember' tag
            // E.g., [DataMember(Name="season_state")]
        public object CreateMultipleInstancesOfModelByLooping(JToken token, object obj, string modelType)
        {
            int instance = 1;

            foreach(var playerObject in token)
            {
                string playerObjectToString = playerObject.ToString();

                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(playerObjectToString));

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

                obj = SetTypeToReadObjectAs(memoryStream, serializer, modelType);

                memoryStream.Close();

                // _h.Spotlight($"CREATED MODEL INSTANCE #{instance} for {obj.GetType()}");
                ReturnJsonFromObject(obj);
                instance++;
            }
            return obj;
        }


        // May 22, 2019 status: this kind of works
        // Example:
            // List<dynamic> ptList2 = new List<dynamic>();
            // PlayerTeam pTeam2 = new PlayerTeam();
            // var objList = _apI.CreateListWithMultipleInstances(allTeamValuesJToken,pTeam2,ptList2,"PlayerTeam");
            // var firstValue = objList[0] as PlayerTeam;
            // Console.WriteLine(firstValue.OrgFull);
        public List<dynamic> CreateListWithMultipleInstances(JToken token, object obj, List<dynamic> objList, string modelType)
        {
            foreach(var playerObject in token)
            {
                string playerObjectToString = playerObject.ToString();

                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(playerObjectToString));

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

                obj = SetTypeToReadObjectAs(memoryStream, serializer, modelType);

                memoryStream.Close();

                // _h.Spotlight($"CREATED MODEL INSTANCE #{instance} for {obj.GetType()}");
                ReturnJsonFromObject(obj);
                objList.Add(obj);
            }

            var listCount = objList.Count;
            for(var jsonIndex = 0; jsonIndex <= listCount - 1; jsonIndex++)
            {
                var jsonGrouping = objList[jsonIndex];
                Console.WriteLine(jsonGrouping);
            }

            return objList;
        }


        internal object SetTypeToReadObjectAs (MemoryStream memoryStream, DataContractJsonSerializer serializer, string modelType)
        {
            switch (modelType)
            {
                case "PlayerSearch":
                    return serializer.ReadObject(memoryStream) as PlayerSearch;
                case "PlayerInfo":
                    return serializer.ReadObject(memoryStream) as PlayerInfo;
                case "ProjectedPitchingStats":
                    return serializer.ReadObject(memoryStream) as ProjectedPitchingStats;
                case "ProjectedHittingStats":
                    return serializer.ReadObject(memoryStream) as ProjectedHittingStats;
                case "PitchingLeaders":
                    return serializer.ReadObject(memoryStream) as PitchingLeaders;
                case "HittingLeaders":
                    return serializer.ReadObject(memoryStream) as HittingLeaders;
                case "LeadingPitcher":
                    return serializer.ReadObject(memoryStream) as LeadingPitcher;
                case "LeadingHitter":
                    return serializer.ReadObject(memoryStream) as LeadingHitter;
                case "HitterSeasonStats":
                    return serializer.ReadObject(memoryStream) as HitterSeasonStats;
                case "PlayerTeams":
                    return serializer.ReadObject(memoryStream) as PlayerTeams;
                case "PlayerTeam":
                    return serializer.ReadObject(memoryStream) as PlayerTeam;
                default:
                    break;
            }
            throw new Exception("no model type found");
        }


        internal JToken CreateObjectJTokenFromSwitch(JObject obj, string tokenName)
        {
            switch (tokenName)
            {
                case "PlayerSearch":
                    return obj["search_player_all"]["queryResults"]["row"];

                case "PlayerInfo":
                    return obj["player_info"]["queryResults"]["row"];

                case "ProjectedPitchingStats":
                    return obj["proj_pecota_pitching"]["queryResults"]["row"];

                case "ProjectedHittingStats":
                    return obj["proj_pecota_batting"]["queryResults"]["row"];

                case "PitchingLeaders":
                    return obj["leader_pitching_repeater"]["leader_pitching_mux"]["queryResults"]["row"];

                case "HittingLeaders":
                    return obj["leader_hitting_repeater"]["leader_hitting_mux"]["queryResults"]["row"];

                case "LeadingPitcher":
                    return obj["leader_pitching_repeater"]["leader_pitching_mux"]["queryResults"]["row"];

                case "LeadingHitter":
                    return obj["leader_hitting_repeater"]["leader_hitting_mux"]["queryResults"]["row"];

                case "HitterSeasonStats":
                    return obj["sport_hitting_tm"]["queryResults"]["row"];

                case "PlayerTeams":
                    return obj["player_teams"];

                case "PlayerTeam":
                    return obj["player_teams"]["queryResults"]["row"];
                default:
                    break;
            }
            throw new Exception("no api type found");
        }
    }



    // code generated using https://app.quicktype.io/
    // the json that was inputted to app.quicktype.io was generated using Postman
    // public class ApiSerializer
    // {
    //     public static object FromJson(string json) => JsonConvert.DeserializeObject<object>(json, Converter.Settings);
    // }


    // code generated using https://app.quicktype.io/
    // the json that was inputted to app.quicktype.io was generated using Postman
    public static class Serialize
    {
        public static string ToJson(this object self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }


    // code generated using https://app.quicktype.io/
    // the json that was inputted to app.quicktype.io was generated using Postman
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling        = DateParseHandling.None,
            Converters               = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal, },
            },
        };
    }


    // code generated using https://app.quicktype.io/
    // the json that was inputted to app.quicktype.io was generated using Postman
    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
                if (Int64.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out long l))
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
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString(CultureInfo.InvariantCulture));
            return;
        }

        // code generated using https://app.quicktype.io/
        // the json that was inputted to app.quicktype.io was generated using Postman
        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
