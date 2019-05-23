using System;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.MlbDataApi;
using BaseballScraper.Infrastructure;

using RestSharp;
using static BaseballScraper.Infrastructure.PostmanMethods;
using static BaseballScraper.EndPoints.MlbDataApiEndPoints;

namespace BaseballScraper.Controllers.MlbDataApiControllers
{

    public class MlbDataSeasonHittingStats
    {
        private static readonly HitterSeasonStats _hSS = new HitterSeasonStats();

        private static readonly MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();

        private static readonly PostmanMethods _postman = new PostmanMethods();

        private static readonly ApiInfrastructure _apI = new ApiInfrastructure();




        // /json/named.sport_pitching_tm.bam?league_list_id='mlb'&game_type={game_type}&season={season}&player_id={player_id}
        public void GetStatsForSeason ()
        {
            // http://lookup-service-prod.mlb.com/json/named.sport_hitting_tm.bam?league_list_id='mlb'&game_type='R'&season='2017'&player_id='592789'
            MlbDataEndPoint newEndPoint = _endPoints.HitterSeasonEndPoint("R","2017","592789");
            // Console.WriteLine($"newEndPoint: {newEndPoint.EndPointUri}");

            PostmanRequest postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "HitterSeasonStats");

            PostmanResponse postmanResponse = _postman.GetPostmanResponse(postmanRequest);

            IRestResponse response = postmanResponse.Response;

            Console.WriteLine($"response: {response.Content}");
            Console.WriteLine();

            var jObject = _apI.CreateModelJObject(response);
            Console.WriteLine($"jObject: {jObject}");
            Console.WriteLine();

            var jToken = _apI.CreateModelJToken(jObject,"HitterSeasonStats");
            Console.WriteLine($"jToken: {jToken}");
            Console.WriteLine();

            var hitter = _apI.CreateInstanceOfModel(jToken,_hSS,"HitterSeasonStats");
            Console.WriteLine($"hitter: {hitter}");
            Console.WriteLine();
        }
    }
}
