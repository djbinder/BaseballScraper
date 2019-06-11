using System;
using System.Collections.Generic;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Yahoo.YahooPlayersCollection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooPlayersCollectionController : ControllerBase
    {
        private readonly Helpers _h = new Helpers();

        private static readonly YahooApiEndPoints _endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController;


        public YahooPlayersCollectionController(YahooApiRequestController yahooApiRequestController)
        {
            _yahooApiRequestController = yahooApiRequestController;
        }

        public YahooPlayersCollectionController(){}




        // Yahoo Doc Link: https://developer.yahoo.com/fantasysports/guide/#players-collection
        // Players collection description (from Yahoo):
        //      * Enables you to get information from a collection of players simultaneously
        //      * To obtain general player info, the collection can be qualified in the URI by game, league or team
        //      * To obtain league or team related information, the collection is qualified by the relevant league or team
        //      * Each element beneath the Players Collection is a Player Resource
        // URIs
        //      * /players/{sub_resource}
        //      * /players;player_keys={player_key1},{player_key2}/{sub_resource}
        //      * /players;out={sub_resource_1},{sub_resource_2}
        //      * /players;player_keys={player_key1},{player_key2};out={sub_resource_1},{sub_resource_2}
        // Filters
        //      * position
        //          * /players;position=SP
        //      * status
        //          * /players;status=FA
        //      * search
        //          * /players;search=smith
        //      * sort
        //          * /players;sort=60
        //      * sort_type
        //          * /players;sort_type=season
        //      * sort_season
        //          * /players;sort_type=season;sort_season=2010
        //      * sort_date (Mlb, Nba, Nhl)
        //          * /players;sort_type=date;sort_date=2010-02-01
        //      * sort_week (Nfl)
        //          * /players;sort_type=week;sort_week=10
        //      * start
        //          * /players;start=25
        //      * count
        //          * /players;count=5


        // MODEL REFERENCE:
        //  *


        // END POINTS REFERENCE:
        //  * PLAYERS COLLECTION END POINTS




// https://fantasysports.yahooapis.com/fantasy/v2/players;position=SP


        [Route("test")]
        public void TestYahooPlayersCollectionController()
        {
            _h.StartMethod();

            // var x = SetFiltersAndCreateInstance("3B");

            // var y = SetFiltersAndCreateInstance("3B","FA");

            // var z = SetFiltersAndCreateInstance("3B","FA","5");

            // var aa = FilterByPlayerNameAndCreateInstance("smith");
            var ab = FilterByPlayerNameAndPositionAndCreateInstance("smith","SP");


        }


        public YahooPlayersCollection CreatePlayersCollectionInstance(string uriPlayersCollection)
        {
            JObject collectionJObject = _yahooApiRequestController.GenerateYahooResourceJObject(uriPlayersCollection);
            // _h.Dig(collectionJObject);
            JToken yahooPlayersCollectionToken = collectionJObject["fantasy_content"]["league"];

            string playersCollectionString = yahooPlayersCollectionToken.ToString();

            YahooPlayersCollection playersCollection = YahooPlayersCollection.FromJson(playersCollectionString);

            PrintPlayerNameAndTeamName(playersCollectionString);

            return playersCollection;
        }





        public YahooPlayersCollection FilterByPlayerNameAndCreateInstance(string playerName)
        {
            string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
            string uriPlayersCollection = _endPoints.PlayersCollectionForPlayerName(leagueKey, playerName).EndPointUri;
            YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
            // _h.Dig(yPc);
            return yPc;
        }

        public YahooPlayersCollection FilterByPlayerNameAndPositionAndCreateInstance(string playerName, string position)
        {
            string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
            string uriPlayersCollection = _endPoints.PlayersCollectionForPlayerNameAndPosition(leagueKey, playerName, position).EndPointUri;
            YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
            _h.Dig(yPc);
            return yPc;
        }






        public YahooPlayersCollection SetFiltersAndCreateInstance(string position)
        {
            string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
            string uriPlayersCollection = _endPoints.PlayersCollectionForPosition(leagueKey, position).EndPointUri;
            YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
            return yPc;
        }

        public YahooPlayersCollection SetFiltersAndCreateInstance(string position, string status)
        {
            string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
            string uriPlayersCollection = _endPoints.PlayersCollectionForPositionAndStatus(leagueKey, position, status).EndPointUri;
            YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
            return yPc;
        }

        public YahooPlayersCollection SetFiltersAndCreateInstance(string position, string status, string count)
        {
            string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
            string uriPlayersCollection = _endPoints.PlayersCollectionForPositionStatusAndCount(leagueKey, position, status, count).EndPointUri;
            YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
            return yPc;
        }



        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintPlayerNameAndTeamName(string PlayersCollectionString)
            {
                var playersCollection = YahooPlayersCollection.FromJson(PlayersCollectionString);
                // _h.Dig(playersCollection);

                List<Player> listOfPlayers = playersCollection.Players.Player;

                int count = listOfPlayers.Count;

                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"PLAYERS COLLECTION SEARCH || COUNT: {count}");
                Console.WriteLine("---------------------------------------------");

                foreach(var player in listOfPlayers)
                {
                    Console.WriteLine(player.Name.Full);
                    Console.WriteLine(player.EditorialTeamFullName);
                    Console.WriteLine();
                }
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine();
            }



        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}




