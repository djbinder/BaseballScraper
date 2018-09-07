using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.MlbDataApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BaseballScraper.Infrastructure
{
    // code generated using https://app.quicktype.io/
    // the json that was inputted to app.quicktype.io was generated using Postman
    public class ApiSerializer
    {
        public static object FromJson(string json) => JsonConvert.DeserializeObject<object>(json, Converter.Settings);
    }

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
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    public class ApiInfrastructure
    {
        private Constants _c                          = new Constants();
        private static MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();

        /// <summary> Serialize a given object to a JSON stream (i.e., take a given object and convert it to JSON ) </summary>
        /// <param name="obj"> An object; typically a JObject (not certain how it deals with objects besides JObjects) </param>
        /// <returns></returns>
        public string ReturnJsonFromObject (Object obj)
        {
            // _c.Start.ThisMethod();
            //Create a stream to serialize the object to.
            MemoryStream mS = new MemoryStream();

            var objType = obj.GetType();
            Console.WriteLine($"OBJECT TYPE BEING SERIALIZED IS: {objType}");

            // Serializer the given object to the stream
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objType);

            serializer.WriteObject(mS, obj);
            byte[] json      = mS.ToArray();
                 mS.Position = 0;

            StreamReader sR = new StreamReader(mS);
            // this prints all object content in json format
            Console.WriteLine(sR.ReadToEnd());

            mS.Close();

            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public JObject CreateModelJObject(IRestResponse response)
        {
            var     responseToJson    = response.Content;
            JObject responseToJObject = JObject.Parse(responseToJson);
            // Extensions.PrintJObjectItems(responseToJObject);

            return responseToJObject;
        }

        /// <summary> Returns a JToken that lists keys/values for player items in PlayerSearch api </summary>
        /// <returns> A JToken</returns>
        public JToken CreateModelJToken(JObject obj, string modelType)
        {
            // _c.Start.ThisMethod();
            // Extensions.Spotlight("API SERIALIZER | Create model j token");
            // Console.WriteLine($"CREATING J TOKEN FOR MODEL TYPE: {modelType}");
            // Console.WriteLine();

            JToken modelToken = CreateObjectJTokenFromSwitch(obj, modelType);

            return modelToken;
        }

        public Object CreateInstanceOfModel (JToken token, Object obj, string modelType)
        {
            string tokenToString = token.ToString();

            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(tokenToString));

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

            obj = SetTypeToReadObjectAs(obj, memoryStream, serializer, modelType);

            memoryStream.Close();

            Extensions.Spotlight("CREATED MODEL INSTANCE");

            ReturnJsonFromObject(obj);

            return obj;
        }

        public Object CreateMultipleInstancesOfModelByLooping(JToken token, Object obj, string modelType)
        {
            int instance = 1;

            foreach(var playerObject in token)
            {
                string playerObjectToString = playerObject.ToString();

                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(playerObjectToString));

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

                obj = SetTypeToReadObjectAs(obj, memoryStream, serializer, modelType);

                memoryStream.Close();

                Extensions.Spotlight($"CREATED MODEL INSTANCE #{instance}");
                ReturnJsonFromObject(obj);

                instance++;
            }
            return obj;
        }

        internal Object SetTypeToReadObjectAs (Object obj, MemoryStream memoryStream, DataContractJsonSerializer serializer, string modelType)
        {
            switch(modelType)
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
            }
            throw new System.Exception("no model type found");
        }

        internal JToken CreateObjectJTokenFromSwitch(JObject obj, string tokenName)
        {
            switch(tokenName)
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
            }
            throw new System.Exception("no api type found");
        }
    }
}