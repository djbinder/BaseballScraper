using System;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.MlbDataApi;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using static BaseballScraper.Infrastructure.PostmanMethods;
using static BaseballScraper.EndPoints.MlbDataApiEndPoints;
using C = System.Console;


namespace BaseballScraper.Controllers.MlbDataApiControllers
{
    [Route("api/mlb/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MlbDataSeasonHittingStatsController : ControllerBase
    {
        private readonly Helpers             _helpers;
        private readonly ApiInfrastructure   _apiInfrastructure;
        private readonly MlbDataApiEndPoints _endPoints;
        private readonly PostmanMethods      _postman;

        // private readonly string _testPlayerId = "592789";

        private static readonly HitterSeasonStats _hSS = new HitterSeasonStats();


        public MlbDataSeasonHittingStatsController(Helpers helpers, MlbDataApiEndPoints endPoints, PostmanMethods postman, ApiInfrastructure apiInfrastructure)
        {
            _helpers           = helpers;
            _endPoints         = endPoints;
            _postman           = postman;
            _apiInfrastructure = apiInfrastructure;
        }


        public MlbDataSeasonHittingStatsController(){}


        /*
            https://127.0.0.1:5001/api/mlb/MlbDataSeasonHittingStats/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            var hitter = CreateHitterSeasonStatsInstance("2019", "430911");
        }


        // /json/named.sport_pitching_tm.bam?league_list_id='mlb'&game_type={game_type}&season={season}&player_id={player_id}
        // http://lookup-service-prod.mlb.com/json/named.sport_hitting_tm.bam?league_list_id='mlb'&game_type='R'&season='2017'&player_id='592789'
        public HitterSeasonStats CreateHitterSeasonStatsInstance (string year, string playerId)
        {
            MlbDataEndPoint newEndPoint     = _endPoints.HitterSeasonEndPoint("R",year,playerId);
            PostmanRequest postmanRequest   = _postman.CreatePostmanRequest(newEndPoint, "HitterSeasonStats");
            PostmanResponse postmanResponse = _postman.GetPostmanResponse(postmanRequest);
            IRestResponse response          = postmanResponse.Response;

            var jObject = _apiInfrastructure.CreateModelJObject(response);
            // _helpers.Dig(jObject);
            var jToken  = _apiInfrastructure.CreateModelJToken(jObject,"HitterSeasonStats");
            var hitter  = _apiInfrastructure.CreateInstanceOfModel(jToken,_hSS,"HitterSeasonStats") as HitterSeasonStats;

            // _helpers.Dig(hitter);
            return hitter;

        }
    }
}

// C.WriteLine($"response: {response.Content}\n");
// C.WriteLine($"jObject: {jObject}\n");
// C.WriteLine($"jToken: {jToken}\n");
// C.WriteLine($"hitter: {hitter}\n");


// http://lookup-service-prod.mlb.com/json/named.sport_hitting_tm.bam?league_list_id='mlb'&game_type='R'&season='2017'&player_id='493316'



// JOBject example

// {
//   "sport_hitting_tm": {
//     "copyRight": " Copyright 2019 MLB Advanced Media, L.P.  Use of any content on this page acknowledges agreement to the terms posted here http://gdx.mlb.com/components/copyright.txt  ",
//     "queryResults": {
//       "created": "2019-07-15T20:19:29",
//       "totalSize": "1",
//       "row": {
//         "gidp": "0",
//         "sac": "1",
//         "np": "52",
//         "sport_code": "mlb",
//         "hgnd": "1",
//         "tb": "2",
//         "gidp_opp": "1",
//         "sport_id": "1",
//         "bb": "3",
//         "avg": ".222",
//         "slg": ".222",
//         "team_full": "New York Mets",
//         "ops": ".639",
//         "hbp": "0",
//         "league_full": "National League",
//         "team_abbrev": "NYM",
//         "so": "5",
//         "hfly": "0",
//         "wo": "0",
//         "league_id": "104",
//         "sf": "0",
//         "team_seq": "1",
//         "league": "NL",
//         "hpop": "0",
//         "cs": "0",
//         "season": "2017",
//         "sb": "0",
//         "go_ao": "2.00",
//         "ppa": "4.00",
//         "player_id": "592789",
//         "ibb": "0",
//         "team_id": "121",
//         "roe": "0",
//         "go": "2",
//         "hr": "0",
//         "rbi": "0",
//         "babip": ".500",
//         "lob": "5",
//         "end_date": "2018-05-25T00:00:00",
//         "xbh": "0",
//         "league_short": "National",
//         "g": "7",
//         "d": "0",
//         "sport": "MLB",
//         "team_short": "NY Mets",
//         "tpa": "13",
//         "h": "2",
//         "obp": ".417",
//         "hldr": "1",
//         "t": "0",
//         "ao": "1",
//         "r": "0",
//         "ab": "9"
//       }
//     }
//   }
// }