// public void CreatePlayersCollectionInstanceX(string position)
// {
//     string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
//     string uriPlayersCollection = _endPoints.PlayersCollectionForPosition(leagueKey, position).EndPointUri;
//     JObject collectionJObject = _yahooApiRequestController.GenerateYahooResourceJObject(uriPlayersCollection);
//     JToken yahooPlayersCollectionToken = collectionJObject["fantasy_content"]["league"];
//     string playersCollectionString = yahooPlayersCollectionToken.ToString();
//     YahooPlayersCollection playersCollection = YahooPlayersCollection.FromJson(playersCollectionString);
//     PrintPlayerNameAndTeamName(playersCollectionString);
// }



    // string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
    // Console.WriteLine($"YAHOO RESOURCE CONTROLLER > leagueKey: {leagueKey}");

    // string mlbString = "mlb";

    // var positionString = "3B";
    // var positionResource = $"position={positionString}";

    // var count = "25";
    // var countResource = $"count={count}";

    // var status = "K";
    // var statusResource = $"status={status}";

    // // string uriString = $"https://fantasysports.yahooapis.com/fantasy/v2/{mlbString}/players;position=SP ";
    // // string uriString = $"https://fantasysports.yahooapis.com/fantasy/v2/league/{leagueKey}/players;position=SP";
    // // string uriString = $"https://fantasysports.yahooapis.com/fantasy/v2/league/{leagueKey}/players;position=3B";
    // string uriString = $"https://fantasysports.yahooapis.com/fantasy/v2/league/{leagueKey}/players;{positionResource};{countResource};{statusResource}";

    // Console.WriteLine(uriString);

    // // var uriPlayersCollection = _endPoints.PlayersCollectionForPosition("3B", leagueKey).EndPointUri;
    // var uriPlayersCollection = _endPoints.PlayersCollectionForPositionAndStatus(leagueKey,"3B","K").EndPointUri;
    // Console.WriteLine($"YAHOO PLAYERS COLLECTION > uriPlayersCollection : {uriPlayersCollection} ");

    // // 2 sub-objects
    // // JObject collectionJObject = _yahooApiRequestController.GenerateYahooResourceJObject(uriString);
    // JObject collectionJObject = _yahooApiRequestController.GenerateYahooResourceJObject(uriPlayersCollection);
    // // _h.Dig(collectionJObject);

    // var yahooPlayersCollectionToken = collectionJObject["fantasy_content"]["league"];

    // string playersCollectionString = yahooPlayersCollectionToken.ToString();

    // var playersCollection = YahooPlayersCollection.FromJson(playersCollectionString);
    // _h.Dig(playersCollection);
