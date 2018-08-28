using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using BaseballScraper.Controllers.MlbDataApiControllers;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.MlbDataApi;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BaseballScraper.Controllers.MlbDataApiControllers
{
    [Route("api/mlb")]
    [ApiController]
    public class MlbDataPlayerSearchController: Controller
    {
        private Constants _c                          = new Constants();
        private ApiInfrastructure _a                  = new ApiInfrastructure();
        private static MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();
        private static PostmanMethods _postman        = new PostmanMethods();



        [Route("playersearch/{playerLastName}")]
        public IActionResult ViewPlayerSearchModel(string playerLastName)
        {
            _c.Start.ThisMethod();

            Console.WriteLine($"SEARCHING FOR PLAYER: {playerLastName}");

            IRestResponse response = GetPlayerSearchModelPostmanResponse(playerLastName);

            JObject playerJObject = _a.CreateModelJObject(response);

            JToken playerJToken = _a.CreateModelJToken(playerJObject, "PlayerSearch");

            PlayerSearch newPlayerSearchInstance = new PlayerSearch();

            _a.CreateInstanceOfModel(playerJToken, newPlayerSearchInstance, "PlayerSearch");

            return Content($"{playerJToken}");
        }

        public IRestResponse GetPlayerSearchModelPostmanResponse(string name)
        {
            _c.Start.ThisMethod();

            // type ---> BaseballScraper.EndPoints.MlbDataApiEndPoints+MlbDataEndPoint
            var newEndPoint = _endPoints.PlayerSearchEndPoint(name);

            // type --> PostmanRequest
            // PostmanRequest has Client(i.e., RestSharp.RestClient) and Request (i.e., RestSharp.RestRequest)
            var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PlayerSearch");

            // type --> PostmanResponse
            // PostmanResponse Class only has Response
            var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            // type --> IRestResponse
            // RESPONSE ---> RestSharp.RestResponse
            var response = postmanResponse.Response;

            return response;
        }

        // /// <summary> Returns a JToken that lists keys/values for player items in PlayerSearch api </summary>
        // /// <param name="name"> The Mlb player's last name </param>
        // /// <example> https://127.0.0.1:5001/api/mlb/bryant </example>
        // /// <returns></returns>
        // public JObject CreatePlayerSearchModelJObject(IRestResponse response)
        // {
        //     _c.Start.ThisMethod();

        //     var responseToJson = response.Content;
        //     // clean up / better structure the json
        //     JObject playerSearchJObject = JObject.Parse(responseToJson);
        //     // Extensions.PrintJObjectItems(playerSearchJObject);

        //     CreatePlayerSearchModelJToken(playerSearchJObject);
        //     return playerSearchJObject;
        // }

        // public JToken CreatePlayerSearchModelJToken(JObject obj)
        // {
        //     _c.Start.ThisMethod();

        //     // type = Newtonsoft.Json.Linq.JObject
        //     // this data is used to create a new instance of PlayerSearch
        //     JToken playerInfoItems = obj["search_player_all"]["queryResults"]["row"];

        //     return playerInfoItems;
        // }


        // public PlayerSearch CreatePlayerSearchModel (JToken token)
        // {
        //     _c.Start.ThisMethod();
        //     // Console.WriteLine($"OBJECT TYPE IS: {objectType}");

        //     string tokenToString = token.ToString();

        //     MemoryStream createPlayerMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(tokenToString));

        //     PlayerSearch deserializedPlayer = new PlayerSearch();

        //     DataContractJsonSerializer createPlayerJsonSerializer = new DataContractJsonSerializer(deserializedPlayer.GetType());

        //     // deserializedPlayer = BaseballScraper.Models.MlbDataApi.PlayerSearch
        //     deserializedPlayer = createPlayerJsonSerializer.ReadObject(createPlayerMemoryStream) as PlayerSearch;
        //     createPlayerMemoryStream.Close();

        //     Extensions.Spotlight("begin writing from object");
        //     _a.ReturnJsonFromObject(deserializedPlayer);

        //     return deserializedPlayer;
        // }
    }
}