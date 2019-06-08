using System;
using System.IO;
using BaseballScraper.Models.Configuration;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.Infrastructure
{
    public class JsonHandler
    {
        private static readonly Helpers _h = new Helpers();

        public static readonly string yahooConfigFilePath = "Configuration/yahooConfig.json";

        public YahooConfiguration _yConfig = new YahooConfiguration();



        public void JsonReaderTester()
        {
            // NewtonsoftJsonHandlers.DeserializeJsonFromFile(yahooConfigFilePath, typeof(YahooConfiguration));


            // VersionConverter.Convert(yahooConfigFilePath, _yConfig.GetType());

        }


        public class DotNetStandardJsonHandler
        {
            public static MemoryStream memoryStream = new MemoryStream();

            // STATUS [ June 6, 2019 ] : this works
            // https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data
            public void ReadJsonFile(string filePath)
            {
                var configData = JsonConvert.DeserializeObject<YahooConfiguration>(File.ReadAllText(filePath));

                DataContractJsonSerializer dcJr = new DataContractJsonSerializer(typeof(YahooConfiguration));

                dcJr.WriteObject(memoryStream, configData);

                memoryStream.Position = 0;

                StreamReader configReader = new StreamReader(memoryStream);

                Console.WriteLine(configReader.ReadToEnd());
            }


            // STATUS [ June 6, 2019 ] : this works
            // https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data
            public void DeserializeTypeInstanceFromJson()
            {
                memoryStream.Position = 0;

                DataContractJsonSerializer dcJr = new DataContractJsonSerializer(typeof(YahooConfiguration));

                var configData = (YahooConfiguration)dcJr.ReadObject(memoryStream);

                Console.WriteLine();
                Console.WriteLine(configData);
            }

        }


        public class NewtonsoftJsonHandlers
        {
            public NewtonsoftJsonHandlers() {}


            // public void JsonTextReader()
            // {
            //     JsonTextReader tR = new JsonTextReader()
            // }


            // STATUS [ June 6, 2019 ] : this works
            // https://www.newtonsoft.com/json/help/html/CustomJsonConverterGeneric.htm
            public Object Convert(string jsonFilePath, Type type)
            {
                var obj = NewtonsoftJsonHandlers.DeserializeJsonFromFileStatic(jsonFilePath, type);
                // Console.WriteLine($"Convert object type: {obj.GetType()}");
                return obj;
            }


            // STATUS [ June 6, 2019 ] : this works
            // https://www.newtonsoft.com/json/help/html/SerializeObject.htm
            // NewtonsoftJsonHandlers.SerializeObject(yahooConfigFilePath);
            public String SerializeObjectToString(string filePath)
            {
                var data = DeserializeJsonToTypeFromString(filePath);

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    // Console.WriteLine(json);

                return json;
            }

            // STATUS [ June 6, 2019 ] : this works
            // https://www.newtonsoft.com/json/help/html/DeserializeWithJsonSerializerFromFile.htm
            public static Object DeserializeJsonToTypeFromString(string filePath)
            {
                var configData = JsonConvert.DeserializeObject<YahooConfiguration>(File.ReadAllText(filePath));
                // Console.WriteLine(configData);
                return configData;
            }

            // STATUS [ June 6, 2019 ] : this works
            // https://www.newtonsoft.com/json/help/html/DeserializeWithJsonSerializerFromFile.htm
            public Object DeserializeJsonFromFile(string filePath, Type type)
            {
                var checkType = type.GetType();
                // Console.WriteLine($"checkType: {checkType}");

                using(StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var obj = serializer.Deserialize(file, type);

                    var objType = obj.GetType();
                    // Console.WriteLine($"Serialized type is: {objType}");

                    return obj;
                }
            }

            public static Object DeserializeJsonFromFileStatic(string filePath, Type type)
            {
                var checkType = type.GetType();
                // Console.WriteLine($"checkType: {checkType}");

                using(StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var obj = serializer.Deserialize(file, type);

                    var objType = obj.GetType();
                    // Console.WriteLine($"Serialized type is: {objType}");

                    return obj;
                }
            }
        }
    }
}



// Console.WriteLine(configData);
// Console.WriteLine();
// Console.WriteLine(configDataJson);
// Console.WriteLine();
// Console.WriteLine(configDataJsonItems);

// Console.WriteLine($"Name: {yahooConfig.Name}");
// Console.WriteLine($"AppId: {yahooConfig.AppId}");
// Console.WriteLine();
// Console.WriteLine($"Base64 Encoding: {yahooConfig.Base64Encoding}");
// Console.WriteLine();
// Console.WriteLine($"Refresh Token: {yahooConfig.RefreshToken}");
// Console.WriteLine();
// Console.WriteLine($"Client Id: {yahooConfig.ClientId}");
// Console.WriteLine();
// Console.WriteLine($"Client Secret: {yahooConfig.ClientSecret}");
// Console.WriteLine();



// public static Object DeserializeAnonymousType()
// {
//     // var item = DeserializeJsonFromFile(yahooConfigFilePath, typeof(YahooConfiguration));
//     var item = new YahooConfiguration();

//     var json = DeserializeJsonFromFile(yahooConfigFilePath, typeof(YahooConfiguration));
//     var jsonString = json.ToString();
//     var x = JsonConvert.DeserializeAnonymousType(jsonString, item);

//     Console.WriteLine(x);

//     return x;
// }
