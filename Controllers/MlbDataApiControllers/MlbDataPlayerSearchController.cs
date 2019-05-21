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
    #pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, IDE0052
    public class MlbDataPlayerSearchController: Controller
    {
        private readonly Helpers _h                            = new Helpers();
        private readonly ApiInfrastructure _a                  = new ApiInfrastructure();
        private static readonly MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();
        private static readonly PostmanMethods _postman        = new PostmanMethods();


        // https://appac.github.io/mlb-data-api-docs/#player-data-player-search-get
        /// <summary> View instantiated PlayerSearch object  </summary>
        /// <returns> Instantiated PlayerSearch </returns>
        [Route("playersearch/{playerLastName}")]
        public IActionResult ViewPlayerSearchModel(string playerLastName)
        {
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
            // type ---> BaseballScraper.EndPoints.MlbDataApiEndPoints+MlbDataEndPoint
            var newEndPoint = _endPoints.PlayerSearchEndPoint(name);

            // PostmanRequest has Client(i.e., RestSharp.RestClient) and Request (i.e., RestSharp.RestRequest)
            var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PlayerSearch");

            // PostmanResponse Class only has Response
            var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            IRestResponse response = postmanResponse.Response;

            return response;
        }


        public void GetPlayerHittingStats()
        {

        }
    }
}
