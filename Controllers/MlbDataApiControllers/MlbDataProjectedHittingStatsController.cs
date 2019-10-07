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

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.MlbDataApiControllers
{
    [Route("api/mlb/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MlbDataProjectedHittingStatsController: ControllerBase
    {
        private readonly Helpers             _helpers;
        private readonly ApiInfrastructure   _apiInfrastructure;
        private readonly MlbDataApiEndPoints _endPoints;
        private readonly PostmanMethods      _postman;


        public MlbDataProjectedHittingStatsController(Helpers helpers, ApiInfrastructure apiInfrastructure, MlbDataApiEndPoints endPoints, PostmanMethods postman)
        {
            _helpers           = helpers;
            _apiInfrastructure = apiInfrastructure;
            _endPoints         = endPoints;
            _postman           = postman;
        }



        // https://appac.github.io/mlb-data-api-docs/#stats-data-projected-hitting-stats-get
        /// <summary> View instantiated pecota projections for a selected hitter in a selected season  </summary>
        /// <param name="playerId">todo: describe playerId parameter on ViewPlayerInfo</param>
        /// <example> https://127.0.0.1:5001/api/mlb/projectedhittingstats/493316 </example>
        /// <returns> A view of a projectedhittingstats model </returns>
        [Route("projectedhittingstats/{playerId}")]
        public IActionResult ViewPlayerInfo(int playerId)
        {
            // this gets you Cespedes
            // int playerIdPlaceHolder = 493316;
            Console.WriteLine($"GETTING INFO FOR PLAYER ID: {playerId}");

            IRestResponse response = GetProjectedHittingStatsPostmanResponse(playerId);

            JObject playerJObject = _apiInfrastructure.CreateModelJObject(response);

            JToken playerJToken = _apiInfrastructure.CreateModelJToken(playerJObject, "ProjectedHittingStats");

            ProjectedHittingStats newInstance = new ProjectedHittingStats();

            _apiInfrastructure.CreateInstanceOfModel(playerJToken, newInstance, "ProjectedHittingStats");

            return Content($"{playerJToken}");
        }


        public IRestResponse GetProjectedHittingStatsPostmanResponse(int playerId)
        {
            _helpers.StartMethod();

            // type ---> BaseballScraper.EndPoints.MlbDataApiEndPoints+MlbDataEndPoint
            var newEndPoint = _endPoints.ProjectedHittingStatsEndPoint(2017, playerId);

            // PostmanRequest has Client(i.e., RestSharp.RestClient) and Request (i.e., RestSharp.RestRequest)
            var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "ProjectedHittingStats");

            // PostmanResponse Class only has Response
            var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            IRestResponse response = postmanResponse.Response;

            return response;
        }
    }
}
