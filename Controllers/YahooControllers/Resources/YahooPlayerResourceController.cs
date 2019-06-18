using System;
using System.Collections.Generic;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Player;
using BaseballScraper.Models.Yahoo;
using BaseballScraper.Models.Yahoo.Resources.YahooPlayerResource;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers.Resources
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooPlayerResourceController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private static readonly YahooApiEndPoints _endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController;
        private readonly YahooAuthController _yahooAuthController = new YahooAuthController();
        private readonly PlayerBaseController _playerBaseController;
        private readonly PlayerBaseController.PlayerBaseFromExcel _playerBaseFromExcel = new PlayerBaseController.PlayerBaseFromExcel();


        public YahooPlayerResourceController(YahooApiRequestController yahooApiRequestController, YahooAuthController yahooAuthController, PlayerBaseController playerBaseController)
        {
            _yahooApiRequestController = yahooApiRequestController;
            _yahooAuthController = yahooAuthController;
            _playerBaseController = playerBaseController;
        }

        public YahooPlayerResourceController(){}



        // Yahoo Doc Link: https://developer.yahoo.com/fantasysports/guide/#player-resource
        // Player resource description (from Yahoo):
        //      * With the Player API, you can obtain the player (athlete) related information
        //      * Examples: their name, professional team, and eligible positions
        //      * The player is identified in the context of a particular game, and can be requested as the base of your URI by using the global ````.
        // URIs
        //      * https://fantasysports.yahooapis.com/fantasy/v2/player/
        //      * https://fantasysports.yahooapis.com/fantasy/v2/player//
        //      * https://fantasysports.yahooapis.com/fantasy/v2/player/;out=,{sub_resource_2}


        // MODEL REFERENCE:
        //  * Models/Yahoo/YahooPlayerResource.cs





        [Route("test")]
        public void TestYahooPlayerResourceController()
        {
            _h.StartMethod();
        }




        #region YAHOO PLAYER RESOURCE - PRIMARY METHODS ------------------------------------------------------------


            // STATUS [ June 10, 2019 ] : this works
            /// <summary>
            ///     Instantiate new instance of yahoo player resource
            /// </summary>
            /// <param name="yahooPlayerId">
            ///     The Mlb players yahoo player id
            ///     Typically four or five numbers but fed to method as a string
            ///         E.g., "8967" instead of 8967
            /// </param>
            /// <remarks>
            ///     If you do not know the yahoo id, try the 'GetYahooPlayersIdFromPlayerName()' method to get it
            /// </remarks>
            /// <example>
            ///     var yahooPlayerResourceModel = CreateYahooPlayerResourceInstances("8967");
            /// </example>
            public YahooPlayerResource CreateYahooPlayerResourceInstances(string yahooPlayerId)
            {
                // string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                // Console.WriteLine($"YAHOO RESOURCE CONTROLLER > leagueKey: {leagueKey}");

                string keyPrefix = "mlb.p.";
                string playerKey = $"{keyPrefix}{yahooPlayerId}";

                // e.g., https://fantasysports.yahooapis.com/fantasy/v2/player/mlb.p.8967
                var uriPlayer = _endPoints.PlayerBaseEndPoint(playerKey).EndPointUri;

                JObject playerJObject = _yahooApiRequestController.GenerateYahooResourceJObject(uriPlayer);

                JToken playerResource = playerJObject["fantasy_content"]["player"];
                string playerResourceString = playerResource.ToString();

                YahooPlayerResource yPlayerResource = JsonConvert.DeserializeObject<YahooPlayerResource>(playerResourceString);
                _h.Dig(yPlayerResource);

                return yPlayerResource;
            }


            // STATUS [ June 10, 2019 ] : this works
            /// <summary>
            ///     Instantiate multiple instances of yahoo player resource
            /// </summary>
            /// <example>
            ///     string rizzoId = "8868";
            ///     string goldschmidtId = "8967";
            ///     List<string> playerIds = new List<string>
            ///     {
            ///         rizzoId,
            ///         goldschmidtId
            ///     };
            ///     var playerList = CreateListOfYahooPlayerResourceInstances(playerIds);
            /// </example>
            public List<YahooPlayerResource> CreateListOfYahooPlayerResourceInstances(List<string> ListOfYahooPlayerIds)
            {
                List<YahooPlayerResource> yahooPlayerResourceList = new List<YahooPlayerResource>();
                YahooPlayerResource yahooPlayerResource = new YahooPlayerResource();

                foreach(var id in ListOfYahooPlayerIds)
                {
                    yahooPlayerResource = CreateYahooPlayerResourceInstances(id);
                    yahooPlayerResourceList.Add(yahooPlayerResource);
                }

                // _h.Dig(yahooPlayerResourceList);
                return yahooPlayerResourceList;
            }


        #endregion YAHOO PLAYER RESOURCE - PRIMARY METHODS ------------------------------------------------------------





        #region YAHOO PLAYER RESOURCE - SUPPORT METHODS ------------------------------------------------------------


            // STATUS [ June 10, 2019 ] : this works
            /// <summary>
            ///     Retrieves yahoo player id from player's yahoo name
            ///     This is helpful with primary methods if you do not know the player's yahoo id
            /// </summary>
            /// <example>
            ///     var playerId = GetYahooPlayersIdFromPlayerName("Anthony Rizzo");
            /// </example>
            public string GetYahooPlayersIdFromPlayerName(string PlayerName)
            {
                var yahooPlayerId = "";
                IEnumerable<PlayerBase> playerBase = _playerBaseFromExcel.GetOnePlayersBaseFromYahooName(PlayerName);
                // _h.Dig(playerBase);

                var enumerator = playerBase.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    yahooPlayerId = enumerator.Current.YahooPlayerId;
                }

                return yahooPlayerId;
            }


        #endregion YAHOO PLAYER RESOURCE - SUPPORT METHODS ------------------------------------------------------------
    }
}
