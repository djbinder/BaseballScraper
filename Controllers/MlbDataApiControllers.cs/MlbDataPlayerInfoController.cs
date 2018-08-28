using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.MlbDataApi;
using BaseballScraper.Controllers.MlbDataApiControllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using System;

namespace BaseballScraper.Controllers.MlbDataApiControllers
{
    [Route("api/mlb")]
    [ApiController]
    public class MlbDataPlayerInfoController: Controller
    {
        private Constants _c                          = new Constants();
        private ApiInfrastructure _a                  = new ApiInfrastructure();
        private static MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();
        private static PostmanMethods _postman        = new PostmanMethods();


        // https://appac.github.io/mlb-data-api-docs/#player-data-player-info-get
        /// <summary> View instantiated player model with player's info </summary>
        /// <returns> A view of a playerinfo model </returns>
        [Route("playerinfo/{playerId}")]
        public IActionResult ViewPlayerInfo(int playerId)
        {
            _c.Start.ThisMethod();

            // this gets you Cespedes
            // int playerIdPlaceHolder = 493316;
            Console.WriteLine($"GETTING INFO FOR PLAYER ID: {playerId}");

            IRestResponse response = GetPlayerInfoPostmanResponse(playerId);

            JObject playerJObject = _a.CreateModelJObject(response);

            JToken playerJToken = _a.CreateModelJToken(playerJObject, "PlayerInfo");

            PlayerInfo newPlayerInfoInstance = new PlayerInfo();

            _a.CreateInstanceOfModel(playerJToken, newPlayerInfoInstance, "PlayerInfo");

            return Content($"{playerJToken}");
        }

        public IRestResponse GetPlayerInfoPostmanResponse(int playerId)
        {
            _c.Start.ThisMethod();

            // type ---> BaseballScraper.EndPoints.MlbDataApiEndPoints+MlbDataEndPoint
            var newEndPoint = _endPoints.PlayerInfoEndPoint(playerId);

            // type --> PostmanRequest
            // PostmanRequest has Client(i.e., RestSharp.RestClient) and Request (i.e., RestSharp.RestRequest)
            var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PlayerInfo");

            // type --> PostmanResponse
            // PostmanResponse Class only has Response
            var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            // type --> IRestResponse
            var response = postmanResponse.Response;

            return response;
        }
    }
}