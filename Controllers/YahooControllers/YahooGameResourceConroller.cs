using System;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;



#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooGameResourceConroller : ControllerBase
    {
        private readonly Helpers _h = new Helpers();

        private static YahooApiRequestController _yahooApiRequestController = new YahooApiRequestController();

        private readonly YahooAuthController _yahooAuthController = new YahooAuthController();

        public static readonly JsonHandler.NewtonsoftJsonHandlers _newtonHandler = new JsonHandler.NewtonsoftJsonHandlers();

        public static readonly string theGameConfigFilePath = "Configuration/theGameIsTheGameConfig.json";


        public YahooGameResourceConroller(YahooApiRequestController yahooApiRequestController, YahooAuthController yahooAuthController)
        {
            _yahooApiRequestController = yahooApiRequestController;
            _yahooAuthController = yahooAuthController;
        }

        public YahooGameResourceConroller(){}



        // Yahoo Doc Link: https://developer.yahoo.com/fantasysports/guide/#game-resource
        // Game resource description (from Yahoo):
        //      * Game API gets fantasy game (i.e., sport/league) information
        //      * Examples: fantasy game name, the Yahoo! game code, and season
        //      * To refer to a Game resource, you’ll need to provide a game_key
        //      * game_key will be either a game_id or game_code.
        //      * 'game_id': unique ID for a given fantasy game for a given season
        //      * 'game_code': generally identifies a game, independent of season, and, when used as a game_key, will typically return the current season of that game
        //      * If you want the current season of a game, use game_code as the game_key.
        // URIs
        //      * https://fantasysports.yahooapis.com/fantasy/v2/game/
        //      * https://fantasysports.yahooapis.com/fantasy/v2/game//
        //      * https://fantasysports.yahooapis.com/fantasy/v2/game/;out=,{sub_resource_2}







        [Route("test")]
        public void TestYahooGameResourceController()
        {
            _h.StartMethod();

        }



        //         // SEE: https://developer.yahoo.com/fantasysports/guide/#description
        // #region GENERATE YAHOO LEAGUE KEY ------------------------------------------------------------


        //     // STATUS [ June 7, 2019 ] : this works
        //     // STEP 1
        //     /// <summary>
        //     ///     The yahoo game id for mlb changes each season
        //     ///     This method gets the id for the current year
        //     ///     Method is ultimately called in 'GetTheGameIsTheGameLeagueKey' method
        //     /// </summary>
        //     /// <returns>
        //     ///     A string that is three numbers
        //     ///     e.g., 378 OR 388
        //     /// </returns>
        //     public string GetYahooMlbGameKeyForThisYear()
        //     {
        //         _h.StartMethod();
        //         var gameLink = "https://fantasysports.yahooapis.com/fantasy/v2/game/mlb";
        //         var gameObject = _yahooApiRequestController.GenerateYahooResourceJObject(gameLink);
        //         var gameKey = gameObject["fantasy_content"]["game"]["game_key"].ToString();
        //         return gameKey;
        //     }


        //     // STATUS [ June 7, 2019 ] : this works
        //     // STEP 2
        //     /// <summary>
        //     ///     Each yahoo league has a unique id
        //     ///     This method gets the id for the league you want data from
        //     ///     The league key suffix is in a config file
        //     ///     Method is ultimately called in 'GetTheGameIsTheGameLeagueKey' method
        //     /// </summary>
        //     /// <returns>
        //     ///     A string of lowercase L + the league id
        //     ///     e.g., l.1234 OR l.679
        //     /// </returns>
        //     public string GetTheGameIsTheGameLeagueKeySuffix()
        //     {
        //         _h.StartMethod();
        //         TheGameIsTheGameConfiguration theGameConfig = new TheGameIsTheGameConfiguration();
        //         Type theGameConfigType = theGameConfig.GetType();

        //         var configObject = _newtonHandler.DeserializeJsonFromFile(theGameConfigFilePath, theGameConfigType) as TheGameIsTheGameConfiguration;

        //         string leagueKeySuffix = configObject.LeagueKeySuffix;
        //         return leagueKeySuffix;
        //     }


        //     // STATUS [ June 7, 2019 ] : this works
        //     // STEP 3
        //     /// <summary>
        //     ///     To get league day you need the league key
        //     ///     The league key is a combo of the mlb game key and the league suffix
        //     ///     Methods calls the two previous methods (STEP 1 and STEP 2)
        //     /// </summary>
        //     /// <returns>
        //     ///     A string that is the Y! mlb game key and the league suffix
        //     ///     e.g., 378.l.1234 OR 388.l.679
        //     /// </returns>
        //     public string GetTheGameIsTheGameLeagueKey()
        //     {
        //         _h.StartMethod();
        //         var gameKey = GetYahooMlbGameKeyForThisYear();
        //         var theGameKeySuffix = GetTheGameIsTheGameLeagueKeySuffix();
        //         var leagueKey = $"{gameKey}{theGameKeySuffix}";
        //         // PrintLeagueKeyDetails(gameKey,theGameKeySuffix,leagueKey);
        //         return leagueKey;
        //     }


        // #endregion GENERATE YAHOO LEAGUE KEY ------------------------------------------------------------


    }
}
