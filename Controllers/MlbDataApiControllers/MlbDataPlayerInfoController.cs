using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.MlbDataApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Linq;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.MlbDataApiControllers
{

    [Route("api/mlb/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MlbDataPlayerInfoController: ControllerBase
    {
        private readonly Helpers             _helpers;
        private readonly ApiInfrastructure   _apiInfrastructure;
        private readonly MlbDataApiEndPoints _endPoints;
        private readonly PostmanMethods      _postman;

        private readonly string _testIdString = "493316";
        private readonly int _testIdInt = 493316;


        public MlbDataPlayerInfoController(Helpers helpers, ApiInfrastructure apiInfrastructure, MlbDataApiEndPoints endPoints, PostmanMethods postman)
        {
            _helpers           = helpers;
            _apiInfrastructure = apiInfrastructure;
            _endPoints         = endPoints;
            _postman           = postman;
        }

        public MlbDataPlayerInfoController(){}


        /*
            https://127.0.0.1:5001/api/mlb/MlbDataPlayerInfo/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            var playerInfo = CreatePlayerInfoInstance(_testIdInt);
        }


        // https://appac.github.io/mlb-data-api-docs/#player-data-player-info-get
        /// <summary>
        ///     View instantiated player model with player's info
        /// </summary>
        /// <returns>
        ///     A view of a playerinfo model
        /// </returns>
        public PlayerInfo CreatePlayerInfoInstance(int playerId)
        {
            _helpers.StartMethod();
            Console.WriteLine($"GETTING INFO FOR PLAYER ID: {playerId}");

            IRestResponse response  = GetPlayerInfoPostmanResponse(playerId);
            JObject playerJObject   = _apiInfrastructure.CreateModelJObject(response);
            JToken playerJToken     = _apiInfrastructure.CreateModelJToken(playerJObject, "PlayerInfo");
            int jTokenChildrenCount = playerJToken.Count<object>();

            Console.WriteLine($"jTokenChildrenCount: {jTokenChildrenCount}");

            PlayerInfo newPlayerInfoInstance = new PlayerInfo();

            newPlayerInfoInstance = _apiInfrastructure.CreateInstanceOfModel(playerJToken, newPlayerInfoInstance, "PlayerInfo") as PlayerInfo;

            _helpers.Dig(newPlayerInfoInstance);

            return newPlayerInfoInstance;
        }

        public IRestResponse GetPlayerInfoPostmanResponse(int playerId)
        {
            var newEndPoint     = _endPoints.PlayerInfoEndPoint(playerId);
            var postmanRequest  = _postman.CreatePostmanRequest(newEndPoint, "PlayerInfo");
            var postmanResponse = _postman.GetPostmanResponse(postmanRequest);
            var response        = postmanResponse.Response;

            return response;
        }
    }
}
