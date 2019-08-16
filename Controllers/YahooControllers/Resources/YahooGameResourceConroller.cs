using System;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;



#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers.Resources
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
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

    }
}
