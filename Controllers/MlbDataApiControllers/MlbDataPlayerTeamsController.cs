using System;
using System.Collections.Generic;
using System.Globalization;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.MlbDataApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using static BaseballScraper.EndPoints.MlbDataApiEndPoints;
using static BaseballScraper.Infrastructure.PostmanMethods;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006, MA0016
namespace BaseballScraper.Controllers.MlbDataApiControllers
{

    [Route("api/mlb/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MlbDataPlayerTeamsController : ControllerBase
    {
        private readonly Helpers             _helpers;
        private readonly MlbDataApiEndPoints _endPoints;
        private readonly PostmanMethods      _postman;
        private readonly ApiInfrastructure   _apI;


        public MlbDataPlayerTeamsController(Helpers helpers, MlbDataApiEndPoints endPoints, PostmanMethods postman, ApiInfrastructure apI)
        {
            _helpers   = helpers;
            _endPoints = endPoints;
            _postman   = postman;
            _apI       = apI;
        }


        public MlbDataPlayerTeamsController() { }

        // May 22, 2019 status: this works
        // https://appac.github.io/mlb-data-api-docs/#player-data-player-teams-get
        // _pT.GetTeamsForPlayerSingleSeason("2017","493316");
        // MlbDataEndPoint newEndPoint = _endPoints.PlayerTeamsEndPoint("2017","493316");
        // http://lookup-service-prod.mlb.com/json/named.player_teams.bam?season='2014'&player_id='493316'
        // JToken allTeamValues = jObject["player_teams"]["queryResults"]["row"];
        public List<PlayerTeam> GetTeamsForPlayerSingleSeason(string year, string playerId)
        {
            MlbDataEndPoint newEndPoint     = _endPoints.PlayerTeamsEndPoint(year,playerId);
            PostmanRequest postmanRequest   = _postman.CreatePostmanRequest(newEndPoint, "PlayerTeams");
            PostmanResponse postmanResponse = _postman.GetPostmanResponse(postmanRequest);
            IRestResponse response          = postmanResponse.Response;

            // jObject: player_teams > copyRight, queryResults > created, totalSize, row
            JObject jObject = _apI.CreateModelJObject(response);

            // totalSize --> the size of "row" which is equal to number of teams (e.g., a totalSize of 2 means there are two teams shown for the player in the "row" json header)
            int totalSize = Convert.ToInt32(jObject["player_teams"]["queryResults"]["totalSize"], CultureInfo.CurrentCulture);

            // returns all keys & values for all teams the player played for
            JToken allTeamValuesJToken = _apI.CreateModelJToken(jObject,"PlayerTeam");

            List<PlayerTeam> ptList = new List<PlayerTeam>();

            for(var teamIndex = 0; teamIndex <= totalSize - 1; teamIndex++)
            {
                PlayerTeam pTeam  = new PlayerTeam();
                PlayerTeam pTeamInstance = _apI.CreateInstanceOfModel(allTeamValuesJToken[teamIndex],pTeam,"PlayerTeam") as PlayerTeam;
                ptList.Add(pTeamInstance);
            }
            return ptList;
        }


        // May 22, 2019 status: this works
        // https://appac.github.io/mlb-data-api-docs/#player-data-player-teams-get
        // _pT.GetTeamsForPlayerAllSeasons("493316");
        // http://lookup-service-prod.mlb.com/json/named.player_teams.bam?player_id='493316'
        // MlbDataEndPoint newEndPoint = _endPoints.PlayerTeamsEndPoint("493316");
        public List<PlayerTeam> GetTeamsForPlayerAllSeasons(string playerId)
        {
            MlbDataEndPoint newEndPoint     = _endPoints.PlayerTeamsEndPoint(playerId);
            PostmanRequest postmanRequest   = _postman.CreatePostmanRequest(newEndPoint, "PlayerTeams");
            PostmanResponse postmanResponse = _postman.GetPostmanResponse(postmanRequest);
            IRestResponse response          = postmanResponse.Response;

            // jObject is all of the json: player_teams > copyRight, queryResults > created, totalSize, row
            JObject jObject = _apI.CreateModelJObject(response);

            // totalSize --> the size of "row" which is equal to number of teams (e.g., a totalSize of 2 means there are two teams shown for the player in the "row" json header)
            int totalSize = Convert.ToInt32(jObject["player_teams"]["queryResults"]["totalSize"], CultureInfo.CurrentCulture);

            // returns all keys & values for all teams the player played for
            JToken allTeamValuesJToken = _apI.CreateModelJToken(jObject,"PlayerTeam");
            // JToken allTeamValues = jObject["player_teams"]["queryResults"]["row"];

            List<PlayerTeam> ptList = new List<PlayerTeam>();

            for(var teamIndex = 0; teamIndex <= totalSize - 1; teamIndex++)
            {
                PlayerTeam pTeam = new PlayerTeam();

                var pTeamInstance = _apI.CreateInstanceOfModel(allTeamValuesJToken[teamIndex],pTeam,"PlayerTeam") as PlayerTeam;
                ptList.Add(pTeamInstance);
            }
            // Console.WriteLine($"ptList Count: {ptList.Count}");
            return ptList;
        }


        // May 22, 2019 status: the code works fine but the season's the api pulls are not the same as you would see on fangraphs for some reason. E.g., Cespedes played for the Mets in 2016, but the json from the api has no 2016 info for him listed
        // Example: _pT.GetListOfPlayersSeasons("493316");
        public List<int> GetListOfPlayersSeasons (string playerId)
        {
            playerId = "493316";
            List<PlayerTeam> ptList = GetTeamsForPlayerAllSeasons("493316");

            List<int> years = new List<int>();

            foreach(PlayerTeam playersTeam in ptList)
            {
                // if (playersTeam.SportCode != "mlb")
                //     continue;
                if (!string.Equals(playersTeam.SportCode, "mlb", StringComparison.Ordinal))
                    continue;

                years.Add(Convert.ToInt32(playersTeam.LeagueSeason, CultureInfo.CurrentCulture));
            }

            foreach (var year in years)
            {
                // Console.WriteLine($"year: {year}");
            }
            return years;
        }
    }
}
