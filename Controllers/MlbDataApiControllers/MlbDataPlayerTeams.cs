using System;
using System.Collections.Generic;
using System.Linq;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.MlbDataApi;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using static BaseballScraper.EndPoints.MlbDataApiEndPoints;
using static BaseballScraper.Infrastructure.PostmanMethods;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.MlbDataApiControllers
{
    public class MlbDataPlayerTeams
    {
        private static readonly Helpers _h = new Helpers();

        private static readonly MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();

        private static readonly PostmanMethods _postman = new PostmanMethods();

        private static readonly ApiInfrastructure _apI = new ApiInfrastructure();


        public MlbDataPlayerTeams() { }


        // https://appac.github.io/mlb-data-api-docs/#player-data-player-teams-get
        // _pT.GetTeamsForPlayerSingleSeason("2017","493316");
        public List<PlayerTeam> GetTeamsForPlayerSingleSeason(string year, string playerId)
        {
            // http://lookup-service-prod.mlb.com/json/named.player_teams.bam?season='2014'&player_id='493316'
            // MlbDataEndPoint newEndPoint = _endPoints.PlayerTeamsEndPoint("2017","493316");
            MlbDataEndPoint newEndPoint = _endPoints.PlayerTeamsEndPoint(year,playerId);

            PostmanRequest postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PlayerTeams");

            PostmanResponse postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            IRestResponse response = postmanResponse.Response;

            // jObject.Count: 1
            // jObject: player_teams > copyRight, queryResults > created, totalSize, row
            JObject jObject = _apI.CreateModelJObject(response);

            // totalSize --> the size of "row" which is equal to number of teams (e.g., a totalSize of 2 means there are two teams shown for the player in the "row" json header)
            int totalSize = Convert.ToInt32(jObject["player_teams"]["queryResults"]["totalSize"]);

            // returns all keys & values for all teams the player played for
            JToken allTeamValues = jObject["player_teams"]["queryResults"]["row"];

            List<PlayerTeam> ptList = new List<PlayerTeam>();

            for(var teamIndex = 0; teamIndex <= totalSize - 1; teamIndex++)
            {
                JToken team = allTeamValues[teamIndex];
                PlayerTeam pt = new PlayerTeam
                {
                    OrgFull = team["org_full"].ToString(),
                    LeagueSeason = Convert.ToInt32(team["league_season"]),
                };
                _h.Intro(pt.OrgFull,"org full");
                _h.Intro(pt.LeagueSeason,"league_season");
            }

            return ptList;

        }



        // _pT.GetTeamsForPlayerAllSeasons("493316");
        public List<PlayerTeam> GetTeamsForPlayerAllSeasons(string playerId)
        {
            // http://lookup-service-prod.mlb.com/json/named.player_teams.bam?player_id='493316'
            // MlbDataEndPoint newEndPoint = _endPoints.PlayerTeamsEndPoint("493316");
            MlbDataEndPoint newEndPoint = _endPoints.PlayerTeamsEndPoint(playerId);

            PostmanRequest postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PlayerTeams");

            PostmanResponse postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            IRestResponse response = postmanResponse.Response;

            // jObject.Count: 1
            // jObject: player_teams > copyRight, queryResults > created, totalSize, row
            JObject jObject = _apI.CreateModelJObject(response);

            // totalSize --> the size of "row" which is equal to number of teams (e.g., a totalSize of 2 means there are two teams shown for the player in the "row" json header)
            int totalSize = Convert.ToInt32(jObject["player_teams"]["queryResults"]["totalSize"]);

            // returns all keys & values for all teams the player played for
            JToken allTeamValues = jObject["player_teams"]["queryResults"]["row"];

            List<PlayerTeam> ptList = new List<PlayerTeam>();

            for(var teamIndex = 0; teamIndex <= totalSize - 1; teamIndex++)
            {
                PlayerTeam pTeam = new PlayerTeam();

                var pTeamInstance = _apI.CreateInstanceOfModel(allTeamValues[teamIndex],pTeam,"PlayerTeam") as PlayerTeam;

                ptList.Add(pTeamInstance);
            }
            Console.WriteLine($"ptList Count: {ptList.Count}");
            return ptList;

        }
    }
}





// JToken team = allTeamValues[teamIndex];
// PlayerTeam pt = new PlayerTeam
// {
//     OrgFull = team["org_full"].ToString(),
//     LeagueSeason = Convert.ToInt32(team["league_season"]),
// };
// _h.Intro(pt.OrgFull,"org full");
// _h.Intro(pt.LeagueSeason,"league_season");



// JToken playerTeamsToken = jObject["player_teams"];
// _h.Intro(playerTeamsToken,"playerTeamsToken");
// PlayerTeams pts = new PlayerTeams();

// _apI.CreateInstanceOfModel(allTeamsToken,pts,"PlayerTeams");
// Console.WriteLine(pts.PlayerTeamsRepeater.CopyRight);
// // Console.WriteLine(playerTeams);
// // // Console.WriteLine(playerTeams.PlayerTeamsRepeater.PlayerTeamsQueryResults.TotalSize.Value);
// // playerTeams.PlayerTeamsRepeater.PlayerTeamsQueryResults.TotalSize = totalSizeInt;
// // Console.WriteLine(playerTeams.PlayerTeamsRepeater.PlayerTeamsQueryResults.TotalSize);

// // _h.Spotlight(playerTeams.ToString());

// // var playerTeamsJson = playerTeams.ToJson();
// // _h.Intro(playerTeamsJson,"playerTeamsJson");

// // var playerTeamsRepeater = playerTeams.PlayerTeamsRepeater;
// // _h.Intro(playerTeamsRepeater,"playerTeamsRepeater");

// PlayerTeam pt = new PlayerTeam();

// var rows = allTeamsToken["queryResults"]["row"];


// _apI.CreateMultipleInstancesOfModelByLooping(rows,pt,"PlayerTeam");



// Console.WriteLine(pt.OrgFull);





// // int tokenCounter = 0;
// // foreach(var token in allTeamsToken)
// // {
// //     Console.WriteLine(token);
// //     // Console.WriteLine(token["queryResults"]["totalSize"]);
// //     // PlayerTeam x = _apI.CreateInstanceOfModel(token,pt,"PlayerTeam") as PlayerTeam;
// //     // Console.WriteLine(x.StartDate);
// //     _h.Spotlight("BREAK");
// //     tokenCounter++;
// // }
