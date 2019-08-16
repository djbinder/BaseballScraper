using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.MlbDataApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.MlbDataApiControllers.cs
{
    [Route("api/mlb/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MlbDataProjectedPitchingStatsController: ControllerBase
    {
        private readonly Helpers _h                            = new Helpers();
        private readonly ApiInfrastructure _a                  = new ApiInfrastructure();
        private static readonly MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();
        private static readonly PostmanMethods _postman        = new PostmanMethods();

        // https://appac.github.io/mlb-data-api-docs/#stats-data-projected-pitching-stats-get
        /// <summary> View instantiated pecota projections for a selected pitcher in a selected season  </summary>
        /// <example> https://127.0.0.1:5001/api/mlb/projectedpitchingstats/592789 </example>
        /// <returns> A view of a projectedpitchingstats model </returns>
        [Route("projectedpitchingstats/{playerId}")]
        public IActionResult ViewPlayerInfo(int playerId)
        {
            // this gets you Syndergaard
            // int playerIdPlaceHolder = 592789;
            Console.WriteLine($"GETTING INFO FOR PLAYER ID: {playerId}");

            IRestResponse response = GetProjectedPitchingStatsPostmanResponse(playerId);

            JObject playerJObject = _a.CreateModelJObject(response);

            JToken playerJToken = _a.CreateModelJToken(playerJObject, "ProjectedPitchingStats");

            ProjectedPitchingStats newInstance = new ProjectedPitchingStats();

            _a.CreateInstanceOfModel(playerJToken, newInstance, "ProjectedPitchingStats");

            return Content($"{playerJToken}");
        }


        public IRestResponse GetProjectedPitchingStatsPostmanResponse(int playerId)
        {
            // type ---> BaseballScraper.EndPoints.MlbDataApiEndPoints+MlbDataEndPoint
            var newEndPoint = _endPoints.ProjectedPitchingStatsEndPoint(2018, playerId);

            // PostmanRequest has Client(i.e., RestSharp.RestClient) and Request (i.e., RestSharp.RestRequest)
            var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "ProjectedPitchingStats");

            // PostmanResponse Class only has Response
            var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            IRestResponse response = postmanResponse.Response;

            return response;
        }
    }
}
