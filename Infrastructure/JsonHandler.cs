using System;
using System.IO;
using BaseballScraper.Models.Configuration;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Infrastructure
{
    public class JsonHandler
    {
        private static readonly Helpers _h = new Helpers();

        public static readonly string yahooConfigFilePath = "Configuration/yahooConfig.json";

        public YahooConfiguration _yConfig = new YahooConfiguration();



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
                Console.WriteLine($"\n{configData}");
            }

        }


        public class NewtonsoftJsonHandlers
        {
            public NewtonsoftJsonHandlers() {}


            // STATUS [ June 6, 2019 ] : this works
            // https://www.newtonsoft.com/json/help/html/CustomJsonConverterGeneric.htm
            public object Convert(string jsonFilePath, Type type)
            {
                var obj = DeserializeJsonFromFileStatic(jsonFilePath, type);
                return obj;
            }


            // STATUS [ June 6, 2019 ] : this works
            // https://www.newtonsoft.com/json/help/html/SerializeObject.htm
            // NewtonsoftJsonHandlers.SerializeObject(yahooConfigFilePath);
            public string SerializeObjectToString(string filePath)
            {
                var data = DeserializeJsonToTypeFromString(filePath);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                return json;
            }

            // STATUS [ June 6, 2019 ] : this works
            // https://www.newtonsoft.com/json/help/html/DeserializeWithJsonSerializerFromFile.htm
            public static object DeserializeJsonToTypeFromString(string filePath)
            {
                var configData = JsonConvert.DeserializeObject<YahooConfiguration>(File.ReadAllText(filePath));
                return configData;
            }

            // STATUS [ June 6, 2019 ] : this works
            // https://www.newtonsoft.com/json/help/html/DeserializeWithJsonSerializerFromFile.htm
            public object DeserializeJsonFromFile(string filePath, Type type)
            {
                var checkType = type.GetType();

                using(StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var obj = serializer.Deserialize(file, type);
                    var objType = obj.GetType();
                    return obj;
                }
            }

            public static object DeserializeJsonFromFileStatic(string filePath, Type type)
            {
                var checkType = type.GetType();

                using(StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var obj = serializer.Deserialize(file, type);
                    var objType = obj.GetType();
                    return obj;
                }
            }
        }
    }
}
