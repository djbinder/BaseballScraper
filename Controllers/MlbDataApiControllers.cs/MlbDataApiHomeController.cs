using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.MlbDataApi;
using BaseballScraper.Postman;
using System.Reflection;

using MarkdownLog;
using MarkdownDeep;

namespace BaseballScraper.Controllers.MlbDataApiControllers
{
    [Route("api/mlb")]
    [ApiController]
    public class MlbDataApiHomeController: Controller
    {
        private Constants _c = new Constants();

        private static MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();

        private static PostmanMethods _postman = new PostmanMethods();


        [HttpGet]
        [Route("leaders/pitchingleaders")]
        public PitchingLeaders CreatePitchingLeadersModel ()
        {
            _c.Start.ThisMethod();

            var newEndPoint = _endPoints.PitchingLeadersEndPoint(2, "2018", "era");
            // Console.WriteLine(newEndPoint);

            var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PitchingLeaders");

            var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            var response = postmanResponse.Response;

            var responseToJson = response.Content;

            JObject obj = JObject.Parse(responseToJson);
            // Extensions.PrintJObjectItems(obj);

            var responseToJsonString = responseToJson.ToString();
            // Console.WriteLine(responseToJsonString);

            // type = BaseballScraper.Models.MlbDataApi.PitchingLeaders
            var pitchingLeaders = PitchingLeaders.FromJson(responseToJson);

            // System.Collections.Generic.Dictionary`2[System.String,System.String][]
            // The lengths of 'row' is equal to the first parameter passed when creating the endpoint
            var row = pitchingLeaders.LeaderPitchingRepeater.LeaderPitchingMux.QueryResults.Row;

            // R type = System.Collections.Generic.Dictionary`2[System.String,System.String]
                // R.KEYS ---> System.Collections.Generic.Dictionary`2+KeyCollection[System.String,System.String]
                // R.VALUES ---> System.Collections.Generic.Dictionary`2+ValueCollection[System.String,System.String]
            foreach(var r in row)
            {
                foreach(KeyValuePair<string,string> pair in r)
                {
                    Console.WriteLine("{0}, {1}", pair.Key, pair.Value);
                }
            }

            return pitchingLeaders;
        }

        // e.g., https://127.0.0.1:5001/api/mlb/bryant
        [HttpGet("{name}")]
        public PlayerSearch CreatePlayerSearchModel(string name)
        {
            _c.Start.ThisMethod();

            //  type ---> BaseballScraper.EndPoints.MlbDataApiEndPoints+MlbDataEndPoint
            var newEndPoint = _endPoints.PlayerSearchEndPoint(name);
                // PostmanRequest has Client(i.e., RestSharp.RestClient) and Request (i.e., RestSharp.RestRequest)
                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PlayerInfo");
                // PostmanResponse Class only has Response
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);
                // RESPONSE ---> RestSharp.RestResponse
                var response = postmanResponse.Response;
                // convert response content to json
                var responseToJson = response.Content;

            // clean up / better structure the json
            JObject obj = JObject.Parse(responseToJson);
            // Extensions.PrintJObjectItems(obj);

            // type = Newtonsoft.Json.Linq.JObject
            // this data is used to create a new instance of PlayerSearch
            var playerInfoItems = obj["search_player_all"]["queryResults"]["row"];

            #region CREATE NEW INSTANCE OF PLAYERSEARCH FROM JSON
                string playerInfoItemsToString = playerInfoItems.ToString();

                MemoryStream createPlayerMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(playerInfoItemsToString));

                PlayerSearch deserializedPlayer = new PlayerSearch();

                DataContractJsonSerializer createPlayerJsonSerializer = new DataContractJsonSerializer(deserializedPlayer.GetType());

                // deserializedPlayer = BaseballScraper.Models.MlbDataApi.PlayerSearch
                deserializedPlayer = createPlayerJsonSerializer.ReadObject(createPlayerMemoryStream) as PlayerSearch;
                createPlayerMemoryStream.Close();
            #endregion CREATE NEW INSTANCE OF PLAYERSEARCH FROM JSON

            // #region PRINT THE NEW PLAYERSEARCH
            //     MemoryStream printPlayerMemoryStream = new MemoryStream();

            //     DataContractJsonSerializer printPlayerJsonSerializer = new DataContractJsonSerializer(typeof(PlayerSearch));

            //     printPlayerJsonSerializer.WriteObject(printPlayerMemoryStream, deserializedPlayer);
            //     printPlayerMemoryStream.Position = 0;

            //     StreamReader playerStreamReader = new StreamReader(printPlayerMemoryStream);

            //     Console.WriteLine(playerStreamReader.ReadToEnd());
            //     printPlayerMemoryStream.Close();
            // #endregion PRINT THE NEW PLAYERSEARCH

            WriteFromObject(deserializedPlayer);

            return deserializedPlayer;
        }


        public string WriteFromObject (Object obj)
        {
            _c.Start.ThisMethod();

            MemoryStream mS = new MemoryStream();

            var objType = obj.GetType();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objType);

            serializer.WriteObject(mS, obj);
            byte[] json      = mS.ToArray();
                 mS.Position = 0;

            StreamReader sR = new StreamReader(mS);
            Console.WriteLine(sR.ReadToEnd());

            mS.Close();

            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
    }
}