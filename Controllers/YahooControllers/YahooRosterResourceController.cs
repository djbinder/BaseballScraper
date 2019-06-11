using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Yahoo.YahooRosterResource;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooRosterResourceController : ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private static YahooApiEndPoints _endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController = new YahooApiRequestController();
        private readonly YahooAuthController _yahooAuthController = new YahooAuthController();
        private readonly PlayerBaseController _playerBaseController;

        // public static readonly YahooGameResourceConroller _yahooGameResourceController = new YahooGameResourceConroller();


        public YahooRosterResourceController(YahooApiRequestController yahooApiRequestController, YahooAuthController yahooAuthController, PlayerBaseController playerBaseController)
        {
            _yahooApiRequestController = yahooApiRequestController;
            _yahooAuthController = yahooAuthController;
            _playerBaseController = playerBaseController;
        }

        public YahooRosterResourceController(){}



        // Yahoo Doc Link: https://developer.yahoo.com/fantasysports/guide/#roster-resource
        // Roster resource description (from Yahoo):
        //      * Players on a team are organized into rosters corresponding to:
        //          * Certain weeks in the NFL
        //          * Crtain dates in MLB, NBA, and NHL
        //      * Each player is assigned a position if they’re in the starting lineup
        //      * You only get credit for stats from players in your starting lineup
        //      * You can use this API to edit your lineup by:
        //          * PUTting up new positions for the players on a roster
        //      * You can add/drop players from your roster by:
        //          *  `POSTing new transactions <#transactions-collection-POST>`__ to the league’s transactions collection.
        // URIs
        //      * https://fantasysports.yahooapis.com/fantasy/v2/team//roster
        //      * https://fantasysports.yahooapis.com/fantasy/v2/team//roster/
        //      * For NFL:
        //          * https://fantasysports.yahooapis.com/fantasy/v2/team//roster;week=10
        //      * For MLB, NBA, NHL
        //          * https://fantasysports.yahooapis.com/fantasy/v2/team//roster;date=2011-05-01
        //      * Example:
        //          * // https://fantasysports.yahooapis.com/fantasy/v2/team/253.l.102614.t.10/roster/players


        // MODEL REFERENCE:
        //  * Models/Yahoo/YahooPRosterResource.cs






        [Route("test")]
        public void TestYahooRosterResourceController()
        {
            _h.StartMethod();
        }


        #region YAHOO ROSTER RESOURCE - PRIMARY METHODS ------------------------------------------------------------


            // STATUS [ June 10, 2019 ] : this works
            /// <summary>
            ///     Instantiate new instance of yahoo roster resource
            /// </summary>
            /// <param name="TeamNumber">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <example>
            ///     var yahooRosterResourceInstance = CreateYahooRosterResourceInstance(1);
            /// </example>
            public YahooRosterResource CreateYahooRosterResourceInstance(int TeamNumber)
            {
                _h.StartMethod();
                var yahooRosterResource = new YahooRosterResource();

                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                // Console.WriteLine($"YAHOO RESOURCE CONTROLLER > leagueKey: {leagueKey}");

                // var rosterResourceUri = "https://fantasysports.yahooapis.com/fantasy/v2/team/388.l.9908.t.10/roster/players";
                var rosterResourceUri = _endPoints.RosterResourceEndPoint(leagueKey, TeamNumber).EndPointUri;
                // Console.WriteLine($"YAHOO ROSTER RESOURCE > rosterResourceUri : {rosterResourceUri} ");

                JObject rosterResourceJObject = _yahooApiRequestController.GenerateYahooResourceJObject(rosterResourceUri);
                // _h.Dig(rosterResourceJObject);

                JToken rosterResource = rosterResourceJObject["fantasy_content"]["team"];
                string rosterResourceString = rosterResource.ToString();
                // Console.WriteLine(rosterResourceString);

                yahooRosterResource = JsonConvert.DeserializeObject<YahooRosterResource>(rosterResourceString);
                _h.Dig(yahooRosterResource);

                return yahooRosterResource;
            }


        #endregion YAHOO ROSTER RESOURCE - PRIMARY METHODS ------------------------------------------------------------

    }
}
