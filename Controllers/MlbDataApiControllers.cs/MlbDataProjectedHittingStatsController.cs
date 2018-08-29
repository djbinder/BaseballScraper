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
    public class MlbDataProjectedHittingStatsController: Controller
    {
        private Constants _c                          = new Constants();
        private ApiInfrastructure _a                  = new ApiInfrastructure();
        private static MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();
        private static PostmanMethods _postman        = new PostmanMethods();

        // https://appac.github.io/mlb-data-api-docs/#stats-data-projected-hitting-stats-get
        /// <summary> View instantiated pecota projections for a selected hitter in a selected season  </summary>
        /// <example> https://127.0.0.1:5001/api/mlb/projectedhittingstats/493316 </example>
        /// <returns> A view of a projectedhittingstats model </returns>
        [Route("projectedhittingstats/{playerId}")]
        public IActionResult ViewPlayerInfo(int playerId)
        {
            _c.Start.ThisMethod();

            // this gets you Cespedes
            // int playerIdPlaceHolder = 493316;
            Console.WriteLine($"GETTING INFO FOR PLAYER ID: {playerId}");

            IRestResponse response = GetProjectedHittingStatsPostmanResponse(playerId);

            JObject playerJObject = _a.CreateModelJObject(response);

            JToken playerJToken = _a.CreateModelJToken(playerJObject, "ProjectedHittingStats");

            ProjectedHittingStats newInstance = new ProjectedHittingStats();

            _a.CreateInstanceOfModel(playerJToken, newInstance, "ProjectedHittingStats");

            return Content($"{playerJToken}");
        }


        public IRestResponse GetProjectedHittingStatsPostmanResponse(int playerId)
        {
            _c.Start.ThisMethod();

            // type ---> BaseballScraper.EndPoints.MlbDataApiEndPoints+MlbDataEndPoint
            var newEndPoint = _endPoints.ProjectedHittingStatsEndPoint(2017, playerId);

            // type --> PostmanRequest
            // PostmanRequest has Client(i.e., RestSharp.RestClient) and Request (i.e., RestSharp.RestRequest)
            var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "ProjectedHittingStats");

            // type --> PostmanResponse
            // PostmanResponse Class only has Response
            var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            // type --> IRestResponse
            var response = postmanResponse.Response;

            return response;
        }
    }
}