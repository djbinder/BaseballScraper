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


        // https://appac.github.io/mlb-data-api-docs/#player-data-player-search-get
        /// <summary> View instantiated PlayerSearch object  </summary>
        /// <returns> Instantiated PlayerSearch </returns>
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
            var response = postmanResponse.Response;

            return response;
        }
    }
}