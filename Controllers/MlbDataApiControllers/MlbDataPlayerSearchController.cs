using BaseballScraper.EndPoints;
using BaseballScraper.Models.MlbDataApi;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using static BaseballScraper.Infrastructure.PostmanMethods;
using C = System.Console;
using System.Linq;
using System.Collections.Generic;


#pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, IDE0052
namespace BaseballScraper.Controllers.MlbDataApiControllers
{

    [Route("api/mlb/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MlbDataPlayerSearchController: ControllerBase
    {
        private readonly Helpers             _helpers;
        private readonly ApiInfrastructure   _apiInfrastructure;
        private readonly MlbDataApiEndPoints _endPoints;
        private readonly PostmanMethods      _postman;


        public MlbDataPlayerSearchController(Helpers helpers, ApiInfrastructure apiInfrastructure, MlbDataApiEndPoints endPoints, PostmanMethods postman)
        {
            _helpers           = helpers;
            _apiInfrastructure = apiInfrastructure;
            _endPoints         = endPoints;
            _postman           = postman;
        }

        public MlbDataPlayerSearchController(){}


        /*
            https://127.0.0.1:5001/api/mlb/mlbdataplayersearch/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            ViewPlayerSearchModel("Baez");
        }


        // STATUS [ July 15, 2019 ] : this works
        /// <summary>
        ///     View instantiated PlayerSearch object
        /// </summary>
        /// <remarks>
        ///     See: https://appac.github.io/mlb-data-api-docs/#player-data-player-search-get
        /// </remarks>
        /// <returns>
        ///     Instantiated PlayerSearch
        /// </returns>
        public List<PlayerSearch> ViewPlayerSearchModel(string playerLastName)
        {
            IRestResponse response  = GetPlayerSearchModelPostmanResponse(playerLastName);
            JObject playerJObject   = _apiInfrastructure.CreateModelJObject(response);
            JToken playerJToken     = _apiInfrastructure.CreateModelJToken(playerJObject, "PlayerSearch");
            int jTokenChildrenCount = playerJToken.Count<object>();

            List<PlayerSearch> listOfPlayers     = new List<PlayerSearch>();
            PlayerSearch newPlayerSearchInstance = new PlayerSearch();

            for(var counter = 0; counter < jTokenChildrenCount; counter++)
            {
                PlayerSearch playerSearchInstance =
                    _apiInfrastructure.CreateInstanceOfModel
                    (
                        playerJToken[counter],
                        newPlayerSearchInstance,
                        "PlayerSearch"
                    ) as PlayerSearch;

                listOfPlayers.Add(playerSearchInstance);
            }
            return listOfPlayers;
        }

        // STATUS [ July 15, 2019 ] : this works
        public IRestResponse GetPlayerSearchModelPostmanResponse(string playerLastName)
        {
            var newEndPoint                 = _endPoints.PlayerSearchEndPoint(playerLastName);
            PostmanRequest postmanRequest   = _postman.CreatePostmanRequest(newEndPoint, "PlayerSearch");
            PostmanResponse postmanResponse = _postman.GetPostmanResponse(postmanRequest);
            IRestResponse response          = postmanResponse.Response;

            return response;
        }
    }
}



// EXAMPLE OF RETURNED JSON:

// {
//   "position": "1B",
//   "birth_country": "USA",
//   "weight": 240,
//   "birth_state": "FL",
//   "name_display_first_last": "Anthony Rizzo",
//   "college": "",
//   "height_inches": 3,
//   "name_display_roster": "Rizzo",
//   "sport_code": "mlb",
//   "bats": "L",
//   "name_first": "Anthony",
//   "team_code": "chn",
//   "birth_city": "Fort Lauderdale",
//   "height_feet": "6",
//   "pro_debut_date": "2011-06-09T00:00:00",
//   "team_full": "Chicago Cubs",
//   "team_abbrev": "CHC",
//   "birth_date": "1989-08-08T00:00:00",
//   "throws": "L",
//   "league": "NL",
//   "name_display_last_first": "Rizzo, Anthony",
//   "position_id": 3,
//   "high_school": "Marjory Stoneman Douglas, Parkland, FL",
//   "name_use": "Anthony",
//   "player_id": 519203,
//   "name_last": "Rizzo",
//   "team_id": 112,
//   "service_years": "",
//   "active_sw": "Y"
// }
